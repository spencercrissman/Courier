using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.ProviderModel;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class RefreshContent : ItemEventProvider {

        public override string Alias {
            get {
                return "RefreshCache";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters) {
                    umbraco.library.RefreshContent();    
        }
    }
}