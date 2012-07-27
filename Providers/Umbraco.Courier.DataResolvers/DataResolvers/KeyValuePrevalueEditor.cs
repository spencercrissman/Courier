using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class KeyValuePrevalueEditor : ItemDataResolverProvider
    {
        private Dictionary<string, string> keyValuePrevalueEditors = Context.Current.Settings.GetConfigurationKeyValueCollection("/configuration/itemDataResolvers/keyValuePrevalueEditors/add", true);
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var data in cpd.Data)
                if (keyValuePrevalueEditors.ContainsValue(data.PreValueEditor.ToString().ToLower()))
                    return true;

            return false;
        }

        public override void Extracting(Item item)
        {

            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (keyValuePrevalueEditors.Values.Contains(cp.PreValueEditor.ToLower()) && cp.Value != null)
                {
                    //nonConvert indicator
                    string value = cp.Value.ToString();
                    bool nonConvert = value.StartsWith("¤");
                    value = value.TrimStart('¤');

                    string[] vals = value.Split(',');
                    string newVals = string.Empty;

                    ItemIdentifier itemid = new ItemIdentifier(cp.DataType.ToString(), ProviderIDCollection.dataTypeItemProviderGuid);

                    if (nonConvert)
                        cp.Value = value.Trim(',');
                    else
                    {
                        DataType dt = PersistenceManager.Default.RetrieveItem<DataType>(itemid);
                        if (dt != null)
                        {
                            foreach (string s in vals)
                            {
                                var val = dt.Prevalues.Where(x => x.Value == s).FirstOrDefault();
                                if (val != null)
                                {
                                    newVals += val.Id.ToString() + ",";
                                }
                            }
                            cp.Value = newVals.Trim(',');
                        }
                    }
                }
            }
        }

        
        public override void Packaging(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (keyValuePrevalueEditors.Values.Contains(cp.PreValueEditor.ToLower()) && cp.Value != null)
                {
                    string[] vals = cp.Value.ToString().Split(',');
                    string newVals = string.Empty;
                    
                    ItemIdentifier itemid = new ItemIdentifier(cp.DataType.ToString(), ProviderIDCollection.dataTypeItemProviderGuid);
                    DataType dt = PersistenceManager.Default.RetrieveItem<DataType>(itemid);

                    bool nonConvert = false;
                    int convertTestVal = 0;
                    if (!int.TryParse(string.Join("", vals), out convertTestVal))
                        nonConvert = true;


                    if (dt != null)
                    {
                        foreach (string s in vals)
                        {
                            int id;
                            if (int.TryParse(s, out id))
                            {
                                if (id > 0)
                                {
                                    var val = dt.Prevalues.Where(x => x.Id == id).FirstOrDefault();
                                    if (val != null)
                                        newVals += val.Value + ",";
                                }
                            }
                            else if(dt.Prevalues.Where(x => x.Value == s).Any())
                            {
                                newVals += s + ",";
                            }
                        }

                        newVals = newVals.Trim(',');

                        //this is a nasty hack but no way around it due to the idiotic way keyvalue types can store stuff
                        if (nonConvert)
                            newVals = "¤" + newVals;

                        cp.Value = newVals;
                    }
                }
            }
        }


    }
}