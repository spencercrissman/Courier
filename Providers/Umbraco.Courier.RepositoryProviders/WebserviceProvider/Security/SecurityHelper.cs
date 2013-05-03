using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core.Configuration;
using System.Text.RegularExpressions;
using System.Web.Security;
using umbraco.BusinessLogic;


namespace Umbraco.Courier.RepositoryProviders.WebserviceProvider.Security
{
    public class SecurityHelper
    {   
        #region Methods (2)

        // Public Methods (2) 
        public static bool AuthorizeIp(string IP)
        {
            if (Umbraco.Courier.Core.Configuration.Security.AllowAllIPs)
                return true;
            else
            {
                if (Umbraco.Courier.Core.Configuration.Security.DeniedIPs.Contains(IP))
                    return false;

                foreach (string s in Umbraco.Courier.Core.Configuration.Security.DeniedIPs)
                {
                    Regex rx = new Regex(WildcardToRegex(s));
                    if (rx.Match(IP).Success)
                        return false;
                }

                if (Umbraco.Courier.Core.Configuration.Security.AllowedIPs.Contains("*"))
                   return true;

                if (Umbraco.Courier.Core.Configuration.Security.AllowedIPs.Contains(IP))
                    return true;

                foreach (string s in Umbraco.Courier.Core.Configuration.Security.AllowedIPs)
                {
                    Regex rx = new Regex(WildcardToRegex(s));
                    if (rx.Match(IP).Success)
                        return true;
                }
            }


            return Umbraco.Courier.Core.Configuration.Security.AllowAllIPs;
        }


        public static void ValidateIP(string IP)
        {
            if (!Umbraco.Courier.Core.Configuration.Security.AllowAllIPs){
                
                var denyList =  Umbraco.Courier.Core.Configuration.Security.DeniedIPs;
                var acceptList = Umbraco.Courier.Core.Configuration.Security.AllowedIPs;

                if(acceptList.Count == 0)
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("Courier is configured to not allow any outside IPs, including: " + IP);
                
                //Black listing:
                if(denyList.Count > 0){
                    if (denyList.Contains(IP))
                        throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("IP: " + IP + " Is in the denied IP List. Rules: " + string.Join(",", denyList.ToArray()) );
                
                    foreach (string s in denyList)
                    {
                        Regex rx = new Regex(WildcardToRegex(s));
                        if (rx.Match(IP).Success)
                            throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("IP: " + IP + " is filtered with the rule: '" + s + "' in the denied IP list. All rules: " + string.Join(",", denyList.ToArray()));
                    }
                }

                //White listing:
                bool whitelisted = acceptList.Contains(IP);
                if(!whitelisted){
                        foreach (string s in acceptList)
                        {
                            Regex rx = new Regex(WildcardToRegex(s));
                            if (rx.Match(IP).Success)
                                whitelisted = true;
                        }                    
                }

                if(!whitelisted)
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("IP: " + IP + " is not the allowed IP list, and your application does not allow all IPs to connect");
            }
            
        }

        public static void ValidateUser(string login, string pass)
        {
            umbraco.BusinessLogic.User u = null;
            if (umbraco.UmbracoSettings.DefaultBackofficeProvider == "UsersMembershipProvider")
            {
                u = new User(login);
                if(u!= null && pass != u.GetPassword())
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " could not be authenticated");
            }
            else
            {
                if (Membership.Providers[umbraco.UmbracoSettings.DefaultBackofficeProvider].ValidateUser(login, pass))
                    u = new User(login);
                else
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " could not be authenticated");
            }

            if(u == null)
                throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " does not exists");
            
                if (u.Disabled)
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " is not enabled");

                // CLN: Can not compare passwords from membership providers  -- Check is done
                //if (u.GetPassword() != pass)
                //    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " and password: xxx does not match");

                if (!Umbraco.Courier.Core.Configuration.Security.AllowAllUsers && Umbraco.Courier.Core.Configuration.Security.DeniedUsers.Contains(u.LoginName))
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " does not have access to courier");

                if (u.Applications.Where(x => x.alias.ToLower() == "courier").Count() == 0)
                    throw new Umbraco.Courier.Core.Exceptions.UnauthorizedClientException("User: " + login + " does not have access to courier.");
        }


        private static bool _AuthorizeUser(string login, string pass)
        {
            var user = umbraco.BusinessLogic.User.GetAllByLoginName(login, false).FirstOrDefault();
            
            if (user == null)
                return false;

            if (user.Disabled)
                return false;

            if (user.GetPassword() != pass)
                return false;

            if (!Umbraco.Courier.Core.Configuration.Security.AllowAllUsers && Umbraco.Courier.Core.Configuration.Security.DeniedUsers.Contains(user.LoginName))
                return false;

            if (user.Applications.Where(x => x.alias.ToLower() == "courier").Count() == 0)
                return false;

            return true;
        }


        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
            Replace("\\*", ".*").
            Replace("\\?", ".") + "$";
        }

        #endregion Methods
    }
}