using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using SharpSvn;
using System.Net;
using System.Collections.ObjectModel;
using Umbraco.Courier.Core.Storage;

namespace Umbraco.Courier.RepositoryProviders {
  public class SubversionRepository : RepositoryProvider {

    public SubversionRepository() {

      this.Name = "Subversion Repository";
      this.Description = "Uses subversion as storage of items to allow connection to VS, through ankh.svn";
      this.Id = ProviderIdCollection.SvnProviderId;

    }

    public string Url { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    


    public override string[] GetAvailableRevisions()
    {
      string url = Url.TrimEnd('/');

      using (SvnClient client = new SvnClient())
      {
          client.LoadConfiguration("path");
          client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
          SvnTarget folderTarget = SvnTarget.FromString(url);

          List<String> filesFound = new List<String>();
          Collection<SvnListEventArgs> listResults;

          if (client.GetList(folderTarget, out listResults))
          {
              foreach (SvnListEventArgs item in listResults)
                  if (item.Entry.NodeKind == SvnNodeKind.Directory && !string.IsNullOrEmpty(item.Name))
                      filesFound.Add(item.Name);

              return filesFound.ToArray();
          }
      }
      
     return new string[0];
    }

    public override Revision GetRevision(string alias)
    {
        string url = Url.TrimEnd('/');
        string tempFolder = Context.Current.MapPath(Core.Settings.rootPath + "/svnRepoTemp/");

        if (System.IO.Directory.Exists(tempFolder))
            System.IO.Directory.Delete(tempFolder);

        using (SvnClient client = new SvnClient())
        {
            client.LoadConfiguration("path");
            client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
            SvnTarget folderTarget = SvnTarget.FromString(url);
            client.Export(folderTarget, tempFolder);
            
            RevisionStorage rs = new RevisionStorage();
            var r = rs.GetFromDirectory(alias, tempFolder + alias);

            rs.Save(r, alias);
                        
            rs.Dispose();

            return r;
        }

        return null;
    }

    public override void CommitRevision(Revision revision) {

      string localPath = PackagingManager.Instance.OutputDirectory;
      string url = Url.TrimEnd('/') + "/" + revision.Directory;

      using (SvnClient client = new SvnClient())
      {
          client.LoadConfiguration("path");
          client.Authentication.DefaultCredentials = new NetworkCredential(Login, Password);
          SvnTarget folderTarget = SvnTarget.FromString(url);
                    
          //client.Export(folderTarget, tempFolder);
      }
    }


    public override void LoadSettings(System.Xml.XmlNode settingsXml) {
      Url = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./url"));
      Login = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./login"));
      Password = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./password"));
    }


  }
}