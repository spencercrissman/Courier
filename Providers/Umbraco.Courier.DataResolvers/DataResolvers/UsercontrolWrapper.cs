using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.ItemProviders;

namespace Umbraco.Courier.DataResolvers {
  public class UsercontrolWrapper : ItemDataResolverProvider {

    private Guid m_dataEditor = new Guid("d15e1281-e456-4b24-aa86-1dda3e4299d5");

    public override List<Type> ResolvableTypes {
      get { return new List<Type>() { typeof(DataType) }; }
    }


    public override bool ShouldExecute(Item item, Core.Enums.ItemEvent itemEvent) {
        DataType cpd = (DataType)item;
        return (cpd.DataEditorId == m_dataEditor);
    }


    public override void Packaging(Item item)
    {
        DataType cpd = (DataType)item;

        foreach (var preval in cpd.Prevalues)
        {
            if (preval.Value.ToLower().EndsWith(".ascx"))
                item.Resources.Add( "/" + preval.Value.TrimStart('~').TrimStart('/') );
        }
    }

  }
}