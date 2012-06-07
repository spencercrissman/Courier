using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using SharpSvn;
using System.Net;
using System.Collections.ObjectModel;
using Umbraco.Courier.Core.Storage;
using System.IO;
using System.Threading;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.SubversionRepository
{
    public class SubversionRepository : RepositoryProvider
    {

        public SubversionRepository()
        {
            this.Name = "Subversion Repository";
            this.Description = "Uses subversion as storage of items to allow connection to VS, through ankh.svn, NOTICE, you cannot submit data to this repository yet. You can only pull data from it";
            this.Id = new Guid("e0474ca8-e73b-11df-9492-0800200c9a66");
        }

        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string LocationAlias { get; set; }

        private string TempFolderPath(string alias)
        {
            string name =  Context.Current.MapPath(Core.Settings.rootPath + "/svnRepoTemp/" + LocationAlias + "/" + alias);
            return name;
        }

        public override string[] GetAvailableRevisions()
        {
            string url = Url.TrimEnd('/');

            using (SvnClient client = new SvnClient())
            {
                
                client.LoadConfiguration("path");
                client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
                SvnUriTarget target = new SvnUriTarget(url);

                List<String> filesFound = new List<String>();
                Collection<SvnListEventArgs> listResults;

                if (client.GetList(target, out listResults))
                {
                    foreach (SvnListEventArgs item in listResults)

                        if (item.Entry.NodeKind == SvnNodeKind.Directory && !string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Path))
                            filesFound.Add(item.Path);

                    return filesFound.ToArray();
                }
            }

            return new string[0];
        }

        public override Revision GetRevision(string alias)
        {
            string url = Url.TrimEnd('/') + "/" + alias;
            string tempFolder = TempFolderPath(alias);

            //ensure we have the latest version
            checkout(alias);

            RevisionStorage rs = new RevisionStorage();
            var r = rs.GetFromDirectory(alias, tempFolder);

            rs.Save(r, alias);

            rs.Dispose();

            return r;
        }

        private void checkout(string alias)
        {
            string projectUrl = Url.TrimEnd('/') + "/" + alias;
            string tempFolder = TempFolderPath(alias);

            bool workingCopy = false;

            if (Directory.Exists(Path.Combine(tempFolder, ".svn")))
                workingCopy = true;

            using (SvnClient client = new SvnClient())
            {
                client.LoadConfiguration("path");
                client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
                
                if (!workingCopy)
                {
                    if (Directory.Exists(Path.Combine(TempFolderPath(string.Empty), ".svn")))
                    {
                        SvnUriTarget target = new SvnUriTarget(projectUrl);

                        if (!Directory.Exists(tempFolder))
                            Directory.CreateDirectory(tempFolder);

                        Collection<SvnInfoEventArgs> info;
                        bool res = client.GetInfo(target, new SvnInfoArgs { ThrowOnError = false }, out info);
                        if(res)
                            client.CheckOut(target, tempFolder);
                    }
                    else
                    {
                        SvnUriTarget target = new SvnUriTarget(Url);
                        Collection<SvnInfoEventArgs> info;
                        bool res = client.GetInfo(target, new SvnInfoArgs { ThrowOnError = false }, out info);
                        
                        if (res)
                            client.CheckOut(target, TempFolderPath(string.Empty));
                    }
                }
                else
                    client.Update(tempFolder);
            }
        }
        


        private void Commit(string alias, string svnFolder, string revFolder)
        {
            var reviFiles = System.IO.Directory.GetFiles(revFolder, "*.*", System.IO.SearchOption.AllDirectories);
            var svnFiles = System.IO.Directory.GetFiles(svnFolder, "*.*", System.IO.SearchOption.AllDirectories);

            Dictionary<string, string> reviFileIndex = new Dictionary<string, string>();
            Dictionary<string, string> svnFileIndex = new Dictionary<string, string>();

            string url = Url.TrimEnd('/') + "/" + alias;
            string tempFolder = TempFolderPath(alias);
            bool workingCopy = false;


            if (Directory.Exists(Path.Combine(tempFolder, ".svn")))
                workingCopy = true;
            else if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);


            foreach (string s in reviFiles)
            {
                DirectoryInfo parent = Directory.GetParent(s);
                if(!parent.Name.StartsWith("."))
                    reviFileIndex.Add(s.Substring(revFolder.Length).Trim('\\'), s);
            }


            foreach (string s in svnFiles)
            {
               if(!s.ToLower().Contains(".svn"))
                    svnFileIndex.Add(s.Substring(svnFolder.Length).Trim('\\'), s);
            }

            using (SvnClient client = new SvnClient())
            {
                client.LoadConfiguration("path");
                client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);

                if (!workingCopy)
                    client.Add(tempFolder);

                SvnUriTarget target = new SvnUriTarget(url);
                
                //remove not needed files
                foreach (var key in svnFileIndex.Keys)
                    if (!reviFileIndex.ContainsKey(key))
                        client.Delete(svnFileIndex[key]);


                //add missing files
                foreach (var file in reviFileIndex)
                {
                    string newPath = Path.Combine(svnFolder, file.Key);
                    bool add = false;

                    if (!File.Exists(newPath))
                    {
                        add = true;
                        ensureDirectory(Directory.GetParent(newPath).FullName, false);
                    }
                    
                    System.IO.File.Copy(file.Value, Path.Combine(svnFolder, file.Key), true);
                }

                SvnAddArgs saa = new SvnAddArgs();
                saa.Force = true;
                saa.Depth = SvnDepth.Infinity;


                foreach (var dir in Directory.GetDirectories(svnFolder))
                {
                    if(!dir.Contains(".svn"))
                        client.Add(dir, saa);
                }

                SvnCommitArgs args = new SvnCommitArgs();
                args.LogMessage = "Comitted from Courier 2";
                args.ThrowOnError = true;
                args.ThrowOnCancel = true;

                client.Commit(tempFolder, args);
            }
        }


        public override void CommitRevision(Revision revision)
        {
            //string localPath = PackagingManager.Instance.OutputDirectory;
            string url = Url.TrimEnd('/') + "/" + revision.Directory;

            string alias = revision.Name;
            string reviPath = Context.Current.MapPath(Core.Settings.revisionsPath + "/" + revision.Directory);
            string svnPath = TempFolderPath(alias);

            //first we ensure the directory is updated to the latest version by checking it out of source control
            checkout(alias);

            //then we commit the changes
            Commit(alias, svnPath, reviPath);
        }


        private void ensureDirectory(string directory, bool clear)
        {
            string path = directory;

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            else if (clear)
            {
                System.IO.Directory.Delete(path, true);
                System.IO.Directory.CreateDirectory(path);
            }
        }



        public override void LoadSettings(System.Xml.XmlNode settingsXml)
        {
            Url = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./url"));
            Login = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./login"));
            Password = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./password"));
            LocationAlias = Umbraco.Courier.Core.Helpers.IO.SanitizeFileName(settingsXml.Attributes["alias"].Value);
        }




    }
}