using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using umbraco.cms.businesslogic.cache;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class RefreshDocumentTypePropertyCache : ItemEventProvider
    {
        public override string Alias
        {
            get { return "ClearContentTypeCache"; }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            try
            {
                Cache.ClearCacheByKeySearch("UmbracoContentType");
                Cache.ClearCacheByKeySearch("ContentType_PropertyTypes_Content");
                Cache.ClearCacheByKeySearch("Tab_PropertyTypes_Content");
            }
            catch (Exception ex)
            {
                Core.Helpers.Logging._Error(ex.ToString());
            }
        }
    }
}