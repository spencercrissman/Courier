using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Courier.Core;
using System.IO.Packaging;

namespace Umbraco.Courier.RepositoryProviders.NetworkshareProvider
{
    public class PackagedNetworkShareProvider : RepositoryProvider
    {
        private XmlNode _settingsXml;

        #region Constructors (1)

        public PackagedNetworkShareProvider()
        {
            this.Name = "PackagedNetworkshare";
            this.Description = "Exposes a revision repository in a network share, revisions are compressed into a single package";
            this.Id = ProviderIdCollection.CompressedNetworkShareRepositoryProviderGuid;
        }

        #endregion Constructors

        public override void LoadSettings(System.Xml.XmlNode settingsXml)
        {
            Path = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode(".//path"));
        }

        public override bool HasFolders
        {
            get
            {
                return true;
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                var newPath = value;
                if (!System.IO.Path.IsPathRooted(newPath))
                    newPath = Core.Context.Current.MapPath(newPath);

                _path = newPath;
            }
        }

        public override void CommitRevision(Revision revision)
        {
            CommitRevision(revision, "");
        }

        public override void CommitRevision(Revision revision, string folder)
        {
            var filename = GetFileFromAlias(revision.Name, folder);

            if (File.Exists(filename))
                File.Delete(filename);

            if (File.Exists(filename))
                throw new Exception("An archive with the same name already exists.");

            using (var package = Package.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                package.PackageProperties.Identifier = revision.Name;
                package.PackageProperties.Title = "Umbraco Courier 2.0 Package - " + revision.Name;
                package.PackageProperties.LastModifiedBy = "Umbraco Courier 2.0";
                package.PackageProperties.Creator = "Umbraco Courier 2.0";
                package.PackageProperties.Created = DateTime.Now;
                package.PackageProperties.Modified = DateTime.Now;
                package.PackageProperties.ContentType = "application/courierPackage";

                foreach (var item in revision.RevisionCollection)
                    AddPackagePart(package, item.Value.FilePath, item.Value.FileContent, "Revision");

                foreach (var item in revision.ResourceCollection)
                    AddPackagePart(package, item.Value.FilePath, item.Value.FileContent, "Resource");

                foreach (var item in revision.VirtualResourceCollection)
                    AddPackagePart(package, item.Value.FilePath, item.Value.FileContent, "VirtualResource");
            }
        }

        private static void AddPackagePart(Package package, string filePath, byte[] fileContents, string type)
        {
            var parts = filePath.Split(new[]{'\\'}, StringSplitOptions.RemoveEmptyEntries);
            parts[parts.Length - 1] = Uri.EscapeDataString(parts[parts.Length - 1]);
            filePath = "/"+string.Join("/", parts);

            var packagePart = package.CreatePart(new Uri(filePath, UriKind.Relative), "application/courier" + type, CompressionOption.Normal);
            using (var stream = packagePart.GetStream(FileMode.Create, FileAccess.Write))
                stream.Write(fileContents, 0, fileContents.Length);
        }

        private string GetFileFromAlias(string alias, string folder)
        {
            return System.IO.Path.Combine(Path, folder ?? "", alias + ".zip");
        }

        public override Revision GetRevision(string alias, string folder)
        {
            return GetRevision(alias, new FileInfo(GetFileFromAlias(alias, folder)));
        }

        public override Revision GetRevision(string alias)
        {
            return GetRevision(alias, "");
        }

        public Revision GetRevision(string alias, Stream packageStream)
        {
            return GetRevision(alias, null, packageStream);
        }

        private Revision GetRevision(string alias, FileSystemInfo fileInfo = null, Stream packageStream = null)
        {
            var r = new Revision();
            r.Name = alias;
            if (fileInfo != null)
                r.LastModified = fileInfo.LastWriteTime;

            using (var archive = fileInfo == null ? Package.Open(packageStream) : Package.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                foreach (var packagePart in archive.GetParts().Where(e => e.Uri.ToString().TrimStart('/').StartsWith(Umbraco.Courier.Core.Settings.RevisionFolderName.Trim())))
                {
                    var path = Uri.UnescapeDataString(packagePart.Uri.ToString()).Replace("/","\\").TrimStart('\\');
                    var item = new RevisionItem { FilePath = path };
                    var bytes = GetBytes(packagePart);

                    ItemIdentifier id = null;
                    var name = "";

                    Core.Serialization.Serializer.GetIdentifier(bytes, ref id, ref name);

                    item.Name = name;
                    item.ItemId = id;
                    item.FileContent = bytes;
                    item.InternalFile = false;

                    r.RevisionCollection.Add(path, item);
                }

                foreach (var packagePart in archive.GetParts().Where(e => e.Uri.ToString().TrimStart('/').StartsWith(Umbraco.Courier.Core.Settings.ResourcesFolderName.Trim())))
                {
                    var path = Uri.UnescapeDataString(packagePart.Uri.ToString()).Replace("/", "\\").TrimStart('\\');
                    var item = new ResourceItem { FilePath = path };
                    item.FileContent = GetBytes(packagePart);
                    item.Name = path;
                    r.ResourceCollection.Add(item.Name, item);
                }

                foreach (var packagePart in archive.GetParts().Where(e => e.Uri.ToString().TrimStart('/').StartsWith(Umbraco.Courier.Core.Settings.VirtualResourcesFolderName.Trim())))
                {
                    var path = Uri.UnescapeDataString(packagePart.Uri.ToString()).Replace("/", "\\").TrimStart('\\');
                    var item = new ResourceItem { FilePath = path };
                    item.FileContent = GetBytes(packagePart);
                    item.Virtual = true;
                    item.Name = path;
                    r.ResourceCollection.Add(item.Name, item);
                }
            }

            return r;
        }

        private byte[] GetBytes(PackagePart packagePart)
        {
            using (var entryStream = packagePart.GetStream())
            {
                var bytes = new byte[entryStream.Length];
                entryStream.Read(bytes, 0, (int)entryStream.Length);
                return bytes;
            }
        }


        public override string[] GetAvailableRevisions()
        {
            return GetAvailableRevisions(null);
        }

        public override string[] GetAvailableRevisions(string parentFolder)
        {
            return System.IO.Directory.GetFiles(GetRevisionFolderPath(parentFolder), "*.zip")
                .Select(GetAliasFromFile)
                .ToArray();
        }

        public override string[] GetAvailableFolders(string parentFolder = null)
        {
            return System.IO.Directory.GetDirectories(GetRevisionFolderPath(parentFolder), "*.*")
                .Select(GetAliasFromDirectory)
                .Where(d => d != null)
                .ToArray();
        }

        private string GetRevisionFolderPath(string parentFolder)
        {
            var path = Path;
            if (!String.IsNullOrEmpty(parentFolder))
                path = System.IO.Path.Combine(path, parentFolder);
            return path;
        }

        private string GetAliasFromDirectory(string f)
        {
            return f.Substring(Path.Length).Trim('\\');
        }

        private static string GetAliasFromFile(string f)
        {
            return System.IO.Path.GetFileNameWithoutExtension(f);
        }
    }
}