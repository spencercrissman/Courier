using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.ItemProviders;
using Umbraco.Courier.Core;

namespace CompanyResolver
{
    public class CompanyPickerDataResolver : Umbraco.Courier.Core.ItemDataResolverProvider
    {

        //This class spots document property data, which contains properties with the alias "company" (or what you specify in the courier.config)
        //it then simply adds a "company" as a dependency to the property data

        private List<string> companyProperties = new List<string>();
        public CompanyPickerDataResolver()
        {
                companyProperties = Context.Current.Settings.GetConfigurationCollection("/configuration/itemDataResolvers/companyPickers/add", true);
        }        

        public override List<Type> ResolvableTypes
        {
            get { return new List<Type>() { typeof(ContentPropertyData) }; }
        }

        //Determines if the resolver should trigger anything, it checks the event, the type of provider triggereing it
        //and checks if the item has any properties that matches the aliases
        public override bool ShouldExecute(Umbraco.Courier.Core.Item item, Umbraco.Courier.Core.Enums.ItemEvent itemEvent)
        {
            if(item.GetType() == typeof(ContentPropertyData) && itemEvent == Umbraco.Courier.Core.Enums.ItemEvent.Packaging)
            {
                ContentPropertyData cpd = (ContentPropertyData)item;
                return (cpd.Data.Where(x => companyProperties.Contains(x.Alias)).Count() > 0);
            }
            return false;
        }


        //Performs the dependency attachment as part of the packaging
        public override void Packaging(Item item)
        {
            ContentPropertyData cpd = (ContentPropertyData)item;
            
            foreach(var prop in cpd.Data.Where(x => companyProperties.Contains(x.Alias))){
                if (prop.Value != null)
                    item.Dependencies.Add(new Dependency(prop.Value.ToString(), Constants.companyProviderID));
            }

        }
    }
}