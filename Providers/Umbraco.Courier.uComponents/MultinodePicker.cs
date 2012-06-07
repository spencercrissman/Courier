using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.Core.Enums;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.DataResolvers;

namespace Umbraco.Courier.uComponents
{
    public class MultinodePicker : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return new Guid("c2d6894b-e788-4425-bcf2-308568e3d38b"); }
        }

        string dataXpath = "//nodeId";

        public override void ExtractingProperty(Core.Item item, ContentProperty propertyData)
        {
            propertyData.Value = XmlDependencies.ReplaceIds(
                                                            propertyData.Value.ToString(),
                                                            dataXpath,
                                                            IdentifierReplaceDirection.FromGuidToNodeId);
        }

        public override void PackagingProperty(Core.Item item, ContentProperty propertyData)
        {
            if (propertyData != null && propertyData.Value != null)
            {
                List<string> replacedIds = new List<string>();
                propertyData.Value = XmlDependencies.ReplaceIds(
                                        propertyData.Value.ToString(),
                                        dataXpath, IdentifierReplaceDirection.FromNodeIdToGuid,
                                        out replacedIds);

                //these are the IDs we found in the picker, those documents are a dependency
                foreach (string guid in replacedIds)
                {
                    if(!string.IsNullOrEmpty(guid.Trim()))
                        item.Dependencies.Add(guid, ProviderIDCollection.documentItemProviderGuid);
                }
            }
        }

        public override void PackagingDataType(DataType item)
        {
            //picker prevalues / settings, we look for node Id values, which can be converted into guids
            foreach (var setting in item.Prevalues)
            {
                var currentSetting = setting.Value;
                var guid = Dependencies.ConvertIdentifier(
                                                            currentSetting,
                                                            IdentifierReplaceDirection.FromNodeIdToGuid);

                if (currentSetting != guid)
                {
                    //set the guid
                    setting.Value = guid;

                    //add document as a dependency
                    item.Dependencies.Add(guid, ProviderIDCollection.documentItemProviderGuid);
                }
            }
        }

        public override void ExtractingDataType(DataType item)
        {

            foreach (var setting in item.Prevalues)
            {
                setting.Value = Dependencies.ConvertIdentifier(
                                                                setting.Value,
                                                                IdentifierReplaceDirection.FromGuidToNodeId);
            }
        }
    }
}