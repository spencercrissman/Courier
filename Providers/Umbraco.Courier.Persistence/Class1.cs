using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.Courier.Persistence
{
    public class CacheHelper
    {
        public static void ClearInternalCache()
        {
            Umbraco.Core.Persistence.Caching.RuntimeCacheProvider.Current.Clear();
        }

    }
}
