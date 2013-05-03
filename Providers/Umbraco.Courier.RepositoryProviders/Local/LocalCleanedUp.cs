using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.Core.Interfaces;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Core.Storage;

namespace Umbraco.Courier.RepositoryProviders
{
    public class LocalCleanedUp : RepositoryProvider, IExtractionTarget, IPackagingTarget
    {
        public LocalCleanedUp()
        {
            this.Name = "Default local storage";
            this.Id = ProviderIdCollection.LocalProviderWithSettingsId;
            this.Description = "Used to perform extraction and packaging on the local instance";
        }
        
        public string RootPath { get; set; }
        public string DatabaseConnectionString { get; set; }

        //this provider works as both local and remotely, as it can fallback to remote services
        //in case it doesnt run in the same app pool
        public bool Remote { get; set; }
        
        public string RemoteUrl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        
        private ItemCrudProvider _database = null;
        private ItemCrudProvider Database
        {
            get
            {
                if(_database == null)
                {
                    _database = ItemPersistenceProviderCollection.Instance.GetProvider( Guid.Parse(Core.Settings.standardDatabasePersistenceProvider));
                    _database.ConnectionString = DatabaseConnectionString;
                }

                return _database;
            }
        }


        private ExecutionContext _context = null;
        private ExecutionContext ExecutionContext
        {
            get
            {
                if (_context == null)
                {
                    _context = new ExecutionContext();
                    if(!string.IsNullOrEmpty(RootPath))
                         _context.CurrentFullPath = RootPath;
                }
                return _context;
            }
        }



        /* LOAD CONNECTIONSTRING AND ROOT DIR */
        public override void LoadSettings(System.Xml.XmlNode settingsXml)
        {
            if (settingsXml != null)
            {
                RootPath = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./path"));

                if (settingsXml.SelectSingleNode("./connectionstring") != null)
                    DatabaseConnectionString = Umbraco.Courier.Core.Helpers.Xml.GetNodeValue(settingsXml.SelectSingleNode("./DatabaseConnectionString"));
            }
        }
        


        /* PACKAGING */
        public Item Package(ItemIdentifier itemId)
        {
            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId, Database, ExecutionContext);
            Item it = Core.Cache.ItemCacheManager.Instance.GetItem(itemId, provider);
            
            if (it == null)
            {
                it = provider.Package(itemId);
                if (it != null)
                    Core.Cache.ItemCacheManager.Instance.StoreItemAsync(it, provider);
            }


            if (it == null)
                return null;
            // throw new Exception(String.Format("Failed to package {0} with provider {1}", itemId.Id, provider.Name));

