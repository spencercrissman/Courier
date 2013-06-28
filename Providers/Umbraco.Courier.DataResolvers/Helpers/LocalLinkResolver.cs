using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers.Helpers
{
    public class LocalLinkResolver
    {
        string regex = "{locallink:?([^\\}' >]+)}";
        public bool RegisterLinksAsDependencies { get; set; }

        public string ReplaceLocalLinks(string value, bool toUnique, Item item)
        {
            if (toUnique)
            {
                List<string> newIds = new List<string>();
                value = replaceIDWithGuid(value, out newIds);

                if (RegisterLinksAsDependencies)
                {
                    foreach (var s in newIds)
                        item.Dependencies.Add(s, ProviderIDCollection.documentItemProviderGuid);
                }
                
                return value;
            }
            else
            {
                return replaceGuidWithID(value);
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
    }
}