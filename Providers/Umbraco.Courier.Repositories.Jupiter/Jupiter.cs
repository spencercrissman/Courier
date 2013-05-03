using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Interfaces;
using Umbraco.Courier.Core.Diagnostics.Logging;

namespace Umbraco.Courier.Repositories.Jupiter
{
    public class JupiterRepository : RepositoryProvider, IExtractionTarget
    {

        public JupiterRepository()
        {
            this.Name = "Jupiter webservice";
            this.Description = "Exposes a courier repository using the default Courier webservice on jupiter";
            this.Id = Constants.ProviderId;
        }

        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordEncoding { get; set; }


        //Settings
        public string Url { get; set; }
        public int UserId { get; set; }



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
        }

        public bool Exists(ItemIdentifier itemId)
        {
            return false;
        }

        public string Hash(ItemIdentifier itemId)
        {
            return string.Empty;
        }

        public string Hash(Resource resourcePath)
        {
            return string.Empty;
        }

        public RepositoryActionResponse ExtractItem(Item item, bool overWrite)
        {
            byte[] obj = Umbraco.Courier.Core.Serialization.Serializer.Serialize(item, item.GetType());

            JupiterWebservice jws = new JupiterWebservice(Url);
            var reply = jws.Extract(this.SessionKey, obj, item.ItemId, true, "user", "pass");

            return reply;
        }

        public RepositoryActionResponse PostProcess(Item item, bool overWrite)
        {
            return new RepositoryActionResponse();
        }

        public bool TransferResource(ResourceTransfer resource)
        {
            JupiterWebservice jws = new JupiterWebservice(Url);

            resource.Resource.ResourceAsBase64 = Convert.ToBase64String(resource.Resource.ResourceContents);
            resource.Resource.ResourceContents = null;
            
            jws.TransferResource(this.SessionKey, resource.ItemID, resource.ItemType, resource.Resource, true, Login, Password);

            jws.Dispose();
            return true;
        }

        public bool TransferResources(ResourceTransfer[] resources)
        {
            foreach (var r in resources)
                TransferResource(r);

            return true;
        }

        public void ExecuteEvent(string eventAlias, ItemIdentifier itemId, SerializableDictionary<string, string> parameters)
        {
            
        }

        public void ExecuteQueue(string queue)
        {
            
        }

        public void OpenSession(string sessionKey)
        {
            JupiterWebservice jws = new JupiterWebservice(Url);

            this.SessionKey = sessionKey;
            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session opening: " + sessionKey, LogItemEntryType.Information);

            jws.OpenSession(sessionKey, "user", "pass");
            jws.Dispose();
        }

        public void CloseSession(string sessionKey)
        {
            JupiterWebservice jws = new JupiterWebservice(Url);

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session Closing: " + sessionKey, LogItemEntryType.Information);

            jws.CloseSession(sessionKey, "user", "pass");
            jws.Dispose();
        }

        public void Commit(string sessionKey)
        {
            JupiterWebservice jws = new JupiterWebservice(Url);

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session Committing: " + sessionKey, LogItemEntryType.Information);

            jws.Commit(sessionKey, "user", "pass");
            jws.Dispose();
        }

        public void Rollback(string sessionKey)
        {
            JupiterWebservice jws = new JupiterWebservice(Url);

            RevisionLog.Instance.AddRevisionEntry(this.GetType(), "ExtractRevisions", "Session rolling back: " + sessionKey, LogItemEntryType.Information);

            jws.Rollback(sessionKey, "user", "pass");
            jws.Dispose();
        }

        public List<ItemConflict> Compare(Item item)
        {
            return new List<ItemConflict>();
        }
    }
}