using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class RTEstylesheets : PropertyDataResolverProvider
    {

        public override Guid DataTypeId
        {
            get { return Guid.Parse("5E9B75AE-FACE-41c8-B47E-5F4B0FD82F83"); }
        }


        public override void ExtractingDataType(DataType item)
        {
            base.ExtractingDataType(item);

            if (shouldRun(item))
            {
                var valueArray = item.Prevalues[0].Value.Split('|');
                var ids = valueArray[5].Split(',');

                bool changed = false;
                string newIds = "";
                
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var tempItemId = new ItemIdentifier(id, ItemProviders.StyleSheetItemProvider.Instance().Id);
                        var stylesheet = CurrentItemProvider.DatabasePersistence.RetrieveItem<Stylesheet>(tempItemId);

                        if (stylesheet != null)
                        {
                            newIds += stylesheet.Id.ToString() + ",";
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    newIds = newIds.Trim(',');
                    valueArray[5] = newIds;
                    item.Prevalues[0].Value = string.Join("|", valueArray);    
                }
            }
        }


        public override void PackagingDataType(DataType item)
        {
            base.PackagingDataType(item);

            if (shouldRun(item))
            {
                var valueArray = item.Prevalues[0].Value.Split('|');
                var ids = valueArray[5].Split(',');
                bool changed = false;
                string newIds = "";

      
                foreach(var id in ids){
                    int _id = 0;

                    if(int.TryParse(id, out _id))
                    {
                        //we create a temp Id to fetch using the nodeId, which does not work across instances, but the returned object does return a valid alias
                        var tempItemId = new ItemIdentifier(_id.ToString(), ItemProviders.StyleSheetItemProvider.Instance().Id);
                        var stylesheet = CurrentItemProvider.DatabasePersistence.RetrieveItem<Stylesheet>(tempItemId);

                        if (stylesheet != null)
                        {
                            item.Dependencies.Add(stylesheet.ItemId);
                            newIds += stylesheet.Alias + ",";
                            changed = true;
                        }
                    }
                }

                
                
                if (changed)
                {
                    newIds = newIds.Trim(',');
                    valueArray[5] = newIds;
                    item.Prevalues[0].Value = string.Join("|", valueArray);    
                }
            }
        }

        private bool shouldRun(DataType item)
        {
            if (item.Prevalues != null && item.Prevalues.Count > 0 && !string.IsNullOrEmpty(item.Prevalues[0].Value))
                if (item.Prevalues[0].Value.Split('|').Count() > 6)
                    return true;
                    
            return false;
        }
    }
}