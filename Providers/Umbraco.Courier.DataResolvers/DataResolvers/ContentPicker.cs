using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class ContentPicker : ItemDataResolverProvider
    {
        private Dictionary<string, string> contentPickerDataTypes = new Dictionary<string, string>();

        public ContentPicker()
        {
            contentPickerDataTypes = Context.Current.Settings.GetConfigurationKeyValueCollection("/configuration/itemDataResolvers/contentPickers/add", true);
        }
        
        public override List<Type> ResolvableTypes
        {
            get{return  new List<Type>() { typeof(ContentPropertyData) };}
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach(var data in cpd.Data)
                if (data.Value != null && contentPickerDataTypes.ContainsValue(data.DataTypeEditor.ToString().ToLower()))
                    return true;               

            return false;
        }
        
        public override void Extracting(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && contentPickerDataTypes.Values.Contains(cp.DataTypeEditor.ToString().ToLower()))
                {
                    if (Core.Helpers.Dependencies.IsNodeGuidList(cp.Value.ToString()))
                    {
                        //a content picker either stores IDs as a single int or comma seperated
                        string[] vals = cp.Value.ToString().Split(',');
                        string newVals = string.Empty;
                        bool changed = false;

                        foreach (string val in vals)
                        {
                            if (!string.IsNullOrEmpty(val))
                            {
                                Guid docGUID;
                                if (Guid.TryParse(val, out docGUID))
                                {
                                    int nodeId = PersistenceManager.Default.GetNodeId(docGUID, NodeObjectTypes.Document);
                                    if (nodeId != 0)
                                    {
                                        newVals += nodeId.ToString() + ",";
                                        changed = true;
                                    }
                                }
                                else
                                {
                                    newVals += val.Trim() + ',';
                                }
                            }
                        }
                        //replace the GUIDs with int IDs, everything is now back to normal... 
                        if (changed)
                            cp.Value = newVals.Trim(',');
                    }
                }
            }
        }

        public override void Packaging(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && !string.IsNullOrEmpty(cp.Value.ToString()) && contentPickerDataTypes.Values.Contains(cp.DataTypeEditor.ToString().ToLower()))
                {

                    if (Core.Helpers.Dependencies.IsNodeIdList(cp.Value.ToString()))
                    {

                        //a media picker either stores IDs as a single int or comma seperated
                        string[] vals = cp.Value.ToString().Split(',');
                     //   string[] contentPath = cpd.ContentPath.Split(',');

                        string newVals = string.Empty;

                        foreach (string val in vals)
                        {
                            if (!string.IsNullOrEmpty(val.Trim()))
                            {
                                int docID = 0;
                                if (int.TryParse(val.Trim(), out docID))
                                {

                                    Guid nodeGuid = PersistenceManager.Default.GetUniqueId(docID, NodeObjectTypes.Document);
                                    if (nodeGuid != Guid.Empty)
                                    {
                                        //Create a dependency to the content item (which will then hold the actual file as a resource)
                                        Dependency fileDep = new Dependency();
                                        fileDep.ItemId = new ItemIdentifier(nodeGuid.ToString(), ProviderIDCollection.documentItemProviderGuid);
                                        fileDep.Name = "Document from picker";

                                        //this determins if a picked node is a parent of the current one, and then determins if it is actually
                                        //is needed for the node to be installed
                                        //if (!contentPath.Contains(val.Trim()))
                                        //    fileDep.IsChild = true;

                                        item.Dependencies.Add(fileDep);
                                        newVals += nodeGuid.ToString() + ",";
                                    }else
                                        newVals += val.Trim() + ",";
                                }
                                else
                                {
                                    newVals += val.Trim() + ",";
                                }
                            }
                        }

                        //replace the int ID values with GUID values
                        cp.Value = newVals.Trim(',');
                    }
                }
            }
        }
    }
}