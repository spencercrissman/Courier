using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ExtensionMethods;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.Core.Services;
using Umbraco.Courier.Core.Storage;
using Umbraco.Courier.Core.ProviderModel;
using System.Web.Security;


using umbraco;
using Umbraco.Courier.RepositoryProviders.WebserviceProvider.Security;
using Umbraco.Courier.Core.Diagnostics.Logging;
using System.IO;
using Umbraco.Courier.RepositoryProviders.WebserviceProvider.Compression;


namespace Umbraco.Courier.RepositoryProviders.Webservices
{
  /// <summary>
  /// Summary description for Repository
  /// </summary>
  [WebService(Namespace = "http://courier.umbraco.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
  // [System.Web.Script.Services.ScriptService]
  public class Repository : System.Web.Services.WebService {

    #region Methods (8) 

		// Public Methods (6) 
     private void AuthorizeClient(string username, string password) {

       string _user = Umbraco.Courier.Core.Helpers.Encryption.Decrypt(username);
       string _pass = Umbraco.Courier.Core.Helpers.Encryption.Decrypt(password);

       Authentication.AuthorizeClient(_user, _pass, true);
     }


    [WebMethod]
    public string CommitRevision(Umbraco.Courier.Core.Revision revision, string username, string password) {
      //Auth login and IP
      AuthorizeClient(username, password);
              
      RevisionStorage revstorage = new RevisionStorage();
      revstorage.Save(revision);
      revstorage.Dispose();

      return "woo";
    }

    [WebMethod]
    public string CommitRevisionitem(String revision, Core.RevisionItem revisionItem, string username, string password) {
        //Auth login and IP
        AuthorizeClient(username, password);


        RevisionStorage revstorage = new RevisionStorage();
        revstorage.SaveRevisionItem(revisionItem, revision);
        revstorage.Dispose();
        
        return "woo";
    }


    [WebMethod]
    public string CommitResourceitem(String revision, Core.ResourceItem resourceItem, string username, string password) {
        //Auth login and IP
        AuthorizeClient(username, password);
        
        RevisionStorage revstorage = new RevisionStorage();
        revstorage.SaveResourceItem(resourceItem, revision);
        revstorage.Dispose();


        return "woo";
    }
      

    [WebMethod]
    public ItemConflictCollection CompareRevision(string directory, string username, string password) {

        /*
      //Auth login and IP
     AuthorizeClient(username, password);

      var engine = new Core.ExtractionManager();
      engine.Load(directory);
      engine.CompareRevisonWithServer(true);
      engine.Unload();
      engine.Dispose();

      return engine.RevisionConflicts;*/

        return new ItemConflictCollection();
    }


    [WebMethod]
    public void ExtractRevision(string directory, string username, string password) {

        /*
      //Auth login and IP
      AuthorizeClient(username, password);

      var engine = new Core.ExtractionManager();
      engine.Load(directory);
      engine.ExtractAll(true, true);
      engine.Unload();
      engine.Dispose();
        */

    }






    [WebMethod]
    public void OpenSession(string sessionKey, string username, string password) {
        //Auth login and IP
        AuthorizeClient(username, password);
        
        RevisionLog.StartNewLog("__tempremoteLog", RevisionLogType.Remote);

        LocalRepo.SessionKey = sessionKey;
        LocalRepo.OpenSession(sessionKey);
    }


    [WebMethod]
    public void CloseSession(string sessionKey, string username, string password)
    {
        //Auth login and IP
        AuthorizeClient(username, password);

        LocalRepo.SessionKey = sessionKey;
        LocalRepo.CloseSession(sessionKey);

        RevisionLog.CloseCurrentLog(Core.Context.Current.MapPath("/app_data/courier/temp_log.temp"), false);
    }


    [WebMethod]
    public void Commit(string sessionKey, string username, string password) {
        //Auth login and IP
        AuthorizeClient(username, password);

        LocalRepo.SessionKey = sessionKey;
        LocalRepo.Commit(sessionKey);
    }


    [WebMethod]
    public void Rollback(string sessionKey, string username, string password)
    {
        //Auth login and IP
        AuthorizeClient(username, password);

        LocalRepo.SessionKey = sessionKey;
        LocalRepo.Rollback(sessionKey);
    }



    [WebMethod]
    public string[] GetAvailableRevisions(string username, string password) {

      //Auth login and IP
      AuthorizeClient(username, password);

      RevisionStorage revstorage = new RevisionStorage();
      List<string> revs = revstorage.GetAllLocalRevisions();
        
      revstorage.Dispose();

      return revs.ToArray();
    }

    [WebMethod]
    public Umbraco.Courier.Core.Revision GetRevision(string name, string username, string password) {
      
      //Auth login and IP
      AuthorizeClient(username, password);

        /*
      RevisionStorage revstorage = new RevisionStorage();
      var rev = revstorage.GetFromDirectory(name, name);
      revstorage.Dispose();
        */


      return LocalRepo.GetRevision(name);
    }

    [WebMethod]
    public List<ExceptionProxy> ValidateRevision(string directory, string username, string password) {
      //Auth login and IP
      AuthorizeClient(username, password);

      RevisionStorage revstorage = new RevisionStorage();
      var rev = revstorage.GetFromDirectory(directory, directory);
      revstorage.Dispose();

      return new List<ExceptionProxy>();

//      return createProxy( rev.Validate() );
    }


    [WebMethod]
    public bool Exists(ItemIdentifier itemId, string user, string pass)
    {
        AuthorizeClient(user, pass);
        return LocalRepo.Exists(itemId);

    }
    
    [WebMethod]
    public List<ItemConflict> Compare(byte[] item, ItemIdentifier itemId, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);
        
        var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
        var i = p.Deserialize(itemId, item);

        return LocalRepo.Compare(i);
    }


    [WebMethod]
    public RepositoryActionResponse Extract(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
        var i = p.Deserialize(itemId, item);

        LocalRepo.SessionKey = sessionKey;
        var status = LocalRepo.ExtractItem(i, overwrite);

        return status;
    }


    [WebMethod]
    public RepositoryActionResponse PostProcess(string sessionKey, byte[] item, ItemIdentifier itemId, bool overwrite, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
        var i = p.Deserialize(itemId, item);

        LocalRepo.SessionKey = sessionKey;
        return LocalRepo.PostProcess(i,overwrite);
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
        rt.Resource.ResourceAsBase64 = string.Empty;

        if (!Core.Settings.disableBase64Encoding)
            rt.Resource.ResourceContents = Convert.FromBase64String(resource.ResourceAsBase64);

        if (!Core.Settings.disableZip)
            rt.Resource.ResourceContents = Compression.Decompress(rt.Resource.ResourceContents);

        LocalRepo.SessionKey = sessionKey;
        return LocalRepo.TransferResource(rt);
    }


    [WebMethod]
    public bool TransferResources(string sessionKey, ResourceTransfer[] resources, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        foreach (var rt in resources)
        {

            if (!Core.Settings.disableBase64Encoding && !string.IsNullOrEmpty(rt.Resource.ResourceAsBase64))
            {
                rt.Resource.ResourceContents = Convert.FromBase64String(rt.Resource.ResourceAsBase64);
                rt.Resource.ResourceAsBase64 = string.Empty;
            }

            if (!Core.Settings.disableZip)
                rt.Resource.ResourceContents = Compression.Decompress(rt.Resource.ResourceContents);
        }

        LocalRepo.SessionKey = sessionKey;
        return LocalRepo.TransferResources(resources);
    }



    [WebMethod]
    public void Execute(string sessionKey, string EventAlias, ItemIdentifier itemId, SerializableDictionary<string, string> parameters, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        LocalRepo.SessionKey = sessionKey;
        LocalRepo.ExecuteEvent(EventAlias, itemId, parameters);
    }


    [WebMethod]
    public void ExecuteQueue(string sessionKey, string queueAlias, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);
        
        LocalRepo.SessionKey = sessionKey;
        LocalRepo.ExecuteQueue(queueAlias);
    }

    [WebMethod]
    public string Package(string sessionKey, ItemIdentifier itemId, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        LocalRepo.SessionKey = sessionKey;
        var item = LocalRepo.Package(itemId);

        byte[] item_array = new byte[0];

        if (item != null)
        {
            foreach (var r in item.Resources)
            {
                r.ResourceContents = null;
            }

            item_array = Core.Serialization.Serializer.Serialize(item, item.GetType());
        }

        return Convert.ToBase64String(item_array);
    }

    [WebMethod]
    public SystemItem[] GetChildren(ItemIdentifier itemId, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);
        return LocalRepo.GetChildren(itemId);
    }

