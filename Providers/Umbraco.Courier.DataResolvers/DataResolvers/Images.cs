using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers {
    public class Images : ItemDataResolverProvider {
        private static List<string> macroDataTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/macros/add", true);
       //private static List<string> macroPropertyTypes = Context.Current.Settings.GetConfigurationCollection("/configuration/macroPropertyTypeResolvers/contentPickers/add", true);


        public override List<Type> ResolvableTypes {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }
        
        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent) {

            if (item.GetType() == typeof(ContentPropertyData)) {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var cp in cpd.Data)
                    if (cp.Value != null && macroDataTypes.Contains(cp.DataTypeEditor.ToString().ToLower()))
                        return true;
            }
            return false;
        }

        
        public override void Packaging(Item item) {

            if (item.GetType() == typeof(ContentPropertyData)) {
                ContentPropertyData cpd = (ContentPropertyData)item;
                foreach (var prop in cpd.Data.Where(x => x.Value != null && macroDataTypes.Contains(x.DataTypeEditor.ToString().ToLower()))) {
                    FindResoucesInString(prop.Value.ToString(), item);
                }
            }
        }
        
        private void FindResoucesInString(string str, Item item) {
            List<string> imgs = Umbraco.Courier.Core.Helpers.Dependencies.ReferencedImageFilessInstring(str);
            imgs.AddRange(Umbraco.Courier.Core.Helpers.Dependencies.ReferencedPagessInstring(str));

            string mediaRoot = umbraco.IO.SystemDirectories.Media.TrimStart('~').ToLower();
            
            foreach (var img in imgs)
            {

                var path = img.ToLower().TrimStart('~');

                if (path.StartsWith("http://") || path.StartsWith("https://"))
                    continue;

                if (path.StartsWith(mediaRoot))
                {
                    string imagePath = img.TrimStart('~');

                    if (imagePath.Contains('_'))
                        imagePath = imagePath.Substring(0, imagePath.LastIndexOf('_'));
                    
                    item.Dependencies.Add(imagePath, ProviderIDCollection.mediaItemProviderGuid);
                }
                else
                {
                    //dont add local links to resources
                    if (!img.Contains('{'))
                    {
                        item.Resources.Add(img);
                    }
                }
            }
        }

    }

}