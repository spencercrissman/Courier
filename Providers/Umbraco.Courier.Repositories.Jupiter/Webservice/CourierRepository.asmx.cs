using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Umbraco.Courier.Repositories.Core;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;

namespace Umbraco.Courier.Repositories.Jupiter
{
    /// <summary>
    /// Summary description for CourierRepository
    /// </summary>
    [WebService(Namespace = "http://courier.umbraco.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CourierRepositoryASMX : System.Web.Services.WebService
    {

        [WebMethod]
        public void OpenSession(string sessionKey, string username, string password)
        {
            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";

            //Auth login and IP
            AuthorizeClient(username, password);

            LocalRepo.SessionKey = sessionKey;
            LocalRepo.OpenSession(sessionKey);
        }


        [WebMethod]
        public void CloseSession(string sessionKey, string username, string password)
        {
            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";

            //Auth login and IP
            AuthorizeClient(username, password);

            LocalRepo.SessionKey = sessionKey;
            LocalRepo.CloseSession(sessionKey);

            //RevisionLog.CloseCurrentLog(Core.Context.Current.MapPath("/app_data/courier/temp_log.temp"), false);
        }


        [WebMethod]
        public void Commit(string sessionKey, string username, string password)
        {
            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";

            //Auth login and IP
            AuthorizeClient(username, password);

            LocalRepo.SessionKey = sessionKey;
            LocalRepo.Commit(sessionKey);
        }


        [WebMethod]
        public void Rollback(string sessionKey, string username, string password)
        {
            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";

            //Auth login and IP
            AuthorizeClient(username, password);

            LocalRepo.SessionKey = sessionKey;
            LocalRepo.Rollback(sessionKey);
        }

        [WebMethod]
        public RepositoryActionResponse Extract(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass)
        {
            //Auth login and IP
            AuthorizeClient(user, pass);

            var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
            var i = p.Deserialize(itemId, item);

            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";

            LocalRepo.SessionKey = sessionKey;
            var status = LocalRepo.ExtractItem(i, overwrite);

            return status;
        }

        [WebMethod]
        public bool TransferResource(string sessionKey, ItemIdentifier itemId, string type, Resource resource, bool overwrite, string user, string pass)
        {
            //Auth login and IP
            AuthorizeClient(user, pass);

            ResourceTransfer rt = new ResourceTransfer();
            rt.ItemType = type;
            rt.OverWrite = overwrite;
            rt.ItemID = itemId;

            rt.Resource = resource;
            rt.Resource.ResourceContents = Convert.FromBase64String(resource.ResourceAsBase64);
            rt.Resource.ResourceAsBase64 = string.Empty;
            
            if (string.IsNullOrEmpty(sessionKey))
                sessionKey = "default";
           
            LocalRepo.SessionKey = sessionKey;
            return LocalRepo.TransferResource(rt);
        }





        private static Local m_local = null;
        private static Local LocalRepo
        {
            get
            {
                if (m_local == null)
                    m_local = new Local();

                return m_local;
            }
        }

        private void AuthorizeClient(string username, string password)
        {

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["fromExternalClient"]))
            {
                //if (Core.Licensing.InfralutionLicensing.IsLight())
               //     throw new Umbraco.Licensing.Exceptions.InvalidLicenseException(Core.Licensing.InfralutionLicensing.LICENSE_APIERROR + "\n\n" + Core.Licensing.InfralutionLicensing.LICENSE_CONTACTINFO);
            }

      //      string _user = Umbraco.Courier.Core.Helpers.Encryption.Decrypt(username);
      //      string _pass = Umbraco.Courier.Core.Helpers.Encryption.Decrypt(password);

            //Authentication.AuthorizeClient(_user, _pass, true);
        }
    }
}
