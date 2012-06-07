using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Umbraco.Courier.DataResolvers;

namespace Umbraco.Courier.uComponents
{
    public class UrlPicker : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return new Guid("22c6f52c-5d41-45b8-845e-c16354cfdd7b"); }
        }

        string dataXpath = "//node-id";
        string resourceXpath = "//url";

        public override void ExtractingProperty(Core.Item item, ItemProviders.ContentProperty propertyData)
        {
            //convert any node guids back to node Ids
            propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(propertyData.Value.ToString(), dataXpath, Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId);
        
            //register any uploaded media
        }

        public override void PackagingProperty(Core.Item item, ItemProviders.ContentProperty propertyData)
        {
            //first we need to get the mode of the picker
            XmlDocument doc = Umbraco.Courier.Core.Helpers.XmlDependencies.LoadXml(propertyData.Value.ToString());
            if (doc != null)
            {
                XmlNode node = doc.SelectSingleNode("/url-picker");

                Umbraco.Courier.Core.Helpers.Logging._Debug("loading node");

                if (node != null && node.Attributes["mode"] != null)
                {                    

                    string mode = node.Attributes["mode"].Value.ToLower();
                    Umbraco.Courier.Core.Helpers.Logging._Debug("node mode:" + mode);

                    if (mode == "upload")
                    {
                        foreach (var resource in Umbraco.Courier.Core.Helpers.XmlDependencies.FindResources(propertyData.Value.ToString(), resourceXpath, null))
                        {
                            item.Resources.Add(resource);
                        }
                    }

                    if (mode == "content" || mode == "media")
                    {
                        var provider = Courier.ItemProviders.ProviderIDCollection.documentItemProviderGuid;
                        if (mode == "media")
                            provider = Courier.ItemProviders.ProviderIDCollection.mediaItemProviderGuid;


                        List<string> replacedIds = new List<string>();
                        propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(propertyData.Value.ToString(), dataXpath, Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid, out replacedIds);

                        foreach (string guid in replacedIds)
                        {
                            if(!string.IsNullOrEmpty(guid.Trim()))
                                item.Dependencies.Add(guid, provider);
                        }
                    }
                }


            }
        }       
    }
}