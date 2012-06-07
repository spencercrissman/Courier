using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;
using System.Text;
using Umbraco.Courier.Core.Diagnostics.Logging;
using System.Text.RegularExpressions;

namespace Umbraco.Courier.DataResolvers.ResourceResolvers
{
    public class MacroParameters : ResourceDataResolverProvider
    {
        private static List<string> macroDataTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/macros/add", true);
        private static List<string> macroPropertyTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/macroPropertyTypeResolvers/contentPickers/add", true);
        

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>(){ typeof(Template) }; }
        }

        public override bool ShouldExecute(Type itemType, Core.ItemIdentifier itemId, Core.Resource resource, Core.Enums.ItemEvent eventType)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
                return true;

            return false;
        }

        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
           
            {
                var item = PersistenceManager.Default.RetrieveItem<Template>(itemId);
                var encoding = Core.Settings.Encoding;

                var contents = resource.ResourceContents;
                string fileContent = string.Empty;

                if (contents == null || contents.Length <= 0)
                {
                    string p = Context.Current.MapPath(resource.PackageFromPath);

                    if (System.IO.File.Exists(p))
                    {
                        contents = System.IO.File.ReadAllBytes(p);
                        fileContent = encoding.GetString(contents);
                    }
                }
                else
                    fileContent = encoding.GetString(contents);

                if (fileContent != string.Empty)
                {

                    fileContent = ReplaceMacrosInstring(fileContent, true, item);
                    resource.ResourceContents = encoding.GetBytes(fileContent);
                }
            }
        }

        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
          
                var item = PersistenceManager.Default.RetrieveItem<Template>(itemId);
                var encoding = Core.Settings.Encoding;

                var contents = resource.ResourceContents;

                if (contents != null && contents.Length > 0)
                {
                    string fileContent = encoding.GetString(contents);

                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        fileContent = ReplaceMacrosInstring(fileContent, false, item);
                        resource.ResourceContents = encoding.GetBytes(fileContent);
                    }
                }
            
        }

        public string ReplaceMacrosInstring(string str, bool toUnique, Item item)
        {
            string original = str;

            RevisionLog.Instance.Information(item.ItemId, this, RevisionLog.ItemDataResolvers + " - MacroParameters", "Replacing macros in string to guid:" + toUnique.ToString());

            try
            {
                StringBuilder result = new StringBuilder();

                string propertyValue = str;
                propertyValue = propertyValue.Replace("<umbraco:Macro", "<umbraco:macro", StringComparison.OrdinalIgnoreCase);
                propertyValue = propertyValue.Replace("<?UMBRACO_MACRO", "<?umbraco_macro", StringComparison.OrdinalIgnoreCase);

                if (propertyValue.IndexOf("<umbraco:macro") > -1 || propertyValue.IndexOf("<?umbraco_macro") > -1)
                {
                    StringBuilder fieldResult = new StringBuilder(propertyValue);

                    bool stop = false;
                    int processed = 0;

                    while (!stop)
                    {
                        // look for the different tags. we need to process them in order, though, we will never typically see both at the same time.
                        //change to regex?

                        int tagIndex1 = fieldResult.ToString().IndexOf("<?umbraco_macro");
                        int tagIndex2 = fieldResult.ToString().IndexOf("<umbraco:macro");
                        int tagIndex = -1;

                        if (tagIndex1 > -1 && tagIndex2 > -1)
                            tagIndex = Math.Min(tagIndex1, tagIndex2);
                        else if (tagIndex1 > -1)
                            tagIndex = tagIndex1;
                        else if (tagIndex2 > -1)
                            tagIndex = tagIndex2;

                        if (tagIndex > -1)
                        {
                            //move content between macros to result
                            result.Append(original.Substring(processed, tagIndex + 1));
                            processed += (tagIndex + 1);

                            //  string tempElementContent = "";
                            fieldResult.Remove(0, tagIndex + 1);

                            //the entire tag
                            string tag = fieldResult.ToString().Substring(0, fieldResult.ToString().IndexOf(">") + 1);

                            RevisionLog.Instance.Information(item.ItemId, this, RevisionLog.ItemDataResolvers + " - MacroParameters", "Found tag:" + tag);

                            string transFormedTag = TransformMacroTag(tag, toUnique, item);
                            result.Append(transFormedTag);

                            processed += (tag.Length);
                            fieldResult.Remove(0, tag.Length);
                        }
                        else
                        {
                            if (processed < original.Length)
                            {
                                string end = original.Substring(processed);
                                result.Append(end);
                            }

                            break;
                        }
                    }

                    return result.ToString();
                }
            }
            catch (Exception ex)
            {
                RevisionLog.Instance.Error(item, this, RevisionLog.ItemDataResolvers + " - MacroParameters", ex.ToString());
            }

            return original;
        }

        private string TransformMacroTag(string tag, bool toUnique, Item item)
        {
            //load macro tag struct
            var macroTag = Dependencies.CreateMacroTag(tag);

            if (string.IsNullOrEmpty(macroTag.Alias))
                return tag;

            //load the macro
            var macro = PersistenceManager.Default.RetrieveItem<Macro>(new ItemIdentifier(macroTag.Alias, ProviderIDCollection.macroItemProviderGuid));
            if (macro == null)
            {
                RevisionLog.Instance.Error(this, "DataResolvers", string.Format("Could not load the macro '{0}' that is called in the template {1}", macroTag.Alias, item.Name));
                return tag;
            }

            string _g = "to guids";
            if (!toUnique)
                _g = "to node ids";

            RevisionLog.Instance.Information(this, "DataResolvers", "Transforming macroparameters on macro tag " + _g + ": \n\n'" + tag + "'");

            //add dependency to macro for the template or properties
            if (toUnique)
            {
                if (!item.Dependencies.Exists(item.ItemId))
                {
                    item.Dependencies.Add(macro.ItemId.Id, macro.ItemId.ProviderId);
                }
            }

            //set the values of the macroTag
            var newTag = Setvalues(macro, macroTag, toUnique);


            //replace the attribute values
            foreach (var attr in macroTag.Attributes)
            {
                string attrRegex = string.Format(@"{0}=\""(.*?)\""", attr.Key);
                tag = Regex.Replace(tag, attrRegex, attr.Key + "=\"" + attr.Value + "\"");
            }


            return tag;
        }

        private Umbraco.Courier.Core.Helpers.Dependencies.MacroTag Setvalues(Macro m, Umbraco.Courier.Core.Helpers.Dependencies.MacroTag tag, bool toUnique)
        {

            if (m.Properties != null && m.Properties.Count > 0)
            {
                foreach (var prop in m.Properties.Where(x => !string.IsNullOrEmpty(x.PropertyTypeAlias) && macroPropertyTypes.Contains(x.PropertyTypeAlias.ToLower())))
                {

                    var lAlias = prop.Alias.ToLower();

                    if (tag.Attributes.ContainsKey(lAlias))
                    {
                        if (toUnique)
                        {
                            int id = 0;

                            if (int.TryParse(tag.Attributes[lAlias], out id))
                            {

                                Guid guid = PersistenceManager.Default.GetUniqueId(id);

                                if (guid != Guid.Empty)
                                    tag.Attributes[lAlias] = guid.ToString();
                            }
                        }
                        else
                        {
                            Guid guid = Guid.Empty;
                            if (Guid.TryParse(tag.Attributes[lAlias], out guid))
                            {
                                int id = PersistenceManager.Default.GetNodeId(guid);

                                if (id != 0)
                                    tag.Attributes[lAlias] = id.ToString();
                            }

                        }
                    }

                }
            }
            return tag;
        }
    }
}