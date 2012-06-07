using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.RepositoryProviders.Helpers
{
    public class LocalIO
    {
        public static string RevisionsFolder(string alias)
        {
            var root = RootFolder(alias);
                        
            EnsureRevisionFolders(root);
            return Path.Combine(root, Core.Settings.RevisionFolderName);
        }

        public static string RootFolder(string alias)
        {
            var root = Core.Context.Current.MapPath(Core.Settings.revisionsPath) + Core.Helpers.IO.DirSepChar + alias;
            return root;            
        }

        public static string ResourcesFolder(string alias)
        {
            var root = System.IO.Path.Combine(Core.Context.Current.MapPath(Core.Settings.revisionsPath), alias);
            EnsureRevisionFolders(root);
            return Path.Combine(root, Core.Settings.ResourcesFolderName);
        }


        public static string ItemFilePath(Item item, string providerDirectory, string revisionAlias)
        {
            var root = RevisionsFolder(revisionAlias);
            var courierFileName = Umbraco.Courier.Core.Helpers.IO.SanitizeFileName(item.CourierFileName);
            
            return Path.Combine(root, providerDirectory, courierFileName + "." + Core.Settings.fileExt);
        }

        public static string ResourceFilePath(Resource resource, string revisionAlias)
        {
            var root = ResourcesFolder(revisionAlias);
            var path = (root + Core.Helpers.IO.DirSepChar + resource.TemporaryStoragePath.Replace("//", "/").Replace('/', '\\').TrimStart('\\'));
            
            return path;
        }
        
        private static void EnsureRevisionFolders(string path)
        {
            //root
            CreateFolder(path);

            //revisions
            CreateFolder( Path.Combine(path, Core.Settings.RevisionFolderName));

            //resources
            CreateFolder( Path.Combine(path, Core.Settings.ResourcesFolderName));

            //settings
            CreateFolder( Path.Combine(path, Core.Settings.SettingsFolderName));
        }
                
        private static void CreateFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }

        static object SpinLock = new object();
        public static void SaveFile(string path, byte[] contents)
        {
            lock (SpinLock)
            {
                string file  = "";
                string folder = "";

                try
                {
                    file = Core.Context.Current.MapPath(path);
                    folder = new FileInfo(file).Directory.FullName;

                    CreateFolder(folder.TrimEnd('\\'));

                    if (contents == null || contents.Length == 0)
                        System.IO.File.Create(path).Close();
                    else
                        System.IO.File.WriteAllBytes(path, contents);
                }
                catch (Exception ex)
                {
                    Logging._Error("Tried saving " + path + " to: " + Environment.NewLine + file + Environment.NewLine + "Folder: " + folder + Environment.NewLine + ex.ToString());
                }
            }
        }

    }
}