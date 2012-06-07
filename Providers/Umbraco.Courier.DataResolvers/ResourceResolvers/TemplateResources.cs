using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.ProviderModel;
using System.Text.RegularExpressions;

namespace Umbraco.Courier.DataResolvers.ResourceResolvers
{
    public class TemplateResources : ResourceDataResolverProvider
    {
        string regex = "{locallink:?([^\\}' >]+)}";
        public override bool ShouldExecute(Type itemType, ItemIdentifier itemId, Resource resource, Core.Enums.ItemEvent eventType)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
                return true;


            return false;
        }
        
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Template) }; }
        }       

        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
                var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    List<string> ids;
                    fileContent = replaceIDWithGuid(fileContent, out ids);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            
        }


        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
               var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    fileContent = replaceGuidWithID(fileContent);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            
        }


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