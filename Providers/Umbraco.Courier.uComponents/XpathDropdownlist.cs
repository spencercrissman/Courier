using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.DataResolvers;

namespace Umbraco.Courier.uComponents.XpathLists
{
    public class XpathDropdownList : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get
            {
                return new Guid("57a62843-c488-4c29-8125-52f51873613e");
            }
        }

        string dataXpath = "//nodeId";

        public override void ExtractingProperty(Core.Item item, ItemProviders.ContentProperty propertyData)
        {
            propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(propertyData.Value.ToString(), dataXpath, Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId);
        }

        public override void PackagingProperty(Core.Item item, ItemProviders.ContentProperty propertyData)
        {
            List<string> replacedIds = new List<string>();
            propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(propertyData.Value.ToString(), dataXpath, Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid, out replacedIds);

            //these are the IDs we found in the picker, those documents are a dependency
            foreach (string guid in replacedIds)
            {
                if (!string.IsNullOrEmpty(guid.Trim()))
                    item.Dependencies.Add(guid, Courier.ItemProviders.ProviderIDCollection.documentItemProviderGuid);
            }
        }
    }
}