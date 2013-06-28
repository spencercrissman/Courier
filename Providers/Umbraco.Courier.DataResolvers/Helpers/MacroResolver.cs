using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using System.Text.RegularExpressions;
using System.Collections;

namespace Umbraco.Courier.DataResolvers.Helpers
{
    public class MacroResolver
    {
        public static List<string> MacroPropertyTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/macros/add", true);
        public static List<string> MacroContentPickers = Context.Current.Settings.GetConfigurationCollection("/configuration/macroPropertyTypeResolvers/contentPickers/add", true);

        public ExecutionContext context { get; set; }

        private static string macroElementRegex = string.Format("(</?{0}((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>)", Regex.Escape("umbraco:macro"));
        private static string oldMacroElementRegex = string.Format("(</?{0}((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>)", Regex.Escape("?UMBRACO_MACRO"));
        //private static string attrRegex = @"(.*?)=\""(.*?)\""";

        public bool RegisterNodeDependencies { get; set; }
        public bool RegisterMacroDependencies { get; set; }
        
        public string ReplaceMacroElements(string source, bool toUnique, Item item)
        {
            Regex rx = new Regex(macroElementRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            return rx.Replace(source, match => MacroElementEvaluator(match, toUnique, item, false));
        }

        public string ReplaceOldMacroElements(string source, bool toUnique, Item item)
        {
            Regex rx = new Regex(oldMacroElementRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            return rx.Replace(source, match => MacroElementEvaluator(match, toUnique, item, true));
        }

        private string MacroElementEvaluator(Match m, bool toUnique, Item item, bool oldSyntax)
        {
            // Get the matched string.
            var element = m.ToString();
            var alias = "";

            if(oldSyntax)
                alias = getAttributeValue(m.ToString(), "macroAlias");
            else
                alias = getAttributeValue(m.ToString(), "alias");

            if (!string.IsNullOrEmpty(alias))
            {
                if (this.RegisterMacroDependencies)
                    item.Dependencies.Add(alias, ItemProviders.ProviderIDCollection.macroItemProviderGuid);

                if (this.RegisterNodeDependencies)
                {
                    var attributesToReplace = MacroAttributesWithPicker(alias);

                    foreach (var attr in attributesToReplace)
                    {
                        string regex = string.Format("(?<attributeAlias>{0})=\"(?<attributeValue>[^\"]*)\"", attr);
                        Regex rx = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        element = rx.Replace(element, match => MacroAttributeEvaluator(match, toUnique, item));
                    }
                }
            }
            return element;
        }

        private string MacroAttributeEvaluator(Match m, bool toUnique, Item item)
        {
            var value = m.Groups["attributeValue"].Value.ToString();
            var alias = m.Groups["attributeAlias"].Value.ToString();

            if (toUnique){
                int id = 0;
                if (int.TryParse(value, out id))
                {   
                    Tuple<Guid,Guid> reference = PersistenceManager.Default.GetUniqueIdWithType(id);
                    if (reference != null)
                    {
                        value = reference.Item1.ToString();
                        
                        if (this.RegisterNodeDependencies){
                            var provider = ItemProviders.NodeObjectTypes.GetCourierProviderFromNodeObjectType(reference.Item2);

                            if (provider.HasValue)
                                item.Dependencies.Add(value, provider.Value);
                        }
                    }
                }
            }else{
                Guid guid = Guid.Empty;
                if (Guid.TryParse(value, out guid))
                {
                    int id = PersistenceManager.Default.GetNodeId(guid);
                    if (id != 0)
                        value = id.ToString();
                }
            }

            // Get the matched string.
            return alias + "=\"" + value + "\"";
        }
        /*
        public static IList<string> MacrosWithPicker()
        {
            string types = "'" + string.Join("','", MacroContentPickers) + "'";
            string query = @"SELECT distinct cmsMacro.macroAlias
                              FROM cmsMacroProperty
                              inner join cmsMacro on macro = cmsmacro.id
                              inner join cmsMacroPropertyType on macroPropertyType = cmsMacroPropertyType.id
                              where cmsMacroPropertyType.macroPropertyTypeAlias in (" + types + ")";

            return PersistenceManager.Default.Query<string>(query, null);
        }
        */

        public IList<string> MacroAttributesWithPicker(string macroAlias)
        {
            string types = "'" + string.Join("','", MacroContentPickers) + "'";
            string query = @"SELECT macroPropertyAlias
                              FROM cmsMacroProperty
                              inner join cmsMacro on macro = cmsmacro.id
                              inner join cmsMacroPropertyType on macroPropertyType = cmsMacroPropertyType.id
                              where 
                              cmsMacroPropertyType.macroPropertyTypeAlias in (" + types + @")
                              and cmsMacro.macroAlias = '" + macroAlias + "'";

            
            var result = PersistenceManager.Default; 
            result.ExecutionContext = context;
            var r = result.Query<string>(query, null);

            return r;
        }

        private static Hashtable getElementAttributes(String element)
        {
            Hashtable ht = new Hashtable();
            MatchCollection m =
                Regex.Matches(element, "(?<attributeName>\\S*)=\"(?<attributeValue>[^\"]*)\"",
                              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            // fix for issue 14862: return lowercase attributes for case insensitive matching
            foreach (Match attributeSet in m)
                ht.Add(attributeSet.Groups["attributeName"].Value.ToString().ToLower(), attributeSet.Groups["attributeValue"].Value.ToString());

            return ht;
        }

        private static string getAttributeValue(string element, string attribute)
        {
            string attrRegex = string.Format(@"{0}=\""(.*?)\""", attribute);
            Match m2 = Regex.Match(element, attrRegex, RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                return m2.Groups[1].Value;
            }
            else
                return null;
        }
    }
}