using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.presentation.umbracobase;
using umbraco.DataLayer;
using umbraco.BusinessLogic;
using System.Xml;
using System.Web.Script.Serialization;
using Umbraco.Courier.Core;

namespace CompanyResolver
{

    //Company class, is standard .net class, whic enherites from Umbraco.Courier.Core.Item
    //To make it support serialization and setting of a unique ID which tells courier which Provider to use
    //The rest is a standard class implementation (along with some restExtension attributes

    [RestExtension("company")]
    public class Company : Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Symbol { get; set; }


        [RestExtensionMethod(returnXml = false)]
        public static string Get(int id)
        {
            Company c = new Company(id);
            if (c == null)
                return "not found";

            return c.ToJSon();
        }

        public Company() { }
        public Company(int id)
        {
            string sql = "SELECT * from companies Where ID = @id";
            IRecordsReader rr = Application.SqlHelper.ExecuteReader(sql, Application.SqlHelper.CreateParameter("@id", id));

            if (rr.Read())
                FromReader(this, rr);

            rr.Dispose();
        }

        public Company(string name)
        {
            string sql = "SELECT * from companies Where name = @name";
            IRecordsReader rr = Application.SqlHelper.ExecuteReader(sql, Application.SqlHelper.CreateParameter("@name", name));

            if (rr.Read())
                FromReader(this, rr);

            rr.Dispose();
        }

        private static Company FromReader(IRecordsReader rr)
        {
            Company c = new Company();
            return FromReader(c, rr);
        }


        private static Company FromReader(Company c, IRecordsReader rr)
        {
            c.Id = rr.GetInt("id");
            c.Category = rr.GetString("category");
            c.Name = rr.GetString("name");
            c.Symbol = rr.GetString("symbol");

            return c;
        }


        public static List<Company> GetAll()
        {
            string sql = "SELECT * from companies order by Name ASC";
            IRecordsReader rr = umbraco.BusinessLogic.Application.SqlHelper.ExecuteReader(sql);

            List<Company> list = new List<Company>();

            while (rr.Read())
                list.Add(FromReader(rr));

            rr.Dispose();

            return list;
        }


        public XmlNode ToXml(XmlDocument xd)
        {
            XmlNode node = umbraco.xmlHelper.addTextNode(xd, "company", "");
            node.AppendChild(umbraco.xmlHelper.addTextNode(xd, "id", this.Id.ToString()));
            node.AppendChild(umbraco.xmlHelper.addTextNode(xd, "category", this.Category));
            node.AppendChild(umbraco.xmlHelper.addTextNode(xd, "name", this.Name));
            node.AppendChild(umbraco.xmlHelper.addTextNode(xd, "symbol", this.Symbol));
            return node;
        }

        public string ToJSon()
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }
}