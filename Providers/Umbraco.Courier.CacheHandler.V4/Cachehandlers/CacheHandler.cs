using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.macro;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Interfaces;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.Cachehandler.V4
{
    public class CacheHandler : ApplicationBase
    {
        public CacheHandler()
        {
            if (Settings.EnableCaching)
            {
                Source = new Repository(Umbraco.Courier.Core.ProviderModel.RepositoryProviderCollection.Instance.Default);
                Source.Provider.SessionKey = Guid.NewGuid().ToString();

                Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);
                Document.AfterSave += new Document.SaveEventHandler(Document_AfterSave);
                Document.AfterUnPublish += Document_AfterUnPublish;
                content.AfterUpdateDocumentCache += content_AfterUpdateDocumentCache;

                Media.AfterSave += new Media.SaveEventHandler(Media_AfterSave);
                Template.AfterSave += new Template.SaveEventHandler(Template_AfterSave);
                DocumentType.AfterSave += new DocumentType.SaveEventHandler(DocumentType_AfterSave);
                Macro.AfterSave += new Macro.SaveEventHandler(Macro_AfterSave);
                StyleSheet.AfterSave += new StyleSheet.SaveEventHandler(StyleSheet_AfterSave);

                umbraco.cms.businesslogic.language.Language.AfterSave +=
                    new umbraco.cms.businesslogic.language.Language.SaveEventHandler(Language_AfterSave);
                umbraco.cms.businesslogic.Dictionary.DictionaryItem.Saving +=
                    new umbraco.cms.businesslogic.Dictionary.DictionaryItem.SaveEventHandler(DictionaryItem_Saving);

                Document.AfterDelete += new Document.DeleteEventHandler(Document_AfterDelete);
                Document.AfterMoveToTrash += new Document.MoveToTrashEventHandler(Document_AfterMoveToTrash);

                Media.AfterMoveToTrash += new Media.MoveToTrashEventHandler(Media_AfterMoveToTrash);
                Media.AfterDelete += new Media.DeleteEventHandler(Media_AfterDelete);

                Template.AfterDelete += new Template.DeleteEventHandler(Template_AfterDelete);
                DocumentType.AfterDelete += new DocumentType.DeleteEventHandler(DocumentType_AfterDelete);
                Macro.AfterDelete += new Macro.DeleteEventHandler(Macro_AfterDelete);

                StyleSheet.AfterDelete += new StyleSheet.DeleteEventHandler(StyleSheet_AfterDelete);

                umbraco.cms.businesslogic.language.Language.AfterDelete +=
                    new umbraco.cms.businesslogic.language.Language.DeleteEventHandler(Language_AfterDelete);
                umbraco.cms.businesslogic.Dictionary.DictionaryItem.Deleting +=
                    new umbraco.cms.businesslogic.Dictionary.DictionaryItem.DeleteEventHandler(DictionaryItem_Deleting);
            }
        }

        void content_AfterUpdateDocumentCache(Document sender, umbraco.cms.businesslogic.DocumentCacheEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            //ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);

            sendToCache(new[] { itemId });
        }

     
        void Document_AfterUnPublish(Document sender, umbraco.cms.businesslogic.UnPublishEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);

            sendToCache(new[] { itemId, propertyId });
        }


        void Language_AfterDelete(umbraco.cms.businesslogic.language.Language sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.CultureAlias, ItemProviders.ProviderIDCollection.languageItemProviderGuid);
            clearCache(itemId);
        }

        void DictionaryItem_Deleting(umbraco.cms.businesslogic.Dictionary.DictionaryItem sender, EventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.key, ItemProviders.ProviderIDCollection.dictionaryItemProviderGuid);
            clearCache(itemId);
        }
        
        void StyleSheet_AfterDelete(StyleSheet sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Text, ItemProviders.ProviderIDCollection.stylesheetItemProviderGuid);
            clearCache(itemId);
        }

        void Macro_AfterDelete(Macro sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.macroItemProviderGuid);
            clearCache(itemId);
        }

        void DocumentType_AfterDelete(DocumentType sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.documentTypeItemProviderGuid);
            clearCache(itemId);
        }

        void Template_AfterDelete(Template sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.templateItemProviderGuid);
            clearCache(itemId);
        }

        void Media_AfterDelete(Media sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediaItemProviderGuid);
            clearCache(itemId);

            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediapropertyDataItemProviderGuid);
            clearCache(propertyId);
        }

        void Media_AfterMoveToTrash(Media sender, umbraco.cms.businesslogic.MoveToTrashEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediaItemProviderGuid);
            clearCache(itemId);

            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediapropertyDataItemProviderGuid);
            clearCache(propertyId);
        }

        void Document_AfterMoveToTrash(Document sender, umbraco.cms.businesslogic.MoveToTrashEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);

            clearCache(itemId);
            clearCache(propertyId);
        }

        void Document_AfterDelete(Document sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);

            clearCache(itemId);
            clearCache(propertyId);
        }

        void Macro_AfterSave(Macro sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.macroItemProviderGuid);
            sendToCache(itemId);
        }

        void DocumentType_AfterSave(DocumentType sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.documentTypeItemProviderGuid);
            sendToCache(itemId);
        }

        void Template_AfterSave(Template sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Alias, ItemProviders.ProviderIDCollection.templateItemProviderGuid);
            sendToCache(itemId);
        }
        
        void Media_AfterSave(Media sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediaItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.mediapropertyDataItemProviderGuid);
            
            sendToCache(new[] { itemId, propertyId });
        }

        void Document_AfterPublish(Document sender, umbraco.cms.businesslogic.PublishEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);
            

            sendToCache(new[] { itemId, propertyId });
        }

        void Document_AfterSave(Document sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            ItemIdentifier propertyId = new ItemIdentifier(sender.UniqueId.ToString(), ItemProviders.ProviderIDCollection.propertyDataItemProviderGuid);
            
            sendToCache(new[]{itemId,propertyId});
        }

        void DictionaryItem_Saving(umbraco.cms.businesslogic.Dictionary.DictionaryItem sender, EventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.key, ItemProviders.ProviderIDCollection.dictionaryItemProviderGuid);
            sendToCache(itemId);
        }
        void Language_AfterSave(umbraco.cms.businesslogic.language.Language sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.CultureAlias, ItemProviders.ProviderIDCollection.languageItemProviderGuid);
            sendToCache(itemId);
        }
        void StyleSheet_AfterSave(StyleSheet sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            ItemIdentifier itemId = new ItemIdentifier(sender.Text, ItemProviders.ProviderIDCollection.stylesheetItemProviderGuid);
            sendToCache(itemId);
        }

        private static Repository Source = null; 
        private void sendToCache(ItemIdentifier itemId)
        {
            sendToCache(new[]{itemId});
        }

        private static void sendToCache(IEnumerable<ItemIdentifier> ids)
        {
            try
            {
                var sessionKey = Guid.NewGuid().ToString();

                foreach (var id in ids)
                {
                    if (Core.Cache.ItemCacheManager.Instance.IsCacheable(id))
                    {
                        Source.Provider.SessionKey = sessionKey;
                        var provider = ItemProviderCollection.Instance.GetProvider(id.ProviderId);
                        Umbraco.Courier.Core.Cache.ItemCacheManager.Instance.ClearItem(id, provider);

                        var item = ((IPackagingTarget) Source.Provider).Package(id);
                      
                        //store in internal courier cache
                        Umbraco.Courier.Core.Cache.ItemCacheManager.Instance.StoreItemAsync(item, provider);
                    }
                }

                ((IPackagingTarget)Source.Provider).CloseSession(Source.Provider.SessionKey);


            }
            catch (Exception ex)
            {
                Logging._Error(ex.ToString());
            }
        }



        private void clearCache(ItemIdentifier itemId)
        {
            var provider = Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(itemId.ProviderId);
            Core.Cache.ItemCacheManager.Instance.ClearItem(itemId, provider);
        }
    }
}