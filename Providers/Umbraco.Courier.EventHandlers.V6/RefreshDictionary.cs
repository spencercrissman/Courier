using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Umbraco.Courier.Core.ExtensionMethods;
using Umbraco.Courier.Core.ProviderModel;

namespace Umbraco.Courier.DataResolvers.ItemEventProviders
{
    public class RefreshDictionary : ItemEventProvider
    {

        public override string Alias
        {
            get
            {
                return "ClearDictionaryCache";
            }
        }

        public override void Execute(Core.ItemIdentifier itemId, Core.SerializableDictionary<string, string> Parameters)
        {
            try
            {
                // Clear Dictionary cache
                var dictionaryStaticType = typeof(umbraco.cms.businesslogic.Dictionary);

                var lockObj = dictionaryStaticType.GetField("m_Locker", BindingFlags.NonPublic | BindingFlags.Static)
                    .IfNotNull(f => f.GetValue(null));

                if (lockObj != null)
                {
                    lock (lockObj)
                    {
                        dictionaryStaticType.GetField("cacheIsEnsured", BindingFlags.NonPublic | BindingFlags.Static)
                            .IfNotNull(f => f.SetValue(null, false));
                        dictionaryStaticType.GetField("DictionaryItems", BindingFlags.NonPublic | BindingFlags.Static)
                            .IfNotNull(f => f.SetValue(null, Hashtable.Synchronized(new Hashtable())));
                    }
                }
            }catch(Exception ex){
                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
            }

            try
            {
                // Clear Item (text) cache
                var itemType = typeof(umbraco.cms.businesslogic.language.Item);
                var lockObj = itemType.GetField("m_Locker", BindingFlags.NonPublic | BindingFlags.Static)
                    .IfNotNull(f => f.GetValue(null));

                if (lockObj != null)
                {
                    lock (lockObj)
                    {
                        itemType.GetField("m_IsInitialize", BindingFlags.NonPublic | BindingFlags.Static)
                            .IfNotNull(f => f.SetValue(null, false));
                        itemType.GetField("_items", BindingFlags.NonPublic | BindingFlags.Static)
                            .IfNotNull(f => f.SetValue(null, Hashtable.Synchronized(new Hashtable())));
                    }
                }
            }
            catch (Exception ex)
            {
                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
            }
        }
    }
}