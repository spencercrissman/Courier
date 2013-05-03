<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.ProviderModel;
using System.Text.RegularExpressions;

namespace Umbraco.Courier.DataResolvers.ResourceResolvers
{
    public class TemplateResources : ResourceDataResolverProvider
    {
        string regex = "{locallink:?([^\\}' >]+)}";
        public override bool ShouldExecute(Type itemType, ItemIdentifier itemId, Resource resource, Core.Enums.ItemEvent eventType)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master"))
                return true;

            return false;
        }
        
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Template) }; }
        }       

        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
                var fileContent = ResourceAsString(resource);
                if (fileContent != string.Empty)
                {
                    //macros
                    Helpers.MacroResolver res = new Helpers.MacroResolver();
                    res.RegisterNodeDependencies = false;
                    res.RegisterMacroDependencies = false;
                    fileContent = res.ReplaceMacroElements(fileContent, true, null);

                    //links
                    Helpers.LocalLinkResolver les = new Helpers.LocalLinkResolver();
                    les.RegisterLinksAsDependencies = false;
                    fileContent = les.ReplaceLocalLinks(fileContent, true, null);

                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            
        }


        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
               var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    //macros
                    Helpers.MacroResolver res = new Helpers.MacroResolver();
                    res.RegisterNodeDependencies = false;
                    res.RegisterMacroDependencies = false;
                    fileContent = res.ReplaceMacroElements(fileContent, false, null);

                    //links
                    Helpers.LocalLinkResolver les = new Helpers.LocalLinkResolver();
                    les.RegisterLinksAsDependencies = false;
                    fileContent = les.ReplaceLocalLinks(fileContent, false, null);
                                        
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
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
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.ProviderModel;
using System.Text.RegularExpressions;

namespace Umbraco.Courier.DataResolvers.ResourceResolvers
{
    public class TemplateResources : ResourceDataResolverProvider
    {
        string regex = "{locallink:?([^\\}' >]+)}";
        public override bool ShouldExecute(Type itemType, ItemIdentifier itemId, Resource resource, Core.Enums.ItemEvent eventType)
        {
            if (itemType == typeof(Template) && resource.PackageFromPath.ToLower().EndsWith(".master")  )
                return true;

            return false;
        }
        
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Template) }; }
        }       

        public override void PackagedResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
                var fileContent = ResourceAsString(resource);
                if (fileContent != string.Empty)
                {
                    //macros
                    Helpers.MacroResolver res = new Helpers.MacroResolver();
                    res.RegisterNodeDependencies = false;
                    res.RegisterMacroDependencies = false;
                    res.context = this.ExecutionContext;

                    fileContent = res.ReplaceMacroElements(fileContent, true, null);

                    //links
                    Helpers.LocalLinkResolver les = new Helpers.LocalLinkResolver();
                    les.RegisterLinksAsDependencies = false;
                    fileContent = les.ReplaceLocalLinks(fileContent, true, null);

                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
                }
            
        }


        public override void ExtractingResource(Type itemType, ItemIdentifier itemId, Resource resource)
        {
               var fileContent = ResourceAsString(resource);

                if (fileContent != string.Empty)
                {
                    //macros
                    Helpers.MacroResolver res = new Helpers.MacroResolver();
                    res.RegisterNodeDependencies = false;
                    res.RegisterMacroDependencies = false;
                    fileContent = res.ReplaceMacroElements(fileContent, false, null);

                    //links
                    Helpers.LocalLinkResolver les = new Helpers.LocalLinkResolver();
                    les.RegisterLinksAsDependencies = false;
                    fileContent = les.ReplaceLocalLinks(fileContent, false, null);
                                        
                    resource.ResourceContents = Core.Settings.Encoding.GetBytes(fileContent);
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
>>>>>>> 2.7.7 merge error update
}