            //this only for internal logging of the queue on remote contexts
            provider.ExecutionContext.CurrentPackaging.Queue.Add(it.ItemId);
            return it;
        }
        public byte[] GetResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revisionAlias)
        {
            if (revisionAlias != null)
            {
                
                //check if the resource has already been packaged before
                resource.ResourceContents = resource.ToByteArray(Core.Settings.revisionsPath + "/" + revisionAlias + "/" +
                                                                    Core.Settings.ResourcesFolderName);
                if (resource.ResourceContents != null)
                    return resource.ResourceContents;
            }


            ItemProvider provider = null;
            if (!string.IsNullOrEmpty(resource.ExtractToPath) && itemType != null)
            {
                provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId, Database, ExecutionContext);
                ResolutionManager.Instance.PackagingResource(itemId, itemType, resource, provider);
            }

            resource.ResourceContents = resource.ToByteArray("~/");
        
            //if (!string.IsNullOrEmpty(resource.ExtractToPath) && itemType != null)
            //    ResolutionManager.Instance.PackagedResource(itemId, itemType, resource, provider);

            return resource.ResourceContents;
        }


        /* Persistence */
        public string SaveItem(Item item, string providerDirectory, string revisionAlias)
        {
            string itemPath = Helpers.LocalIO.ItemFilePath(item, providerDirectory, revisionAlias);

            //serialization of revision
            //ADDED for 2.1, if stripResourcesFromCourierFiles is true
            if (Core.Settings.stripResourcesFromCourierFiles)
            {
                foreach (var r in item.Resources)
                    r.ResourceContents = null;
            }

            Helpers.LocalIO.SaveFile(itemPath, Umbraco.Courier.Core.Serialization.Serializer.Serialize(item, item.GetType()));
            return itemPath;
        }
        public string SaveResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revisionAlias)
        {
            throw new NotImplementedException();

            var relPath = resource.ExtractToPath;
            var fileContents = resource.ResourceContents;
        }


        /* FETCHING SYSTEM IDs */
        public SystemItem[] GetChildren(ItemIdentifier itemID)
        {
            throw new NotImplementedException();
        }
        public SystemItem[] GetRootItems(Guid providerID)
        {
            throw new NotImplementedException();
        }
        public bool Exists(ItemIdentifier itemId)
        {
            throw new NotImplementedException();
        }

        

        /* LOADING A PREPACKAGED REVISION */
        public override Revision GetRevision(string alias)
        {
            string _folder = "";
            string _name = alias;

            //if alias is actually an absolute path
            if (System.IO.Directory.Exists(alias))
            {
                _folder = alias;
                _name = _folder.TrimEnd(IO.DirSepChar).Substring(_folder.LastIndexOf(IO.DirSepChar)).Trim(IO.DirSepChar);
            }
            else if(alias.Contains("/"))
            {
                //If alias is actually a relative path
                _folder = ExecutionContext.MapPath(alias);
            }else
            {
                //if just an alias
                _folder = ExecutionContext.MapPath(Core.Settings.revisionsPath + "/" + alias);
            }

            RevisionStorage revstorage = new RevisionStorage();
            var revi = revstorage.GetFromDirectory(_name, _folder, true);
            revstorage.Dispose();

            return revi;
        }


        /*
         * DELETE A REVISION WITH A CERTAIN ALIAS (not used)
         */ 
        public void Clear(string revisionAlias)
        {
            try
            {
                var root = Helpers.LocalIO.RootFolder(revisionAlias);

                //if (Directory.Exists(root))
                // DeleteFolderRecursive(new DirectoryInfo(root));

            }
            catch (Exception ex)
            {
                Logging._Error(ex.ToString());
            }
        }


        /* HASHING */
        public string Hash(ItemIdentifier itemId)
        {
            Item i = Package(itemId);
            if (i == null)
                return string.Empty;
            else{
                return i.GetHash();
            }
        }
        public string Hash(Resource resource)
        {
            return IO.CheckSum(ExecutionContext.MapPath(resource.ExtractToPath));
        }




        /* EXTRACTION */
        public RepositoryActionResponse ExtractItem(Item item, bool overWrite)
        {
            RepositoryActionResponse result = new RepositoryActionResponse();
            result.SetItemMeta(item);

            if (overWrite || (!overWrite && !Exists(item.ItemId)))
            {
                var p = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(item.ItemId.ProviderId, Database, ExecutionContext);
                Core.Cache.ItemCacheManager.Instance.ClearItem(item.ItemId, p);
                var returnedItem = p.Extract(item);
                result.ItemStatus = returnedItem.Status;
                
                if (result.ItemStatus != ItemStatus.Error && item.PostProcess)
                    result.ItemStatus = ItemStatus.NeedPostProcessing;
                
                return result;
            }

            result.ItemStatus = ItemStatus.Cancelled;
            return result;
        }


        public RepositoryActionResponse PostProcess(Item item, bool overWrite)
        {
            RepositoryActionResponse result = new RepositoryActionResponse();
            result.SetItemMeta(item);

            if (overWrite || (!overWrite && !Exists(item.ItemId)))
            {
                var p = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(item.ItemId.ProviderId, Database, ExecutionContext);

                //handle resolutions before PostProc
                // ResolutionManager.Instance.PostProcessingItem(item);

                //Perform the post-processing
                result.ItemStatus = p.PostProcess(item).Status;

                //handle resolutions after PostProc
                // ResolutionManager.Instance.PostProcessedItem(item);

                return result;
            }


            result.ItemStatus = ItemStatus.Cancelled;
            return result;
        }


        /* TRANSFERING ITEMS - used with extraction */
        public bool TransferResource(ResourceTransfer resource)
        {
            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(resource.ItemID.ProviderId, Database, ExecutionContext);
            ResolutionManager.Instance.ExtractingResource(resource.ItemID, resource.GetItemType(), resource.Resource, provider);

            if (resource.Resource.ResourceContents != null)
            {
                resource.Resource.Extract(resource.Resource.ResourceContents, resource.OverWrite);
                var path = resource.Resource.ExtractToPath;
                var file = resource.Resource.ResourceContents;
                var outPut = "";

                ExecutionContext.TryStoreFile(path, file, out outPut);

                ResolutionManager.Instance.ExtractedResource(resource.ItemID, resource.GetItemType(), resource.Resource, provider);
            }
            return true;
        }
        public bool TransferResources(ResourceTransfer[] resources)
        {
            foreach (var resource in resources)
                TransferResource(resource);

            return true;
        }


        /* EVENTS */
        public void ExecuteEvent(string eventAlias, ItemIdentifier itemId, SerializableDictionary<string, string> parameters)
        {
            EventManager.Instance.ExecuteLocalEvent(eventAlias, itemId, parameters, ExecutionContext);
        }

        public void ExecuteQueue(string queueItems)
        {
            EventManager.Instance.ExecuteLocalQueue(this.SessionKey, queueItems, ExecutionContext);
        }


        public List<ItemConflict> Compare(Item item)
        {
            throw new NotImplementedException();
        }










        /* SESSIONS HANDLING */
        /****************************************
        Session management
        *****************************************/
        public void OpenSession(string key)
        {
            this.SessionKey = key;
            Logging._Debug("Opening session: " + key);

            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ExecutionContext;

            PersistenceManager.Default.OpenSession(key);
            PersistenceManager.Default.BeginTransaction(key);
        }

        public void CloseSession(string key)
        {
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ExecutionContext;
            PersistenceManager.Default.CloseSession(key);
            Logging._Debug("Closing session: " + key);

        }

        public void Commit(string key)
        {
            Logging._Debug("Committing session: " + key);
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ExecutionContext;
            PersistenceManager.Default.CommitTransaction(key);
        }

        public void Rollback(string key)
        {
            Logging._Debug("Rolling back session: " + key);
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ExecutionContext;
            PersistenceManager.Default.Rollback(key);
        }       

    }
}