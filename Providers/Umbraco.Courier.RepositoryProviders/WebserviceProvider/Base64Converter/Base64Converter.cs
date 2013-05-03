using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbraco.Courier.RepositoryProviders.WebserviceProvider
{
    public class Base64Converter
    {
        public static string ConvertToBase64(byte[] filecontent)
        {
            if (filecontent == null || filecontent.Length == 0)
                return string.Empty;
            else
                return Convert.ToBase64String(filecontent);
        }

        public static byte[] ConvertFromBase64(string fileAsBase64)
        {
            if (string.IsNullOrEmpty(fileAsBase64))
                return new byte[0];
            else
                return Convert.FromBase64String(fileAsBase64);
        }
    }
}