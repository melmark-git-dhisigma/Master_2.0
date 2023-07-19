using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Collections;
using System.Data;
using System.DirectoryServices;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;

public partial class ApplicationError : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string baseUrl = "";
        baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        burl.HRef = baseUrl + "Login.aspx";
    }
}