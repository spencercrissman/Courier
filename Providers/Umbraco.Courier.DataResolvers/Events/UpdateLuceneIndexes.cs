using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Courier.Core.ProviderModel;
using Examine;
using UmbracoExamine;

namespace Umbraco.Courier.DataResolvers.Events
{
    public class UpdateLuceneIndexes : ItemEventProvider
    {
        public override string Alias
        {
            get
            {
                return "UpdateLuceneIndexes";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters) {
            Guid g;
            if(Guid.TryParse( itemId.Id, out g )){

                try
                {
                    umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(g);

                    if (d != null)
                    {
                        XElement docXnode = XDocument.Parse(d.ToXml(new XmlDocument(), true).OuterXml).Root;
                        if (d.Published)
                        {
                            try
                            {
                                ExamineManager.Instance.ReIndexNode(docXnode, IndexTypes.Content,
                                   ExamineManager.Instance.IndexProviderCollection
                                       .Where(x => x.EnableDefaultEventHandler));
                            }
                            catch (Exception ex) {
                                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
                            }
                        }

                        try
                        {
                            ExamineManager.Instance.ReIndexNode(docXnode, IndexTypes.Content,
                               ExamineManager.Instance.IndexProviderCollection
                                   .Where(x => x.SupportUnpublishedContent
                                       && x.EnableDefaultEventHandler));
                        }
                        catch (Exception ex) {
                            Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
                        }

                    }
                }
                catch (Exception ex) {
                    Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
                }
            }        
        }
    }
}