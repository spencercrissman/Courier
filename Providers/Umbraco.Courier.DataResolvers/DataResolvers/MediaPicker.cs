using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.DataResolvers
{
    public class MediaPicker : ItemDataResolverProvider
    {
        private Dictionary<string, string> mediaPickerDataTypes = new Dictionary<string, string>();

        public MediaPicker()
        {
            mediaPickerDataTypes = Context.Current.Settings.GetConfigurationKeyValueCollection("/configuration/itemDataResolvers/mediaPickers/add", true);
        }
        
        public override List<Type> ResolvableTypes
        {
            get{return  new List<Type>() { typeof(ContentPropertyData) };}
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach(var data in cpd.Data)
                if (data.Value != null && mediaPickerDataTypes.ContainsValue(data.DataTypeEditor.ToString().ToLower()))
                    return true;               

            return false;
        }

        public override void Extracting(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && mediaPickerDataTypes.Values.Contains(cp.DataTypeEditor.ToString().ToLower()))
                {
                    if (Core.Helpers.Dependencies.IsNodeGuidList(cp.Value.ToString()))
                    {
                        //a content picker either stores IDs as a single int or comma seperated
                        string[] vals = cp.Value.ToString().Split(',');
                        string newVals = string.Empty;

                        foreach (string val in vals)
                        {
                            Guid docGUID;

                            if (Guid.TryParse(val, out docGUID))
                            {
                                int nodeId = PersistenceManager.Default.GetNodeId(docGUID, NodeObjectTypes.Media);
                                if (nodeId != 0)
                                    newVals += nodeId.ToString() + ",";
                            }
                        }
                        //replace the GUIDs with int IDs, everything is now back to normal... 
                        cp.Value = newVals.Trim(',');
                    }
                }
            }
        }

        public override void Packaging(Item item)
        {
             Logging._Error("logging media picker packagign " + item.Name);

            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && mediaPickerDataTypes.Values.Contains(cp.DataTypeEditor.ToString().ToLower()))
                {
                    if (Core.Helpers.Dependencies.IsNodeIdList(cp.Value.ToString())){
                    //a media picker either stores IDs as a single int or comma seperated
                    string[] vals = cp.Value.ToString().Split(',');
                    string newVals = string.Empty;
                    bool changed = false;

                    Logging._Error("Adding media picker data from: " + cp.Value.ToString());
    
                    foreach (string val in vals)
                    {
                        int docID;
                        if (int.TryParse(val, out docID))
                        {
                            Guid nodeGuid = PersistenceManager.Default.GetUniqueId(docID, NodeObjectTypes.Media);

                            if (nodeGuid != Guid.Empty)
                            {
                                //Create a dependency to the content item (which will then hold the actual file as a resource)
                                Dependency fileDep = new Dependency();
                                fileDep.ItemId = new ItemIdentifier(nodeGuid.ToString(), ProviderIDCollection.mediaItemProviderGuid);
                                fileDep.Name = "Media from picker";

                                item.Dependencies.Add(fileDep);

                                newVals += nodeGuid.ToString() + ",";
                                changed = true;
                            }else
                               newVals += val.Trim() + ",";
                        }
                    }

                    //replace the int ID values with GUID values
                    if(changed)
                        cp.Value = newVals.Trim(',');
                }

                }
            }
        }
    }
}