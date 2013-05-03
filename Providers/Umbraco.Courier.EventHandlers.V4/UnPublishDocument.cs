using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Core;
using System.Xml;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class UnPublishDocument : ItemEventProvider
    {
        public override string Alias
        {
            get
            {
                return "UnPublishDocument";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            Guid g;
            if (Guid.TryParse(itemId.Id, out g))
            {
                try {
                    umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(g);

                    if (d != null)
                    {
                        d.UnPublish();
                        umbraco.library.UnPublishSingleNode(d.Id);
                    }

                }
                catch (Exception ex) {
                    Umbraco.Courier.Core.Diagnostics.Logging.RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "UnpushlishDocument", ex.ToString(), Core.Diagnostics.Logging.LogItemEntryType.Error);
                }
            }
        }
    }
}