using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using System.Text.RegularExpressions;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class LocalLinks : ItemDataResolverProvider
    {
        private static List<string> localLinkDataTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/localLinks/add", true);
        string regex = "{locallink:?([^\\}' >]+)}";

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }
        
        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {  
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data.Where( X=> X.Value != null && localLinkDataTypes.Contains( X.DataTypeEditor.ToString().ToLower()) ))
                if (cp.Value != null && cp.Value.ToString().ToLower().Contains("{locallink:"))
                     return true;                
            
            return false;
        }
        
        public override void Extracting(Item item)
        {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data)
                {
                    if (cp.Value != null && localLinkDataTypes.Contains(cp.DataTypeEditor.ToString().ToLower()))
                    {
                        string newVal = replaceGuidWithID(cp.Value.ToString());
                        if (newVal != cp.Value.ToString())
                        {
                            cp.Value = newVal;
                            if (item.Status != Core.ItemStatus.NeedPostProcessing)
                                item.Status = Core.ItemStatus.NeedPostProcessing;
                        }
                    }
                }
        }
                
        public override void Packaging(Item item)
        {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data.Where(X => localLinkDataTypes.Contains(X.DataTypeEditor.ToString().ToLower())))
                {
                    if (cp.Value != null && cp.Value.ToString().ToLower().Contains("{locallink:") && localLinkDataTypes.Contains(cp.DataTypeEditor.ToString().ToLower() ))
                    {
                        List<string> nodeGuidsFound = new List<string>();
                        string newVal = replaceIDWithGuid(cp.Value.ToString(), out nodeGuidsFound);
                        if (newVal != cp.Value.ToString())
                        {
                            cp.Value = newVal;

                            Guid g = Guid.Empty;
                            //TODO should it actually be there? (yes)
                            foreach (string id in nodeGuidsFound)
                            {
                                if (Guid.TryParse(id, out g))
                                {
                                    Core.Dependency dep = new Core.Dependency();
                                    dep.ItemId = new Core.ItemIdentifier(id, ProviderIDCollection.documentItemProviderGuid);
                                    dep.Name = "LocalLink Document";
                                    item.Dependencies.Add(dep);
                                }
                            }

                            if (item.Status != Core.ItemStatus.NeedPostProcessing)
                                item.Status = Core.ItemStatus.NeedPostProcessing;
                        }
                    }
                }
        }
        
        /*Only for content property data
        public override void Extracted(Item item)
        {
            if (item.GetType() == typeof(ContentPropertyData))
            {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data)
                {
                    if (cp.Value != null && localLinkDataTypes.Contains(cp.DataTypeEditor.ToString()))
                    {
                        string newVal = cp.Value.ToString();

                        if (newVal.ToLower().Contains("{locallink") && item.Status != Core.ItemStatus.NeedPostProcessing)
                        {
                            item.Status = Core.ItemStatus.NeedPostProcessing;
                            break;
                        }
                    }
                }
            }
        }*/
        
        /*
        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
            {
                var fileContent = ResourceAsString(resource);
                
                if (fileContent != string.Empty)
                {
                    List<string> ids;
                    fileContent = replaceIDWithGuid(fileContent, out ids);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            }
        }

        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
            {
                var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    fileContent = replaceGuidWithID(fileContent);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            }
        }
        */

        private string replaceGuidWithID(string val)
        {

            val = Regex.Replace(val, regex, delegate(Match match)
            {

                string guid = match.Groups[1].Value;
                string tag = match.ToString();


                Guid nodeGuid;

                if (Guid.TryParse(guid, out nodeGuid))
                {
                    int nodeId = PersistenceManager.Default.GetNodeId(nodeGuid);
                    if (nodeId != 0)
                        tag = tag.Replace(guid, nodeId.ToString());
                }

                return tag;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            return val;
        }

        //replace
        private string replaceIDWithGuid(string val, out List<string> ids)
        {
            List<string> idsFound = new List<string>();

            val = Regex.Replace(val, regex, delegate(Match match)
            {
                string id = match.Groups[1].Value;
                string tag = match.ToString();

                int nodeId;

                if (int.TryParse(id, out nodeId))
                {
                    Guid nodeGuid = PersistenceManager.Default.GetUniqueId(nodeId);
                    if (nodeGuid != Guid.Empty)
                    {
                        idsFound.Add(nodeGuid.ToString());
                        tag = tag.Replace(id, nodeGuid.ToString());
                    }
                }
                return tag;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            ids = idsFound;

            return val;
        }

        private string ResourceAsString(Resource r)
        {
            
            var bytes = r.ResourceContents;
            string fileContent = string.Empty;

            if (bytes == null || bytes.Length <= 0)
            {
                string p = Context.Current.MapPath(r.PackageFromPath);
                if (System.IO.File.Exists(p))
                {
                    bytes = System.IO.File.ReadAllBytes(p);
                    fileContent = Core.Settings.Encoding.GetString(bytes);
                }
            }
            else
                fileContent = Core.Settings.Encoding.GetString(bytes);


            return fileContent;
        }

    }
}