using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Storage;



namespace Umbraco.Courier.RepositoryProviders
{
    public class NetworkShareProvider : RepositoryProvider
    {
        #region Constructors (1)

        public NetworkShareProvider()
        {
            this.Name = "Networkshare";
            this.Description = "Exposes a revision repository in a network share";
            this.Id = ProviderIdCollection.NetworkShareRepositoryProviderGuid;
        }

        #endregion Constructors

        #region Properties (1)

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

        #endregion Properties

        #region Methods (4)

        // Public Methods (4) 

        public override void CommitRevision(Revision revision)
        {
            string path = System.IO.Path.Combine(Path, revision.Name);

            RevisionStorage revstorage = new RevisionStorage();
            revstorage.Save(revision, path);
            revstorage.Dispose();
        }

        public override string[] GetAvailableRevisions()
        {
            List<string> retval = new List<string>();
            foreach (string dir in System.IO.Directory.GetDirectories(Path))
            {
                if (System.IO.Directory.GetFiles(dir, "*." + Core.Settings.fileExt, System.IO.SearchOption.AllDirectories).Count() > 0)
                    retval.Add(new System.IO.DirectoryInfo(dir).Name);
            }
            return retval.ToArray();
        }

        public override Revision GetRevision(string alias)
        {
            string path = System.IO.Path.Combine(Path, alias);

            RevisionStorage revstorage = new RevisionStorage();
            var rev = revstorage.GetFromDirectory(alias, path);
            revstorage.Dispose();

            return rev;
        }

        public override void LoadSettings(System.Xml.XmlNode settingsXml)
        {
            Path = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode(".//path"));
        }

        #endregion Methods
    }
}