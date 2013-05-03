using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbraco.Courier.RepositoryProviders
{
    public class ProviderIdCollection
    {
		#region Fields (2) 

        //default webservice provider
        public static Guid CourierWebserviceRepositoryProviderGuid = new Guid("e0472596-e73b-11df-9492-0800200c9a66");
        public static Guid NetworkShareRepositoryProviderGuid = new Guid("e0472598-e73b-11df-9492-0800200c9a66");
        public static Guid CompressedNetworkShareRepositoryProviderGuid = new Guid("e0472599-e73b-11df-9492-0800200c9a66");
        public static Guid LocalProviderId = new Guid("e0472598-e73b-11df-9492-0800200c9a67");

        public static Guid LocalProviderWithSettingsId = new Guid("e0472598-e74b-11df-9492-0800200c9a67");

		#endregion Fields 
    }
}