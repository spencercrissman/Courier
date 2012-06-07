using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.cache;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class RefreshDataTypeCache : ItemEventProvider
    {
        public override string Alias
        {
            get { return "ClearDataTypeCache"; }
        }

        public override void Execute(ItemIdentifier itemId, SerializableDictionary<string, string> Parameters)
        {
            try
            {
                Cache.ClearCacheByKeySearch("UmbracoDataTypeDefinition");
            }
            catch (Exception ex)
            {
                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
            }

        }
    }
}