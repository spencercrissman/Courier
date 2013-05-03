using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using System.Xml;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Diagnostics.Logging;

namespace Umbraco.Courier.DataResolvers
{
    public class RelatedLinks : ItemDataResolverProvider
    {
        private Guid[] RelatedLinksGuid = new Guid[] 
                                                { new Guid("71b8ad1a-8dc2-425c-b6b8-faa158075e63"), 
                                                  new Guid("c9525f23-7b7a-4dab-baeb-08f957bddd50"),
                                                    new Guid("4848424b-ec2c-45f7-a9aa-712bc703b75e") 
                                                };

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
             return (cpd.Data.Where(x => RelatedLinksGuid.Contains(x.DataTypeEditor)).Count() > 0);
        }


        public override void Packaging(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && RelatedLinksGuid.Contains(cp.DataTypeEditor))
                {
                    string xml = cp.Value.ToString();

                    try
                    {
                        XmlDocument xd = new XmlDocument();
                        xd.LoadXml(xml);
                        bool changed = false;

                        foreach (XmlNode node in xd.SelectNodes("//link [@type ='internal' or @type = 'media']"))
                        {
                            XmlAttribute link = node.Attributes["link"];
                            string type = node.Attributes["type"].Value;

                            if (link != null && !string.IsNullOrEmpty(link.Value))
                            {
                                int nodeId = 0;

                                if (int.TryParse(link.Value, out nodeId))
                                {
                                    Guid nodeGuid = PersistenceManager.Default.GetUniqueId(nodeId);
                                    if (nodeGuid != null)
                                    {
                                        link.Value = nodeGuid.ToString();
                                        var provider = ProviderIDCollection.documentItemProviderGuid;

                                        if (type == "media")
                                            provider = ProviderIDCollection.mediaItemProviderGuid;

                                        item.Dependencies.Add(link.Value, provider);

                                        changed = true;
                                    }
                                }
                            }
                        }

                        if (changed)
                            cp.Value = xd.OuterXml;
                    }
                    catch (Exception ex) {
                        RevisionLog.Instance.Error(item, this, RevisionLog.ItemDataResolvers, ex.ToString());
                    }
                }
            }
        }

        public override void Extracting(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data)
            {
                if (cp.Value != null && RelatedLinksGuid.Contains(cp.DataTypeEditor))
                {
                    string xml = cp.Value.ToString();

                    try
                    {
                        XmlDocument xd = new XmlDocument();
                        xd.LoadXml(xml);
                        bool changed = false;

                        foreach (XmlNode node in xd.SelectNodes("//link [@type ='internal' or @type = 'media']"))
                        {
                            XmlAttribute link = node.Attributes["link"];
                            if (link != null && !string.IsNullOrEmpty(link.Value))
                            {
                                Guid guid;

                                if (Guid.TryParse(link.Value, out guid))
                                {
                                    int nodeID = PersistenceManager.Default.GetNodeId(guid);
                                    if (nodeID > 0)
                                    {
                                        link.Value = nodeID.ToString();
                                        changed = true;
                                    }
                                }
                            }
                        }

                        if (changed)
                            cp.Value = xd.OuterXml;
                    }
                    catch (Exception ex) {
                        RevisionLog.Instance.Error(item, this, RevisionLog.ItemDataResolvers, ex.ToString());
                    }
                }
            }
        }
    }
}