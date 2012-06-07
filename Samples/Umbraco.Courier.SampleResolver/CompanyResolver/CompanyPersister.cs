using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Courier.Core;
using Umbraco.Courier.Persistence.V4.NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate;
using CompanyResolver.Nhibrnate_entities;
using Umbraco.Courier.Core.Helpers;

namespace CompanyResolver
{
    //The company persister handles all database activity as part of a single Transaction
    //It uses fluent-Nhibernate to map a CompanyProxy which is the data it gets back from the database
    //this proxy is then mapped to the actual Company object which courier can then serialize and install
    
    [ItemCrud(typeof(Company), typeof(NHibernateProvider))]
    public class CompanyPersister : ItemCrud
    {
        public override List<SystemItem> AvailableItems<T>(ItemIdentifier itemId)
        {
            //if looking for children, return nothing
            if (itemId != null)
                return new List<SystemItem>();


            List<SystemItem> allItems = new List<SystemItem>();
            var session = NHibernateProvider.GetCurrentSession();
            foreach (var c in session.Linq<CompanyProxy>())
            {
                SystemItem si = new SystemItem();
                si.Description = "Company with the name: " + c.Name;
                si.Name = c.Name;
                si.ItemId = new ItemIdentifier(c.Symbol, Constants.companyProviderID);
                si.Icon = "package2.png";
                si.HasChildren = false;
                allItems.Add(si);
            }
            
            return allItems;
        }
        

        public override T PersistItem<T>(T item)
        {
            var session = NHibernateProvider.GetCurrentSession();

            Company update = item as Company;
            CompanyProxy current = GetCompanyBySymbol(update.Symbol, session);

            //if it doesn exist
            if (current == null)
                session.Save( ParseToProxy(update) );
            else
            {
                //set the ID to the current one and persist in case company had its other values updated
                update.Id = current.Id;
                session.Update(ParseToProxy(update));
            } 
            return update as T;
        }


        public override T RetrieveItem<T>(ItemIdentifier itemId)
        {
            var session = NHibernateProvider.GetCurrentSession();
            
            //here we allow 2 ways to get it, either by ID, or by symbol
            //this way we don't need to do alot of conversion of id -> symbol

            int companyId = 0;
            string companySymbol = itemId.Id;

            //the item proxy returned from nhibernate
            CompanyProxy retval;

            if (int.TryParse(itemId.Id, out companyId))
            {
                //return based on ID
                retval = session.Get<CompanyProxy>(companyId);
            }
            else
            {
                //return based on symbol
                retval = GetCompanyBySymbol(companySymbol, session);
            }

            //if not found, return null
            if (retval == null)
                return null;
           
            
            Company c = ParseFromProxy(retval);

            //set the item ID to the Symbol and the provider ID
            c.ItemId = new ItemIdentifier(retval.Symbol, itemId.ProviderId);

            //cast as T
            return c as T;
        }


        private CompanyProxy GetCompanyBySymbol(string symbol, ISession session)
        {
            var c = session.Linq<CompanyProxy>().Where(x => x.Symbol == symbol).FirstOrDefault();
            return c;
        }

        private Company ParseFromProxy(CompanyProxy pi)
        {
            Company c = new Company();
            c.Category = pi.Category;
            c.Name = pi.Name;
            c.Id = pi.Id;
            c.Symbol = pi.Symbol;

            return c;
        }

        private CompanyProxy ParseToProxy(Company pi)
        {
            CompanyProxy c = new CompanyProxy();
            c.Category = pi.Category;
            c.Name = pi.Name;
            c.Id = pi.Id;
            c.Symbol = pi.Symbol;

            return c;
        }

        //not supported in 2.0
        public override bool DeleteItem<T>(ItemIdentifier itemId)
        {
            throw new NotImplementedException();
        }
    }
}