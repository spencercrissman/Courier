using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Exceptions;
using Umbraco.Courier.Core.Interfaces;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Core.Helpers;
using System.IO;
using Umbraco.Courier.Core.Collections;
using Umbraco.Courier.Core.Storage;

namespace Umbraco.Courier.RepositoryProviders {
    public class Local : RepositoryProvider, IExtractionTarget, IPackagingTarget{

        public Local() {
            this.Name = "Default local storage";
            this.Id = ProviderIdCollection.LocalProviderId;
            this.Description = "Used to perform extraction and packaging on the local instance";
        }      
              
        //always connects successfully
        public override bool Connect(out string errorMsg) {
            errorMsg = string.Empty;
            return true;
        }
                
        public RepositoryActionResponse ExtractItem(Item item, bool overwrite) {

            RepositoryActionResponse result = new RepositoryActionResponse();
            result.SetItemMeta(item);
            
            var p = item.Provider;

                if (p == null)
                    p = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(item.ItemId.ProviderId);
            
                if (overwrite || (!overwrite && !Exists(item.ItemId)))
                {
                    Core.Cache.ItemCacheManager.Instance.ClearItem(item.ItemId, p);
                    p.ExecutionContext = ContextManager.Instance.Get(SessionKey);
                    
                    var returnedItem = p.Extract(item);
                    result.ItemStatus = returnedItem.Status;

                    if (result.ItemStatus != ItemStatus.Error && item.PostProcess)
                        result.ItemStatus = ItemStatus.NeedPostProcessing;

                    return result;
                }

                result.ItemStatus = ItemStatus.Cancelled;
                return result;
        }
        
        public RepositoryActionResponse PostProcess(Item item, bool overwrite)
        {
            RepositoryActionResponse result = new RepositoryActionResponse();
            result.SetItemMeta(item);

            var p = item.Provider;

            if (p == null)
                p = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(item.ItemId.ProviderId);

            if (overwrite || (!overwrite && !Exists(item.ItemId))) {
                //handle resolutions before PostProc
               // ResolutionManager.Instance.PostProcessingItem(item);

                //Perform the post-processing
                p.ExecutionContext = ContextManager.Instance.Get(SessionKey);
                result.ItemStatus = p.PostProcess(item).Status;

                //handle resolutions after PostProc
               // ResolutionManager.Instance.PostProcessedItem(item);

                return result;
            }

            result.ItemStatus = ItemStatus.Cancelled;
            return result;
        }
        
        /* In use, in packaging */
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
        
        public void Clear(string revisionAlias)
        {
            try
            {
                var root = Helpers.LocalIO.RootFolder(revisionAlias);

                //if (Directory.Exists(root))
                // DeleteFolderRecursive(new DirectoryInfo(root));

            }catch(Exception ex)
            {
                Logging._Error(ex.ToString());
            }
        }

        private static void DeleteFolderRecursive(DirectoryInfo baseDir)
        {
            baseDir.Attributes = FileAttributes.Normal;
            foreach (var childDir in baseDir.GetDirectories())
                DeleteFolderRecursive(childDir);

            foreach (var file in baseDir.GetFiles())
                file.IsReadOnly = false;

            baseDir.Delete(true);
        }

        public List<ItemConflict> Compare(Item item) {
            var itemId = item.ItemId;
            var p = ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
            ItemConflictCollection icc = p.Compare(item);
            return icc[itemId.ToString()].ToList();
        }
        
        /****************************************
        Session management
        *****************************************/
        public void OpenSession(string key)
        {
            this.SessionKey = key;  
            Logging._Debug("Opening session: " + key);

            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ContextManager.Instance.Get(key);
            PersistenceManager.Default.OpenSession(key);
            PersistenceManager.Default.BeginTransaction(key);
        }

        public void CloseSession(string key)
        {
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ContextManager.Instance.Get(key);
            PersistenceManager.Default.CloseSession(key);
            Logging._Debug("Closing session: " + key);

            //this.SessionKey = string.Empty;
        }

