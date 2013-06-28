using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using System.Text.RegularExpressions;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class LocalLinks : ItemDataResolverProvider
    {
        private static List<string> localLinkDataTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/localLinks/add", true);
        
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }
        
        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {  
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data.Where( X=> X.Value != null && localLinkDataTypes.Contains( X.DataTypeEditor.ToString().ToLower()) ))
                if (cp.Value != null && cp.Value.ToString().IndexOf("{locallink:", StringComparison.OrdinalIgnoreCase)>=0)
                     return true;                
            
            return false;
        }

        public override void Packaging(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data.Where(X => X.Value != null && localLinkDataTypes.Contains(X.DataTypeEditor.ToString().ToLower())))
            {
                if (cp.Value != null && cp.Value.ToString().IndexOf("{locallink:", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Helpers.LocalLinkResolver res = new Helpers.LocalLinkResolver();
                    res.RegisterLinksAsDependencies = true;
                    cp.Value = res.ReplaceLocalLinks(cp.Value.ToString(), true, item);
                }
            }
        }

        public override void Extracting(Item item)
        {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data.Where( X=> X.Value != null && localLinkDataTypes.Contains( X.DataTypeEditor.ToString().ToLower()) ))
                {    
                    if (cp.Value != null && cp.Value.ToString().IndexOf("{locallink:", StringComparison.OrdinalIgnoreCase)>=0)
                    {
                        Helpers.LocalLinkResolver res = new Helpers.LocalLinkResolver();
                        cp.Value = res.ReplaceLocalLinks(cp.Value.ToString(), false, item);
                    }
                }
        }
                
       
    }
}