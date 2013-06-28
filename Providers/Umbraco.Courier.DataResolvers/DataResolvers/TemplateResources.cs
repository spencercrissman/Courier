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
            return true;
        }

        public override void Packaging(Item item)
        {
            Template template = (Template)item;
            string design = template.Design;


            if (!string.IsNullOrEmpty(design))
            {
                //settings to decide which items are included as dependencies, note it will always convert
                var settings = this.ExecutionContext.SettingsManager.GetItemProviderConfigurationKeys("templateItemProvider");

                var processTemplateRes = (settings.ContainsKey("processTemplateResources") && settings["processTemplateResources"] == "false") ? false : true;
                var registerMacrosNodes = (settings.ContainsKey("macroParametersAreDependencies") && settings["macroParametersAreDependencies"] == "false") ? false : true;
                var registeLocalLinks = (settings.ContainsKey("localLinksAreDependencies") && settings["localLinksAreDependencies"] == "false") ? false : true;
                var registerMacros = (settings.ContainsKey("macrosAreDependencies") && settings["macrosAreDependencies"] == "false") ? false : true;


                if (processTemplateRes)
                {
                    //Detecting js files in templates
                    foreach (
                        string jsFile in Umbraco.Courier.Core.Helpers.Dependencies.ReferencedScriptsInstring(design))
                    {
                        //TODO make this better to detect outside urls
                        if (!jsFile.Contains("://"))
                        {
                            if (System.IO.File.Exists(Core.Context.Current.MapPath(jsFile)))
                                item.Resources.Add(jsFile);
                        }
                    }

                    //detecting images in templates
                    foreach (
                        string imgFile in
                            Umbraco.Courier.Core.Helpers.Dependencies.ReferencedImageFilessInstring(design))
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
                    foreach (
                        string cssFile in
                            Umbraco.Courier.Core.Helpers.Dependencies.ReferencedStyleSheetsInstring(design))
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
                                    string name = path.Substring(cssFolderPath.Length).Replace(".css", "").Trim('\\',
                                                                                                                '/');
                                    item.Dependencies.Add(new Dependency("Stylesheet: " + name, name.ToLower(),
                                                                         ItemProviders.ProviderIDCollection.
                                                                             stylesheetItemProviderGuid));
                                }
                                else
                                {
                                    string csscontent = System.IO.File.ReadAllText(Core.Context.Current.MapPath(cssFile));
                                    List<string> cssImages =
                                        Umbraco.Courier.Core.Helpers.Dependencies.FilesInCss(csscontent);

                                    if (cssImages.Count > 0)
                                    {
                                        string dir = "/";

                                        if (cssFile.Contains("/"))
                                            dir = "/" + cssFile.Substring(0, cssFile.LastIndexOf('/') + 1).Trim('/') +
                                                  "/";

                                        foreach (string file in cssImages)
                                            item.Resources.Add(dir + file.Trim('/'));
                                    }

                                    item.Resources.Add(cssFile);
                                }
                            }

                        }
                    }
                }

                    //detecting macros and their dependencies in the template markup
                    Helpers.MacroResolver res = new Helpers.MacroResolver();
                    res.RegisterMacroDependencies = registerMacros;
                    res.RegisterNodeDependencies = registerMacrosNodes;
                    design = res.ReplaceMacroElements(design, true, item);

                    //detect locallinks and register as dependencies
                    Helpers.LocalLinkResolver linkRes = new Helpers.LocalLinkResolver();
                    linkRes.RegisterLinksAsDependencies = registeLocalLinks;
                    design = linkRes.ReplaceLocalLinks(design, true, item);
                    template.Design = design;
            }
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