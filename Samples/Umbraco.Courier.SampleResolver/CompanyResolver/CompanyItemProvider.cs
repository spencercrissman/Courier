using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;

namespace CompanyResolver
{

    //This is the provider that ties everything together
    //for database calls it uses the CompanyPersister.cs, which is found trough simple reflection

    //For deserilization it uses a built-in deserializer

    //It then exposes available items to courier through "AvailableSystemItems"

    //The provider needs a unique ID, name and an icon


    public class CompanyItemProvider : ItemProvider
    {
        public CompanyItemProvider()
        {
            this.Id = Constants.companyProviderID;
            this.Name = "Company provider";
            this.Description = "Synchronizes items in the companies table";
            this.ExtractionDirectory = "Companies";
            this.ProviderIcon = Constants.providerIcon;
        }

        public override List<SystemItem> AvailableSystemItems()
        {
            return this.DatabasePersistence.AvailableItems<Company>();
        }

        public override Item HandleDeserialize(ItemIdentifier id, byte[] byteArray)
        {
            return Umbraco.Courier.Core.Serialization.Serializer.Deserialize<Company>(byteArray);
        }

        public override Item HandlePack(ItemIdentifier id)
        {
            Company item = this.DatabasePersistence.RetrieveItem<Company>(id);

            //here we could also add dependencies or resources
            //item.Resources.Add("/file/path/to/something.jpg");

            //Dependency dep = new Dependency("Something that needs to be installed before company is" "id", new Guid());
            //item.Dependencies.Add(dep);
            
            return item;    
        }

        public override Item HandleExtract(Item item)
        {
            Company c = (Company)item;
            c = this.DatabasePersistence.PersistItem<Company>(c);
            return c;
        }

    }
}