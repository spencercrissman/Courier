using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers
{
    public class Upload : ItemDataResolverProvider
    {
        private Dictionary<string, string> uploadDataTypes;

        public Upload()
        {
            uploadDataTypes = Context.Current.Settings.GetConfigurationKeyValueCollection("/configuration/itemDataResolvers/files/add", true);
        }

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }

        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            foreach (var data in cpd.Data)
                if (uploadDataTypes.ContainsValue(data.DataTypeEditor.ToString().ToLower()))
                    return true;

            return false;
        }


        public override void Packaging(Item item)
        {
            try
            {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data)
                {
                    if (cp.Value != null && uploadDataTypes.Values.Contains(cp.DataTypeEditor.ToString()))
                    {
                        string file = cp.Value.ToString();
                        string dir = IO.DirectoryPart(file);
                        
                        if (!string.IsNullOrEmpty(file))
                        {
                            if (IO.FileExists(file))
                            {
                                var fi = new FileInfo(Umbraco.Courier.Core.Context.Current.MapPath(file));
                                string ext = fi.Extension.ToLower().Trim('.');

                                //add file as a resource
                                item.Resources.Add(file);

                                if (umbraco.UmbracoSettings.ImageFileTypes.ToLower().Contains(ext))
                                {
                                    string name = fi.Name.Substring(0, (fi.Name.LastIndexOf('.')));
                                    

                                    foreach (VirtualFile img in HostingEnvironment.VirtualPathProvider.GetDirectory(dir).Files)
                                    {
                                        //it's not the same file, but has the same start, hence it's a thumbnail
                                        if (img.Name != fi.Name && img.Name.StartsWith(name))
                                        {
                                            string relPath = dir + img.Name;;

                                            //add file as a resource
                                            item.Resources.Add(relPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging._Debug(ex.ToString());
            }
        }
    }
}