        public void Commit(string key)
        {
            Logging._Debug("Committing session: " + key);
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ContextManager.Instance.Get(key);
            PersistenceManager.Default.CommitTransaction(key);
        }

        public void Rollback(string key) {

            Logging._Debug("Rolling back session: " + key);
            PersistenceManager.Default.SessionKey = key;
            PersistenceManager.Default.ExecutionContext = ContextManager.Instance.Get(key);
            PersistenceManager.Default.Rollback(key);
        }       
        
        public override void LoadSettings(System.Xml.XmlNode settingsXml) {
           
        }
        
        public void ExecuteEvent(string eventAlias, ItemIdentifier itemId, SerializableDictionary<string, string> parameters) {
           EventManager.Instance.ExecuteLocalEvent(eventAlias, itemId, parameters, null);
        }
        
        public void ExecuteQueue(string queueItems)
        {
            EventManager.Instance.ExecuteLocalQueue(this.SessionKey, queueItems, null);
        }
        


        public Item Package(ItemIdentifier itemId)
        {
            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
            provider.ExecutionContext = establishPackagingContext(ContextManager.Instance.Get(SessionKey));

            Item it = null;

            if (Core.Settings.EnableCaching)
            {
                it = Core.Cache.ItemCacheManager.Instance.GetItem(itemId, provider);
                if (it == null)
                {
                    it = provider.Package(itemId);
                    if (it != null)
                    {
                        it.Dependencies.Sort((x, y) => x.ItemId.CompareTo(y.ItemId));
                        Core.Cache.ItemCacheManager.Instance.StoreItemAsync(it, provider);
                    }  
                }
            }
            else
            {
                it = provider.Package(itemId);
                
                if(it != null)
                    it.Dependencies.Sort((x, y) => x.ItemId.CompareTo(y.ItemId));
            }


            if (it == null)
                return null;

            

            //this only for internal logging of the queue on remote contexts
            provider.ExecutionContext.CurrentPackaging.Queue.Add(it.ItemId);
            return it;
        }
        
        private ExecutionContext establishPackagingContext(ExecutionContext context)
        {
            if (context.CurrentPackaging == null)
            {
                context.CurrentPackaging = new Core.Packaging.RevisionPackaging("dummy");
             //   context.CurrentPackaging.Queue = new ItemIdentifierCollection();
            }

            return context;
        }

        public SystemItem[] GetChildren(ItemIdentifier itemID)
        {
            SystemItem[] retval = null;

            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemID.ProviderId);
            if (string.IsNullOrEmpty(itemID.Id))
                retval = provider.AvailableSystemItems().ToArray();
            else
                retval = provider.AvailableSystemItems(itemID).ToArray();

            if (retval == null)
                retval = new SystemItem[0];

            return retval;
        }
        
        public SystemItem[] GetRootItems(Guid providerID)
        {
            return GetChildren(new ItemIdentifier(null, providerID));
        }
        
        public bool Exists(ItemIdentifier itemId)
        {
            if (string.IsNullOrEmpty(Hash(itemId)))
                return false;
            else
                return true;

        }    
        
        public string Hash(ItemIdentifier itemId)
        {
            var hash = string.Empty;

            Item i = Package(itemId);
            
            if (i != null)
            {
                hash = i.GetHash();
                foreach (Dependency dependency in i.Dependencies.Where(x => x.IsChild))
                {
                    Item childItem = Package(dependency.ItemId);
                    var childHash = childItem.GetHash();
                    hash += childHash;
                }
            }

            return hash;
        }

        public string Hash(Resource resource)
        {
            return IO.CheckSum(Context.Current.MapPath(resource.ExtractToPath));
        }
        
