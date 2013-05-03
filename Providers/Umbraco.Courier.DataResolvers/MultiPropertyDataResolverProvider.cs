using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Enums;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public abstract class MultiPropertyDataResolverProvider : PropertyDataResolverProvider
    {
        public abstract IEnumerable<Guid> DataTypeIds { get; }

        public override bool ShouldExecute(Core.Item item, Core.Enums.ItemEvent itemEvent)
        {
            var type = item.GetType();

            if (type == typeof(ContentPropertyData))
            {
                var propertyData = (ContentPropertyData)item;

                var rightEvent = (itemEvent == ItemEvent.Extracting || itemEvent == ItemEvent.Packaging);

                if(!rightEvent)
                    return false;

                var hasType = propertyData.Data.Any(x => x.Value != null && DataTypeIds.Contains(x.DataTypeEditor));
                return hasType;

            }
            else
            {
                var dataType = (DataType)item;
                return ( DataTypeIds.Contains(dataType.DataEditorId));
            }
        }

        public override void Extracting(Item item)
        {
            if (item != null)
            {
                var type = item.GetType();

                if (type == typeof(ContentPropertyData))
                {
                    var propertyData = (ContentPropertyData)item;
                    foreach (var property in propertyData.Data.Where(x => DataTypeIds.Contains(x.DataTypeEditor)))
                    {
                        if (property != null && property.Value != null)
                            ExtractingProperty(item, property);
                    }
                }
                else
                {
                    ExtractingDataType((DataType)item);
                }
            }
        }


        public override void Packaging(Item item)
        {
            if (item != null)
            {
                var type = item.GetType();

                if (type == typeof(ContentPropertyData))
                {
                    var propertyData = (ContentPropertyData)item;
                    foreach (var property in propertyData.Data.Where(x => DataTypeIds.Contains(x.DataTypeEditor)))
                    {
                        if (property != null && property.Value != null)
                            PackagingProperty(item, property);
                    }
                }
                else
                {
                    PackagingDataType((DataType)item);
                }
            }
        }

        public override Guid DataTypeId
        {
            get { return Guid.Empty; }
        }


    }
}