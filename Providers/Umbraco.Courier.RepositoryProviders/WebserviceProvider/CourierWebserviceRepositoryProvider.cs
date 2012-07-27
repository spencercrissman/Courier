using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.RepositoryProviders.WebServiceProvider;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.Core.Interfaces;
using System.Web.Security;
using umbraco;
using System.Security.Cryptography;
using System.Text;

using Umbraco.Courier.Core.Diagnostics.Logging;
using Umbraco.Courier.RepositoryProviders.WebserviceProvider.Compression;
using Umbraco.Courier.Core.ExtensionMethods;


namespace Umbraco.Courier.RepositoryProviders
{
    public class CourierWebserviceRepositoryProvider : RepositoryProvider, IExtractionTarget, IPackagingTarget
    {
        #region Constructors (1)

        public CourierWebserviceRepositoryProvider()
        {
            this.Name = "Courier webservice";
            this.Description = "Exposes a courier repository using the default Courier webservice";
            this.Id = ProviderIdCollection.CourierWebserviceRepositoryProviderGuid;
        }

        #endregion Constructors

        #region Properties (4)

        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordEncoding { get; set; }
        

        //Settings
        public string Url { get; set; }
        public int UserId { get; set; }

        
        #endregion Properties

        #region Methods (6)

        // Public Methods (4) 

