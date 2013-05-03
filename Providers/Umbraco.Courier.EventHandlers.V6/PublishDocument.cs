using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.Core;
using System.Xml;
using System.Xml.Linq;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class PublishDocument : ItemEventProvider {

        public override string Alias {
            get {
                return "PublishDocument";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            Guid g;
            if (Guid.TryParse(itemId.Id, out g))
            {

                try
                {
                    umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(g);
                    if (d != null && d.Published && d.ReleaseDate < DateTime.Now)
                    {
                        d.XmlGenerate(new XmlDocument());
                        umbraco.library.UpdateDocumentCache(d.Id);
                    }
                }
                catch (Exception ex)
                {
                    Core.Helpers.Logging._Debug(ex.ToString());
                }
            }
        }
        /*
        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters) {
            Guid g;
            if(Guid.TryParse( itemId.Id, out g )){

                try
                {
                    ContentService cs = new ContentService();
                    var d = cs.GetById(g);
                   
                    // if (d != null && (d.ReleaseDate == null || d.ReleaseDate < DateTime.Now))
                   //     cs.Publish(d);
                }
                catch (Exception ex) {
                    Core.Helpers.Logging._Error(ex.ToString());
                }
            }        
        }*/
    }
}