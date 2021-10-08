using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace College_Mangement_System
{
    public partial class LogoutWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["userToken"] = "";
            Session["userEmail"] = "";
            Session["userRole"] = "";
            Response.Redirect("LoginWebForm.aspx");
        }
    }
}