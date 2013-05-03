using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.Helpers;
using System.Web.Caching;

namespace Umbraco.Courier.RepositoryProviders.WebserviceProvider.Security
{
    public class Authentication
    {
        private static object _syncLock = new object();

        #region Methods (4)
        
        public static void AuthorizeClient(string login, string password, bool cacheAuthResult)
        {
            string ip = HttpContext.Current.Request.UserHostAddress;
            string key = "courier_" + Encryption.Encrypt(login + "_" + password + "_" + ip);
            string error = "";
            
            if (!cacheAuthResult || HttpRuntime.Cache.Get(key) == null)
            {
                SecurityHelper.ValidateIP(ip);
                SecurityHelper.ValidateUser(login, password);
                
                //if we get to this part of the code, there has been no exceptions and we can cache the authentication token
                if (cacheAuthResult)
                    HttpRuntime.Cache.Add(key, true, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), CacheItemPriority.High, null);
            }
        }
                

        #endregion Methods
    }
}