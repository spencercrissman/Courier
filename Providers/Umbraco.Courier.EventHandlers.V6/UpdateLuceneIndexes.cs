using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;
using Examine;
using Umbraco.Courier.ItemProviders;
using UmbracoExamine;
using Umbraco.Courier.Core.Diagnostics.Logging;

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

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            if (itemId != null)
            {
                Guid g;
                if (Guid.TryParse(itemId.Id, out g))
                {
                    try
                    {

                        if (itemId.ProviderId == ProviderIDCollection.mediaItemProviderGuid)
                            ReindexMedia(g, itemId);
                        else if (itemId.ProviderId == ProviderIDCollection.documentItemProviderGuid)
                            ReindexContent(g, itemId);

                    }
                    catch (Exception ex)
                    {
                        RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "UpdateLuceneIndexes", ex.ToString(), LogItemEntryType.Error);
                    }
                }
            }
        }

        private void ReindexContent(Guid contentGuid, ItemIdentifier itemId)
        {
            umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(contentGuid);
            if (d != null)
            {
                XmlNode n = d.ToXml(new XmlDocument(), true);
                if (n != null)
                {
                    XElement docXnode = XDocument.Parse(n.OuterXml).Root;
                    if (d.Published)
                    {
                        //only if published should it be added to indexes which doesnt support published content
                        try
                        {
                            ExamineManager.Instance.ReIndexNode(docXnode, IndexTypes.Content,
                               ExamineManager.Instance.IndexProviderCollection.OfType<BaseUmbracoIndexer>()
                                   .Where(x => x.EnableDefaultEventHandler));
                        }
                        catch (Exception ex)
                        {
                            RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "UpdateLuceneIndexes", ex.ToString(), LogItemEntryType.Error);
                        }
                    }

                    //add to all indexes supporting unpublished content
                    try
                    {
                        ExamineManager.Instance.ReIndexNode(docXnode, IndexTypes.Content,
                           ExamineManager.Instance.IndexProviderCollection.OfType<BaseUmbracoIndexer>()
                               .Where(x => x.SupportUnpublishedContent
                                   && x.EnableDefaultEventHandler));
                    }
                    catch (Exception ex)
                    {
                        RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "UpdateLuceneIndexes", ex.ToString(), LogItemEntryType.Error);
                    }
                }
            }
        }

        private void ReindexMedia(Guid mediaGuid, ItemIdentifier itemId)
        {
            umbraco.cms.businesslogic.media.Media m = new umbraco.cms.businesslogic.media.Media(mediaGuid);
            if (m != null)
            {
                XmlNode n = m.ToXml(new XmlDocument(), true);
                if (n != null)
                {
                    XElement mediaXnode = XDocument.Parse(n.OuterXml).Root;
                    //add to all indexes supporting unpublished content
                    try
                    {
                        ExamineManager.Instance.ReIndexNode(mediaXnode, IndexTypes.Media,
                           ExamineManager.Instance.IndexProviderCollection.OfType<BaseUmbracoIndexer>()
                               .Where(x => x.EnableDefaultEventHandler));
                    }
                    catch (Exception ex)
                    {
                        RevisionLog.Instance.AddItemEntry(itemId, this.GetType(), "UpdateLuceneIndexes", ex.ToString(), LogItemEntryType.Error);
                    }
                }
            }

        }    
    }
}