        public override void CommitRevision(Revision revision)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);

            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);

            foreach (var i in revision.GetAllRevisionItems().Values)
            {
                repo.CommitRevisionitem(revision.Name, i, loginName, pass);

                RevisionFileEventArgs e = new RevisionFileEventArgs();
                e.Name = i.Name;                

                OnRevisionItemCommitted(e);
            }

            foreach (var i in revision.GetAllResources().Values)
            {
                repo.CommitResourceitem(revision.Name, i, loginName, pass);

                RevisionFileEventArgs e = new RevisionFileEventArgs();
                e.Name = i.Name;
                OnRevisionResourceCommitted(e);
            }

            foreach (var i in revision.GetAllVirtualResources().Values)
            {   
                repo.CommitResourceitem(revision.Name, i, loginName, pass);

                RevisionFileEventArgs e = new RevisionFileEventArgs();
                e.Name = i.Name;
                OnRevisionResourceCommitted(e);
            }
            repo.Dispose();
        }

        public override string[] GetAvailableRevisions()
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);

            string _url = getWSUrl(this.Url);

            //call asmx
            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            return repo.GetAvailableRevisions(loginName, pass);
        }

        public override Umbraco.Courier.Core.Revision GetRevision(string alias)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            Revision revi = repo.GetRevision(alias, loginName, pass);
            repo.Dispose();
            return revi;
        }

        public override void LoadSettings(System.Xml.XmlNode settingsXml)
        {
            if (settingsXml != null)
            {
                Url = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./url"));

                if (settingsXml.SelectSingleNode("./user") != null)
                    UserId = int.Parse(Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./user")));
                else
                {
                    Login = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./login"));
                    Password = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./password"));
                    PasswordEncoding = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./passwordEncoding"));

                    UserId = -1;
                }
            }
            
            if (Core.Licensing.InfralutionLicensing.IsTrial())
            {
                    string _u = Url.ToLower().Replace("http://", "").Replace("https://", "");

                    if (Context.Current.HasHttpContext && !Core.Licensing.InfralutionLicensing.IsLocalIpAddress(_u))
                       throw new Umbraco.Licensing.Exceptions.TrialLicenseException(string.Format(Core.Licensing.InfralutionLicensing.LICENSE_REPOERROR, _u) + "\n\n" + Core.Licensing.InfralutionLicensing.LICENSE_CONTACTINFO);
            }
            
        }
        // Private Methods (2) 

        private void getloginAndPass(int userId, ref string login, ref string pass)
        {
            //if we have a userID, we will use that...
            if (UserId >= 0)
            {
                umbraco.BusinessLogic.User u = new umbraco.BusinessLogic.User(UserId);

                //encrypt login and password
                login = Encryption.Encrypt(u.LoginName);
                pass = Encryption.Encrypt(u.GetPassword());
            }
            else
            {
                //we will fetch them from the set values
                login = Encryption.Encrypt(Login);
                pass = Encryption.Encrypt(encodePassWord(Password) );
            }
        }
        private string encodePassWord(string pass) {

          if (string.IsNullOrEmpty(PasswordEncoding))
            PasswordEncoding = "Hashed";
          
            string encodedPassword = pass;

            switch (PasswordEncoding) {
              case "Clear":
                break;
              case "Hashed":
                HMACSHA1 hash = new HMACSHA1();
                hash.Key = Encoding.Unicode.GetBytes(pass);
                encodedPassword =
                  Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(pass)));
                break;
              default:
                throw new ProviderException("Unsupported password format.");
            }
          
          return encodedPassword;
          
        }
        private string getWSUrl(string url)
        {
            string _url = url;
            //call asmx
            if (!_url.Contains(".asmx"))
                _url = _url.Trim('/') + "/" + Core.Settings.webservicesPath;

            return _url;
        }
        #endregion Methods


        public RepositoryActionResponse ExtractItem(Item item, bool overwrite)
        {   

            try {
                //encrypt login and password
                string loginName = "";
                string pass = "";
                getloginAndPass(UserId, ref loginName, ref pass);
                string _url = getWSUrl(this.Url);

                RepositoryWebservice repo = new RepositoryWebservice(_url);
                repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

                byte[] obj = Core.Serialization.Serializer.Serialize(item, item.GetType());

                RepositoryActionResponse result = repo.Extract(this.SessionKey, obj, item.ItemId, overwrite, loginName, pass);
                repo.Dispose();


                return result;
            } 
            catch (Exception ex)
            {

                RevisionLog.Instance.Error(item, this, RevisionLog.ExtractionManager, ex.ToString());

                RepositoryActionResponse response = new RepositoryActionResponse();
                response.LoadException(ex);
                response.ItemStatus = ItemStatus.Error;
                response.ItemId = item.ItemId;
                response.ItemName = item.Name;

                return response;
            }
        }

        public RepositoryActionResponse PostProcess(Item item, bool overwrite)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            byte[] obj = Core.Serialization.Serializer.Serialize(item, item.GetType());

            RepositoryActionResponse response = repo.PostProcess(this.SessionKey, obj, item.ItemId, overwrite, loginName, pass);
            repo.Dispose();
            return response;
        }

        public bool TransferResource(ResourceTransfer resource)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            var res = resource.Resource;

            if (!Core.Settings.disableZip)
                res.ResourceContents = Compression.Compress(res.ResourceContents);

            if (!Core.Settings.disableBase64Encoding)
            {   
                res.ResourceAsBase64 = Convert.ToBase64String(res.ResourceContents);
                res.ResourceContents = new byte[0];
            }            

            bool transfered = repo.TransferResource(this.SessionKey, resource.ItemID, resource.ItemType, res, resource.OverWrite, loginName, pass);
            
            if(transfered)
                RevisionLog.Instance.AddItemEntry(resource.ItemID, this.GetType(), "resources", resource.Resource.ExtractToPath + " transfered", LogItemEntryType.Success);
            else
                RevisionLog.Instance.AddItemEntry(resource.ItemID, this.GetType(), "resources", resource.Resource.ExtractToPath + " not transfered", LogItemEntryType.Error);
            
            
            repo.Dispose();
            return transfered;
        }
        
        public bool TransferResources(ResourceTransfer[] resources)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            foreach (var rt in resources)
            {
                if (!Core.Settings.disableZip)
                    rt.Resource.ResourceContents = Compression.Compress(rt.Resource.ResourceContents);


                if (!Core.Settings.disableBase64Encoding && rt.Resource.ResourceContents != null && rt.Resource.ResourceContents.Length > 0)
                {
                    rt.Resource.ResourceAsBase64 = Convert.ToBase64String(rt.Resource.ResourceContents);
                    rt.Resource.ResourceContents = new byte[0];
                }
            }

            Logging._Debug("transfer resources: session: " + this.SessionKey + " : " + resources.Count().ToString());

            bool transfered = repo.TransferResources(this.SessionKey, resources, loginName, pass);

            repo.Dispose();
            return transfered;
        }
        
        public List<ItemConflict> Compare(Item item)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            byte[] obj = Core.Serialization.Serializer.Serialize(item, item.GetType());

            var conflicts = repo.Compare(obj, item.ItemId, loginName, pass).ToList<ItemConflict>();

            repo.Dispose();
            return conflicts;
        }

        public void ExecuteEvent(string eventAlias, ItemIdentifier itemId, SerializableDictionary<string, string> parameters)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "EventManager", "Executing event" + eventAlias, LogItemEntryType.Information);
            
            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Execute(this.SessionKey, eventAlias, itemId, null, loginName, pass);
            repo.Dispose();
        }

        public void OpenSession(string sessionKey)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            this.SessionKey = sessionKey;

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session opening: " + sessionKey, LogItemEntryType.Information);
            

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.OpenSession(SessionKey, loginName, pass);
            repo.Dispose();
        }

        public void CloseSession(string sessionKey)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);
            
            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.CloseSession(sessionKey, loginName, pass);
            
            /*
            Resource r = new Resource();
            r.TemporaryStoragePath = "/app_data/courier/temp_log.temp";
            
            byte[] remoteLog = Convert.FromBase64String( repo.GetResourceContents(sessionKey, null, "", r, loginName, pass));
            repo.Dispose();
            
            string path = Context.Current.MapPath(Core.Settings.logsPath) + Core.Helpers.IO.DirSepChar + sessionKey + ".remotelog";

            System.IO.File.WriteAllBytes(path, remoteLog);
            */

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session closed", LogItemEntryType.Information);
        }

        public void Commit(string sessionKey)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Committing changes", LogItemEntryType.Information);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Commit(sessionKey, loginName, pass);
            repo.Dispose();
        }

        public void Rollback(string sessionKey)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);


            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Rolling back changes", LogItemEntryType.Information);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Rollback(sessionKey, loginName, pass);
            repo.Dispose();
        }

        public void ExecuteQueue(string queueAlias)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.ExecuteQueue(this.SessionKey, queueAlias, loginName, pass);
            repo.Dispose();
        }     

        public Item Package(ItemIdentifier itemId)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            //repo.Commit(loginName, pass);
            byte[] item = Convert.FromBase64String(repo.Package(this.SessionKey, itemId, loginName, pass));
            repo.Dispose();
            
            if (item == null || item.Length <= 0)
                return null;
            
            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
            Item it = provider.Deserialize(itemId, item);

            return it;
        }

        public SystemItem[] GetChildren(ItemIdentifier itemId)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            //repo.Commit(loginName, pass);

            var children = repo.GetChildren(itemId, loginName, pass);
            repo.Dispose();

            return children;
        }

        public SystemItem[] GetRootItems(Guid providerID)
        {
            return GetChildren(new ItemIdentifier(null, providerID));
        }

        public byte[] GetResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revision)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            string typeName = itemType.AssemblyQualifiedName;

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            byte[] resourceContent = Convert.FromBase64String( repo.GetResourceContents(this.SessionKey, itemId, typeName, resource, loginName, pass));
            repo.Dispose();

            return resourceContent;
        }

        public string SaveItem(Item item, string providerDirectory, string revisionAlias)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            string itemAsBase64 = Convert.ToBase64String( Umbraco.Courier.Core.Serialization.Serializer.Serialize(item, item.GetType()));
            
            RepositoryWebservice repo = new RepositoryWebservice(_url);

            string path = repo.SaveItem(item.ItemId, itemAsBase64, providerDirectory, revisionAlias, loginName, pass);

            //byte[] resourceContent = repo.GetResourceContents(this.SessionKey, itemId, typeName, resource, loginName, pass);
            repo.Dispose();

            return path;
        }

        public void Clear(string revisionAlias)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
           // repo.Clear(revisionAlias, loginName, pass);
            repo.Dispose();
        }

        public string SaveResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revisionAlias)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);
                     
   
            if (!Core.Settings.disableBase64Encoding)
            {   
                resource.ResourceAsBase64 = Convert.ToBase64String(resource.ResourceContents);
                resource.ResourceContents = new byte[0];
            }

            string typestr = string.Empty;
            if (itemType != null)
                typestr = itemType.ToString();

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            string path = repo.SaveResourceContents(itemId, typestr, resource, revisionAlias, loginName, pass);
            repo.Dispose();

            return path;
        }

        public bool Exists(ItemIdentifier itemId)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            bool exists = repo.Exists(itemId, loginName, pass);

            repo.Dispose();
            return exists;
        }

        public string Hash(ItemIdentifier itemId)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            string hash = string.Empty;
            try
            {
                hash = repo.GetItemHash(this.SessionKey, itemId, loginName, pass);
            }
            catch { }

            repo.Dispose();

            return hash;
        }

        public string Hash(Resource resource)
        {
            //encrypt login and password
            string loginName = "";
            string pass = "";
            getloginAndPass(UserId, ref loginName, ref pass);
            string _url = getWSUrl(this.Url);

            RepositoryWebservice repo = new RepositoryWebservice(_url);
            repo.Credentials = new System.Net.NetworkCredential(loginName, pass);

            //just a stub to pass to the service so we pass as little information as possible
            Resource r = new Resource();
            r.ExtractToPath = resource.ExtractToPath;
            r.TemporaryStoragePath = resource.TemporaryStoragePath;

            string hash = repo.GetResourceHash(this.SessionKey, r, loginName, pass); // repo.get .Exists(itemId, loginName, pass);
            repo.Dispose();

            return hash;
        }
    }
}