        public override Revision GetRevision(string alias)
        {
            string _folder = "";
            string _name = alias;

            if (System.IO.Directory.Exists(alias))
            {
                _folder = alias;
                _name = _folder.TrimEnd(IO.DirSepChar).Substring(_folder.LastIndexOf(IO.DirSepChar)).Trim(IO.DirSepChar);
            }
            else
            {
                _folder = Context.Current.MapPath(Core.Settings.revisionsPath + "/" + alias);
            }


            RevisionStorage revstorage = new RevisionStorage();
            var revi = revstorage.GetFromDirectory(_name, _folder);
            revstorage.Dispose();

            return revi;
        }

        public override void CommitRevision(Revision revision)
        {
            //remember to reset dir, so we don't save on the old location...
            revision.Directory = string.Empty;

            RevisionStorage revstorage = new RevisionStorage();
            revstorage.Save(revision);
            revstorage.Dispose();
        }

        /* IN USE  - packaging*/
        public byte[] GetResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revisionAlias)
        {
            if (revisionAlias != null)
            {
                //check if the resource has already been packaged before
                resource.ResourceContents =
                    resource.ToByteArray(Core.Settings.revisionsPath + "/" + revisionAlias + "/" +
                                         Core.Settings.ResourcesFolderName);
                if (resource.ResourceContents != null)
                    return resource.ResourceContents;
            }

            ItemProvider provider = null;
            if (!string.IsNullOrEmpty(resource.ExtractToPath) && itemType != null)
            {
                provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
                provider.ExecutionContext = establishPackagingContext(ContextManager.Instance.Get(SessionKey));
                ResolutionManager.Instance.PackagingResource(itemId, itemType, resource, provider);
            }

            resource.ResourceContents = resource.ToByteArray("~/");
            Logging.Add("nulls", (resource.ResourceContents == null).ToString() +  " < " +resource.ExtractToPath);

            //if (!string.IsNullOrEmpty(resource.ExtractToPath) && itemType != null)
            //    ResolutionManager.Instance.PackagedResource(itemId, itemType, resource, provider);

            Logging.Add("nulls", (resource.ResourceContents == null).ToString() + " > " + resource.ExtractToPath);

            return resource.ResourceContents;
        }

        /* IN USE - packaging */
        object SpinLock = new object();
        public string SaveResourceContents(ItemIdentifier itemId, Type itemType, Resource resource, string revisionAlias)
        {
            lock (SpinLock)
            {
                string path = Helpers.LocalIO.ResourceFilePath(resource, revisionAlias);
                Helpers.LocalIO.SaveFile(path, resource.ResourceContents);
                return path;
            }
        }


        /* IN USE */
        public bool TransferResource(ResourceTransfer resource)
        {
            var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(resource.ItemID.ProviderId);
            provider.ExecutionContext = establishPackagingContext(ContextManager.Instance.Get(SessionKey));

            ResolutionManager.Instance.ExtractingResource(resource.ItemID, resource.GetItemType(), resource.Resource, provider);

            if (resource.Resource.ResourceContents != null)
            {
                resource.Resource.Extract(resource.Resource.ResourceContents, resource.OverWrite);
                ResolutionManager.Instance.ExtractedResource(resource.ItemID, resource.GetItemType(), resource.Resource, provider);
            }
            return true;
        }

        /* IN USE */
        public bool TransferResources(ResourceTransfer[] resources)
        {
            foreach (var tr in resources)
            {
                var provider = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(tr.ItemID.ProviderId);
                provider.ExecutionContext = establishPackagingContext(ContextManager.Instance.Get(SessionKey));

                ResolutionManager.Instance.ExtractingResource(tr.ItemID, tr.GetItemType(), tr.Resource, provider);

                if (tr.Resource.ResourceContents != null)
                {
                    var sp = Core.Settings.Encoding.GetString(tr.Resource.ResourceContents);

                    tr.Resource.Extract(tr.Resource.ResourceContents, tr.OverWrite);
                    ResolutionManager.Instance.ExtractedResource(tr.ItemID, tr.GetItemType(), tr.Resource, provider);
                }
            }
            return true;
        }
    }
}