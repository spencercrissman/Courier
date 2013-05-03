using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Persistence;

namespace Umbraco.Courier.EventHandlers.V6
{
    public class ClearInternalDataCache : ItemEventProvider
    {
        public override string Alias
        {
            get { return "ClearInternalDataCache"; }
        }

        public override void Execute(ItemIdentifier itemId, SerializableDictionary<string, string> Parameters)
        {
            CacheHelper.ClearInternalCache();
        }
    }
}