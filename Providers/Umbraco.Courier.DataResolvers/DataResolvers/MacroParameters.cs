using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Diagnostics.Logging;

namespace Umbraco.Courier.DataResolvers
{
    public class MacroParameters : ItemDataResolverProvider
    {
        private static List<string> macroDataTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/macros/add", true);
        private static List<string> macroPropertyTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/macroPropertyTypeResolvers/contentPickers/add", true);
      
        private static string elementRegex = string.Format("(</?{0}((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>)", Regex.Escape("umbraco:macro"));
        private static string oldelementRegex = string.Format("(</?{0}((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>)", Regex.Escape("?UMBRACO_MACRO"));
        private static string attrRegex = @"(.*?)=\""(.*?)\""";
        
        public MacroParameters()
        {
            AllowAsync = false;    
        }        

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>(){ typeof(ContentPropertyData)}; }
        }        

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var cp in cpd.Data.Where(x => macroDataTypes.Contains(x.DataTypeEditor.ToString(), StringComparer.OrdinalIgnoreCase)))
                if (
                        cp.Value != null 
                        &&
                        (cp.Value.ToString().IndexOf("umbraco:macro", StringComparison.OrdinalIgnoreCase)>0 
                        ||
                        cp.Value.ToString().IndexOf("UMBRACO_MACRO", StringComparison.OrdinalIgnoreCase)>0)
                    )
                    return true;
            

            return false;
        }             
        
        public override void Packaging(Item item) {
            if(item.GetType() == typeof(ContentPropertyData))
            {
                ContentPropertyData cpd = (ContentPropertyData)item;

                foreach (var prop in cpd.Data.Where(x => x.Value != null && macroDataTypes.Contains(x.DataTypeEditor.ToString().ToLower())))
                {
                    string templateText = prop.Value.ToString();

                    Helpers.MacroResolver resolver = new Helpers.MacroResolver();
                    resolver.RegisterMacroDependencies = true;
                    resolver.RegisterNodeDependencies = true;
                    resolver.context = this.ExecutionContext;

                    if(templateText.IndexOf("umbraco:macro", StringComparison.OrdinalIgnoreCase)>0)
                        templateText = resolver.ReplaceMacroElements(templateText, true, item);

                    if (templateText.IndexOf("UMBRACO_MACRO", StringComparison.OrdinalIgnoreCase) > 0)
                        templateText = resolver.ReplaceOldMacroElements(templateText, true, item);

                    prop.Value = templateText;
                }
            }
        }
        
        public override void Extracting(Item item) {

            if (item.GetType() == typeof(ContentPropertyData)) {
                ContentPropertyData cpd = (ContentPropertyData)item;

                foreach (var prop in cpd.Data.Where(x => x.Value != null && macroDataTypes.Contains(x.DataTypeEditor.ToString().ToLower()))) {

                    string templateText = prop.Value.ToString();
                    
                    Helpers.MacroResolver resolver = new Helpers.MacroResolver();
                    resolver.RegisterMacroDependencies = true;
                    resolver.RegisterNodeDependencies = true;
                    resolver.context = this.ExecutionContext;

                    if (templateText.IndexOf("umbraco:macro", StringComparison.OrdinalIgnoreCase) > 0)
                        templateText = resolver.ReplaceMacroElements(templateText, false, item);

                    if (templateText.IndexOf("UMBRACO_MACRO", StringComparison.OrdinalIgnoreCase) > 0)
                        templateText = resolver.ReplaceOldMacroElements(templateText, false, item);

                    prop.Value = templateText;
                }

            }
        }
    }

}