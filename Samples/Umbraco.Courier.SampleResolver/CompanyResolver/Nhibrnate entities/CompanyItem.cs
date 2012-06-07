using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyResolver.Nhibrnate_entities
{
    //Proxy class to pull data from nHibernate
    //Ideally we would use the company class for this
    //but nhibernate requeres virtual properties to work

    public class CompanyProxy
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual string Symbol { get; set; }
    }
}