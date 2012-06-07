using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;
using System.Text.RegularExpressions;

namespace Umbraco.Courier.DataResolvers
{
    public class TemplateResources : ItemDataResolverProvider
    {
        string regex = "{locallink:?([^\\}' >]+)}";
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Template) }; }
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            if (itemEvent == Core.Enums.ItemEvent.Packaging && item.GetType() == typeof(Template))
                return true;
            else
                return false;
        }

        /*
        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
            {
                var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    List<string> ids;
                    fileContent = replaceIDWithGuid(fileContent, out ids);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            }
        }


        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
            {
                var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    fileContent = replaceGuidWithID(fileContent);
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            }
        }
        */

        public override void Packaging(Item item)
        {
            Template template = (Template)item;
            
            string templateFile = Core.Context.Current.MapPath(template.MasterPagePath);
            string design = "";

            if (System.IO.File.Exists(templateFile)){
                design = System.IO.File.ReadAllText(Core.Context.Current.MapPath(template.MasterPagePath));
                
            //Detecting js files in templates
            foreach (string jsFile in Umbraco.Courier.Core.Helpers.Dependencies.ReferencedScriptsInstring(design))
            {
                //TODO make this better to detect outside urls
                if (!jsFile.Contains("://"))
                {
                    if (System.IO.File.Exists(Core.Context.Current.MapPath(jsFile)))
                        item.Resources.Add(jsFile);
                }
            }


            //detecting images in templates
            foreach (string imgFile in Umbraco.Courier.Core.Helpers.Dependencies.ReferencedImageFilessInstring(design))
            {
                string s = imgFile;

                if (!s.Contains("://"))
                {
                    if (!s.StartsWith("/"))
                        s = "/" + s;

                    if (System.IO.File.Exists(Core.Context.Current.MapPath(s)))
                        item.Resources.Add(s);
                }
            }

            
            //Detecting css files in templates
            foreach (string cssFile in Umbraco.Courier.Core.Helpers.Dependencies.ReferencedStyleSheetsInstring(design))
            {
                var css = cssFile;

                //TODO improve handling of remote urls
                if (!css.Contains("://"))
                {
                    css = "/" + css.Trim('/', '~', '\\');
                    
                    string path = Core.Context.Current.MapPath(cssFile).ToLower();
                    string cssFolderPath = Core.Context.Current.MapPath("/css").ToLower();
                    
                    //lets test if the file actually exists so we don't try to transfer a dynamicUrl
                    if (System.IO.File.Exists(path))
                    {
                        if (path.StartsWith(cssFolderPath))
                        {
                            string name = path.Substring(cssFolderPath.Length).Replace(".css", "").Trim('\\','/');
                            item.Dependencies.Add(new Dependency("Stylesheet: " + name, name.ToLower(), ItemProviders.ProviderIDCollection.stylesheetItemProviderGuid));
                        }
                        else
                        {
                       
                            string csscontent = System.IO.File.ReadAllText(Core.Context.Current.MapPath(cssFile));
                            List<string> cssImages = Umbraco.Courier.Core.Helpers.Dependencies.FilesInCss(csscontent);
                       
                            if (cssImages.Count > 0)
                            {
                                string dir = "/";

                                if (cssFile.Contains("/"))
                                    dir = "/" + cssFile.Substring(0, cssFile.LastIndexOf('/') + 1).Trim('/') + "/";

                                foreach (string file in cssImages)
                                    item.Resources.Add(dir + file.Trim('/'));
                            }

                            item.Resources.Add(cssFile);
                        }
                    }

                }
            }


            //Detecting nodes in template markups, through locallink: syntax
            var sdad = Regex.Replace(design, regex, delegate(Match match)
            {
                string id = match.Groups[1].Value;
                string tag = match.ToString();

                int nodeId;

                if (int.TryParse(id, out nodeId))
                {
                    Guid nodeGuid = PersistenceManager.Default.GetUniqueId(nodeId);
                    if (nodeGuid != Guid.Empty)
                        item.Dependencies.Add(new ItemIdentifier(nodeGuid.ToString(), ItemProviders.ProviderIDCollection.documentItemProviderGuid));
                }
                return tag;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);




            }

            
        }



        private string replaceGuidWithID(string val)
        {

            val = Regex.Replace(val, regex, delegate(Match match)
            {

                string guid = match.Groups[1].Value;
                string tag = match.ToString();


                Guid nodeGuid;

                if (Guid.TryParse(guid, out nodeGuid))
                {
                    int nodeId = PersistenceManager.Default.GetNodeId(nodeGuid);
                    if (nodeId != 0)
                        tag = tag.Replace(guid, nodeId.ToString());
                }

                return tag;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            return val;
        }

        //replace
        private string replaceIDWithGuid(string val, out List<string> ids)
        {
            List<string> idsFound = new List<string>();
            
            val = Regex.Replace(val, regex, delegate(Match match)
            {
                string id = match.Groups[1].Value;
                string tag = match.ToString();

                int nodeId;

                if (int.TryParse(id, out nodeId))
                {
                    Guid nodeGuid = PersistenceManager.Default.GetUniqueId(nodeId);
                    if (nodeGuid != Guid.Empty)
                    {
                        idsFound.Add(nodeGuid.ToString());
                        tag = tag.Replace(id, nodeGuid.ToString());
                    }
                }
                return tag;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            ids = idsFound;

            return val;
        }

        private string ResourceAsString(Resource r)
        {
            var bytes = r.ResourceContents;
            string fileContent = string.Empty;

            if (bytes == null || bytes.Length <= 0)
            {
                string p = Context.Current.MapPath(r.PackageFromPath);
                if (System.IO.File.Exists(p))
                {
                    bytes = System.IO.File.ReadAllBytes(p);
                    fileContent = Core.Settings.Encoding.GetString(bytes);
                }
            }
            else
                fileContent = Core.Settings.Encoding.GetString(bytes);


            return fileContent;
        }
    }
}