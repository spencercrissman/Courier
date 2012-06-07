using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Interfaces;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.DataResolvers
{
    public class Tags : PropertyDataResolverProvider
    {
        private Guid tagsDataType = new Guid("4023e540-92f5-11dd-ad8b-0800200c9a66");
         

        public override Guid DataTypeId
        {
            get { return new Guid("4023e540-92f5-11dd-ad8b-0800200c9a66"); }
        }
        
        public override void PackagingProperty(Item item, ContentProperty propertyData)
        {
            if (item != null)
            {
                item.Dependencies.Add(item.ItemId.Id, ProviderIDCollection.tagRelationsProviderGuid);
            }
        }

    }
}