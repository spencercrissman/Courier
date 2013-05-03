using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.DataResolvers
{
    public class EmbeddedContent : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return Guid.Parse("454545ab-1234-4321-abcd-1234567890ab"); }
        }
                
        public override void PackagingProperty(Core.Item item, ItemProviders.ContentProperty propertyData)
        {
            if (propertyData != null && item != null && propertyData.Value != null)
            {
                List<string> idsFound = new List<string>();
                propertyData.Value = XmlDependencies.ReplaceIds(propertyData.Value.ToString(), "//* [@propertyid != '']", Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid, out idsFound);

                Logging._Debug(idsFound.Count + " Embedded content ids found");
                //we find and replace ids
                foreach (var id in idsFound)
                {
                    Guid g = Guid.Empty;
                    //we have to it this way, as there is no indication in the data whether the ID is media or document
                    if (Guid.TryParse(id, out g))
                    {
                        //is it content?
                        if (PersistenceManager.Default.GetNodeId(g, NodeObjectTypes.Document) != 0)
                            item.Dependencies.Add(id, ProviderIDCollection.documentItemProviderGuid);
                        else if (PersistenceManager.Default.GetNodeId(g, NodeObjectTypes.Media) != 0)
                            item.Dependencies.Add(id, ProviderIDCollection.mediaItemProviderGuid);
                    }
                }
            }
       }
       public override void ExtractingProperty(Item item, ContentProperty propertyData)
       {
           propertyData.Value = XmlDependencies.ReplaceIds(propertyData.Value.ToString(), "//* [@propertyid != '']", Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId);
       }
    }

    
}