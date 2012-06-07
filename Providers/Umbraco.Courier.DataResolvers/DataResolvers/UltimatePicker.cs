using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers.DataResolvers
{
    public class UltimatePicker : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return Guid.Parse("cdbf0b5d-5cb2-445f-bc12-fcaaec07cf2c"); }
        }

        public override void PackagingDataType(ItemProviders.DataType item)
        {
            if (item.Prevalues.Count > 0 && !string.IsNullOrEmpty(item.Prevalues[0].Value))
            {
                var vals = item.Prevalues[0].Value.Split('|');
                var nodeGuid = Dependencies.ConvertIdentifier(vals[1], Core.Enums.IdentifierReplaceDirection.FromNodeIdToGuid);

                Guid g = Guid.Empty;
                if (Guid.TryParse(nodeGuid, out g))
                {
                    item.Dependencies.Add(nodeGuid, ProviderIDCollection.documentItemProviderGuid);
                    vals[1] = nodeGuid;
                    item.Prevalues[0].Value = string.Join("|", vals);
                }
            }
        }

        public override void ExtractingDataType(DataType item)
        {
            if (item.Prevalues.Count > 0 && !string.IsNullOrEmpty(item.Prevalues[0].Value))
            {
                var vals = item.Prevalues[0].Value.Split('|');
                var nodeID = Dependencies.ConvertIdentifier(vals[1], Core.Enums.IdentifierReplaceDirection.FromGuidToNodeId);

                int i = 0;
                if (int.TryParse(nodeID, out i))
                {
                    vals[1] = nodeID;
                    item.Prevalues[0].Value = string.Join("|", vals);
                }
            }
        }
    }
}