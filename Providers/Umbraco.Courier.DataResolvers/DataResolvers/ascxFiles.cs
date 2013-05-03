using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core.Helpers;
using Umbraco.Courier.Core.Diagnostics.Logging;

namespace Umbraco.Courier.DataResolvers
{
    public class ascxFiles : ItemDataResolverProvider
    {
        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(Macro), typeof(DataType) }; }
        }


        public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent)
        {
            return (itemEvent == Core.Enums.ItemEvent.Packaging && item.Resources.Where(x => x.PackageFromPath.EndsWith(".ascx")).FirstOrDefault() != null);
        }

        public override void Packaging(Item item)
        {
            List<string> _temp = new List<string>();
            foreach (var file in item.Resources.Where(x => x.PackageFromPath.ToLower().EndsWith(".ascx")))
            {
                string ascx = file.PackageFromPath;
                string ascx_mapped = Context.Current.MapPath(ascx);

                //item.Resources.Add(ascx);

                try
                {
                    if (System.IO.File.Exists(ascx_mapped + ".cs"))
                        _temp.Add(ascx + ".cs");

                    if (System.IO.File.Exists(ascx_mapped + ".designer.cs"))
                        _temp.Add(ascx + ".designer.cs");


                    System.Web.UI.Page p = new System.Web.UI.Page();

                    if (p != null)
                    {
                        var c = p.LoadControl(ascx);

                        if (c != null)
                        {
                            string ass = c.GetType().Assembly.GetName().Name;
                            string dll = "~/bin/" + ass + ".dll";

                            if (System.IO.File.Exists(Context.Current.MapPath(dll)))
                                _temp.Add(dll);
                        }
                    }
                }
                catch (Exception ex)
                {
                    RevisionLog.Instance.Error(item, this, RevisionLog.ItemDataResolvers, ex.ToString());
                }
            }
                        
            foreach (var s in _temp)
               item.Resources.Add(s);
        }
    }
}