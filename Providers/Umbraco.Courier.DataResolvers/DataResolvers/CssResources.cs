using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;

namespace Umbraco.Courier.DataResolvers
{
    public class CssResources: ItemDataResolverProvider
    {
        
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Stylesheet) }; }
        }
        
        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            return true;
        }

        public override void Packaging(Item item)
        {
            Stylesheet cssItem = (Stylesheet)item;

            string css = cssItem.Content;
            List<string> cssImages = Umbraco.Courier.Core.Helpers.Dependencies.FilesInCss(css);

            if (cssImages.Count > 0)
            {
                string dir = "/css/";

                foreach (string file in cssImages)
                {
                    if (file.StartsWith("/"))
                        item.Resources.Add(file);
                    else
                        item.Resources.Add(dir + file.Trim('/'));
                }
            }
        }

    }
}