    [WebMethod]
    public string GetResourceContents(string sessionKey, ItemIdentifier itemId, string itemTypeName, Resource resource, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        LocalRepo.SessionKey = sessionKey;
        Type itemType = Type.GetType(itemTypeName);

        byte[] resourceContents = LocalRepo.GetResourceContents(itemId, itemType, resource, "");

        if(resourceContents == null || resourceContents.Length <= 0)
            return "";
            
        return Convert.ToBase64String(resourceContents);
    }


    [WebMethod]
    public string GetResourceHash(string sessionKey, Resource resource, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        LocalRepo.SessionKey = sessionKey;
        return LocalRepo.Hash(resource);
    }


    [WebMethod]
    public string GetItemHash(string sessionKey, ItemIdentifier itemId, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        LocalRepo.SessionKey = sessionKey;
        
        return LocalRepo.Hash(itemId);
    }


    [WebMethod]
    public string SaveItem(ItemIdentifier itemId, string itemAsBase64, string providerDirectory, string revisionAlias, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);
        
        byte[] itemBytes = Convert.FromBase64String(itemAsBase64);

        var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
        var i = p.Deserialize(itemId, itemBytes);


        return LocalRepo.SaveItem(i, providerDirectory, revisionAlias);
    }

    [WebMethod]
    public string Clear(string revisionAlias, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);
        LocalRepo.Clear(revisionAlias);
        return "";
    }

      /*
    [WebMethod]
    public string SaveResourceContents(ItemIdentifier itemId, string itemTypeName, Resource resource, string revisionAlias, string user, string pass)
    {
        //Auth login and IP
        AuthorizeClient(user, pass);

        Type itemType = Type.GetType(itemTypeName);
        if (!Core.Settings.disableBase64Encoding)
        {
            resource.ResourceContents = Convert.FromBase64String(resource.ResourceAsBase64);
            resource.ResourceAsBase64 = string.Empty;
        }

        string path = LocalRepo.SaveResourceContents(itemId, itemType, resource, revisionAlias);
        return path;
    }*/



    // Private Methods (2) 
    private List<ExceptionProxy> createProxy(List<Exception> list){
      List<ExceptionProxy> retval = new List<ExceptionProxy>();

      foreach (var e in list) {
        ExceptionProxy ep = new ExceptionProxy();
        ep.HelpLink = e.HelpLink;
        ep.Message = e.Message;
        ep.Source = e.Source;
        ep.StackTrace = e.StackTrace;
        retval.Add(ep);
      }

      return retval;
    }

    private SerializableDictionary<Item, List<ItemConflict>> createProxy(Dictionary<Item, List<ItemConflict>> list){
      SerializableDictionary<Item, List<ItemConflict>> retval = new SerializableDictionary<Item,List<ItemConflict>>();
      foreach (var v in list) {
        retval.Add(v.Key, v.Value);
      }
      return retval;
    }

    private static Local m_local = null;
    private static Local LocalRepo {
        get {
            if (m_local == null)
                m_local = new Local();

            return m_local;
            
        }
    }

		#endregion Methods 
  }


  public class RepositoryActionStatus {
      public string LastException { get; set; }
      public ItemStatus ItemStatus { get; set; }
  }
  [Serializable]
  public class ExceptionProxy {
		#region Properties (4) 

    public string HelpLink { get; set; }

    public string Message { get; set; }

    public string Source { get; set; }

    public string StackTrace { get; set; }

		#endregion Properties 
  }


}
