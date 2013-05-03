using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;
using umbraco.cms.businesslogic.macro;

namespace Umbraco.Courier.DataResolvers.Events
{
    public class FlushMacroCache : ItemEventProvider
    {

        public override string Alias
        {
            get
            {
                return "FlushMacroCache";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
                try
                {
                    if (umbraco.UmbracoSettings.UseDistributedCalls)
                    {
                        Macro m = new Macro(itemId.Id);
                        if (m != null)
                        {
                           umbraco.presentation.cache.dispatcher.Refresh(
                           new Guid("7B1E683C-5F34-43dd-803D-9699EA1E98CA"),
                           m.Id);
                        }
                    }
                }
                catch (Exception ex) {
                    Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
                }
        }
    }
}