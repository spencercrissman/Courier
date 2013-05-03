using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.DataResolvers;

namespace Umbraco.Courier.uComponents
{
    public class ImageRadioButtonList : PropertyDataResolverProvider
    {
        public override Guid DataTypeId
        {
            get { return new Guid("3b369027-a116-4fba-b1f4-718b65c8665f"); }
        }

        public override void PackagingDataType(ItemProviders.DataType item)
        {
            //we go through the settings and save references to images stored in the datatype
            foreach (var setting in item.Prevalues.Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.Contains("|")))
            {
                var currentSetting = setting.Value.Split('|');

                if (currentSetting.Length > 1)
                {
                    var alias = currentSetting[0];
                    var file = currentSetting[1];

                    item.Resources.Add(file);
                }
            }
        }

    }
}