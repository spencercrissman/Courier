using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class DampResolver : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return Guid.Parse("ef94c406-9e83-4058-a780-0375624ba7ca"); }
        }

        public override void PackagingDataType(Umbraco.Courier.ItemProviders.DataType item)
        {
            //to ensure UI is there, include the digibiz folder of files
            item.Dependencies.Add("~/umbraco/plugins/DigibizAdvancedMediaPicker", Umbraco.Courier.ItemProviders.ProviderIDCollection.folderItemProviderGuid);
            item.Resources.Add("~/bin/DigibizTree.dll");

            var source = item.Prevalues.Where(x => x.SortOrder == 2).FirstOrDefault();
            var defaultType = item.Prevalues.Where(x => x.SortOrder == 13).FirstOrDefault();
            

            List<string> foundNodes = new List<string>();
            if (source != null && source.Value != null){
                source.Value = Dependencies.ConvertIdentifierCollection(source.Value.ToString(), out foundNodes);

                foreach (var g in foundNodes)
                    item.Dependencies.Add(g, ProviderIDCollection.mediaItemProviderGuid);

            }
            

            if (defaultType != null && defaultType.Value != null)
            {
                defaultType.Value = Dependencies.ConvertIdentifierCollection(defaultType.Value.ToString(), out foundNodes);

                foreach (var g in foundNodes)
                    item.Dependencies.Add(g, ProviderIDCollection.mediaTypeItemProviderGuid);
            }
        }

        public override void ExtractingDataType(ItemProviders.DataType item)
        {
            var source = item.Prevalues.Where(x => x.SortOrder == 2).FirstOrDefault();
            var defaultType = item.Prevalues.Where(x => x.SortOrder == 13).FirstOrDefault();
            List<string> foundNodes = new List<string>();

            if (source != null && source.Value != null)
                source.Value = Dependencies.ConvertIdentifierCollection(source.Value.ToString(), out foundNodes);
            

            if (defaultType != null && defaultType.Value != null)
                defaultType.Value = Dependencies.ConvertIdentifierCollection(defaultType.Value.ToString(), out foundNodes);
            
        }


        public override void PackagingProperty(Umbraco.Courier.Core.Item item, Umbraco.Courier.ItemProviders.ContentProperty propertyData)
        {
            if (propertyData.DBType.ToLower() == "ntext")
            {
                List<string> replacedIds = new List<string>();
                propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(
                                                    propertyData.Value.ToString(),
                                                      "//mediaItem/*",
                                                    "id",
                                                    Umbraco.Courier.Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid,
                                                    out replacedIds);

                //replaced ids are populated by found and confirmed ids..
                foreach (var s in replacedIds)
                    item.Dependencies.Add(s, Umbraco.Courier.ItemProviders.ProviderIDCollection.mediaItemProviderGuid);


                //for good measure we are also replace parentId, but not adding it as a dependency...
                propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(
                                                    propertyData.Value.ToString(),
                                                    "//mediaItem/*",
                                                    "parentID",
                                                    Umbraco.Courier.Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid,
                                                    out replacedIds);
            }
        }



        public override void ExtractingProperty(Umbraco.Courier.Core.Item item, Umbraco.Courier.ItemProviders.ContentProperty propertyData)
        {
            if (propertyData.DBType.ToLower() == "ntext")
            {


                List<string> replacedIds = new List<string>();
                propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(
                                            propertyData.Value.ToString(),
                                            "//mediaItem/*",
                                            "id",
                                            Umbraco.Courier.Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId,
                                            out replacedIds);

                propertyData.Value = Umbraco.Courier.Core.Helpers.XmlDependencies.ReplaceIds(
                                            propertyData.Value.ToString(),
                                            "//mediaItem/*",
                                            "parentID",
                                            Umbraco.Courier.Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId,
                                            out replacedIds);

                Umbraco.Courier.Core.Helpers.Logging._Debug(propertyData.Value.ToString());
            }
        }


    }
}