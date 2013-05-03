using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.cache;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Core.Diagnostics.Logging;

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
                RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "ClearDataTypeCache", ex.ToString(), LogItemEntryType.Error);
            }

        }
    }
}