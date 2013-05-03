using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Enums;

namespace Umbraco.Courier.DataResolvers
{

    public abstract class PropertyDataResolverProvider : ItemDataResolverProvider
    {
        public abstract Guid DataTypeId { get; }

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type> { typeof(ContentPropertyData), typeof(DataType) }; }
        }

        public override bool ShouldExecute(Item item, ItemEvent itemEvent)
        {
            var type = item.GetType();

            if (type == typeof(ContentPropertyData))
            {
                var propertyData = (ContentPropertyData)item;
                return propertyData.Data.Any(x => x.Value != null && x.DataTypeEditor == DataTypeId) && (itemEvent == ItemEvent.Extracting || itemEvent == ItemEvent.Packaging);
            }
            else
            {
                var dataType = (DataType)item;
                return (dataType.DataEditorId == DataTypeId);
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
                    foreach (var property in propertyData.Data.Where(x => x.DataTypeEditor == DataTypeId))
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
                    foreach (var property in propertyData.Data.Where(x => x.DataTypeEditor == DataTypeId))
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



        public virtual void ExtractingProperty(Item item, ContentProperty propertyData) { }
        public virtual void PackagingProperty(Item item, ContentProperty propertyData) { }
        
        public virtual void PackagingDataType(DataType item){}
        public virtual void ExtractingDataType(DataType item){}

        

        
    }
}