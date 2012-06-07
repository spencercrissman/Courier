using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using CompanyResolver.Nhibrnate_entities;

namespace CompanyResolver
{

    //This maps the Class CompanyProxy to the Companies table in the database
    //this tells nhibernate how data should be fetched and saved in the DB
    public class CompanyMap : ClassMap<CompanyProxy>
    {
        public CompanyMap()
        {
            Table("[dbo].[companies]");
            OptimisticLock.None();
            LazyLoad();

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Category);
            Map(x => x.Symbol);
        }
    }
}