using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CompanyResolver.Picker
{
    //this is just a standard usercontrolwrapper based picker to select companies from a companies tables

    public partial class CompanyPicker : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        private string currentValue = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var companies = Company.GetAll();
                foreach (var c in companies)
                {
                    dd1.Items.Add(new ListItem(c.Name, c.Symbol));
                }

                dd1.SelectedValue = currentValue;
            }
        }

        public object value
        {
            get
            {
                return dd1.SelectedValue;
            }
            set
            {
                if(!string.IsNullOrEmpty(value.ToString()))
                    currentValue = value.ToString();
            }
        }
    }
}