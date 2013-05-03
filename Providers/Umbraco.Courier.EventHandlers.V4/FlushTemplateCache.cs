using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using umbraco.cms.businesslogic.template;

namespace Umbraco.Courier.EventHandlers.V4
{
    public class FlushTemplateCache : ItemEventProvider
    {

        public override string Alias
        {
            get
            {
                return "FlushTemplateCache";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            try
            {
                if (umbraco.UmbracoSettings.UseDistributedCalls)
                {
                    Template t = Template.GetByAlias(itemId.Id);
                    if (t != null)
                    {
                        umbraco.presentation.cache.dispatcher.Refresh(
                        new Guid("DD12B6A0-14B9-46e8-8800-C154F74047C8"),
                        t.Id);
                    }
                }
            }
            catch (Exception ex) {
                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
            }
        }
    }
}