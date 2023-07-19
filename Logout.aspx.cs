using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : System.Web.UI.Page
{
    public static clsSession sess = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        setTitle();
        Session["UserSession"] = null;
        Session.RemoveAll();
        Session.Abandon();
    }

    private void setTitle()
    {
        clsData objData = new clsData();
        sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            object obj = objData.FetchValue("Select SchoolDesc from School Where SchoolId='" + sess.SchoolId + "'");
            if (obj != null)
            {
                TitleName.Text = obj.ToString();
            }
        }

    }
}