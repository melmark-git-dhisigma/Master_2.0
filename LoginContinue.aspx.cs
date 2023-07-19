using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;

public partial class LoginContinue : System.Web.UI.Page
{
    clsData objData = null;
    clsSession objSession = null;
    string Redirect = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        ClsErrorLog errlog = new ClsErrorLog();
        if (Session["Values"] != null)
        {
            errlog.WriteToLog("Session[Values]" + Session["Values"].ToString());
            string Values = Session["Values"].ToString();
            string[] arValues = Values.Split('#');
            int LoginId = Convert.ToInt16(arValues[1]);
            SetUserSession(LoginId);
            fillclass();
        }
        else
        {
            errlog.WriteToLog("Session[UserSession]  is NULL");
            Response.Redirect("Logout.aspx");
        }
    }
    public void fillclass()
    {
        ClsErrorLog errlog = new ClsErrorLog();
        objData = new clsData();

        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            try
            {
                if (objData.IFExists("SELECT ClassId from UserClass where UserId='" + sess.LoginId + "'") == true)
                {
                    DataTable dt = objData.ReturnDataTable("SELECT UsrCls.ClassId AS Id,Cls.ClassName AS Name FROM UserClass UsrCls INNER JOIN Class Cls ON Cls.ClassId=UsrCls.ClassId WHERE UsrCls.UserId=" + sess.LoginId + " AND Cls.ActiveInd='A' And UsrCls.ActiveInd='A' And Cls.SchoolId=" + sess.SchoolId + " order by  Name ASC ", false);
                    drpClass.DataSource = dt;
                    drpClass.DataTextField = "Name";
                    drpClass.DataValueField = "Id";
                    drpClass.DataBind();
                    drpClass.Items.Insert(0, new ListItem("------------Select Class------------", "0"));
                }
            }
            catch (Exception ex)
            {
                errlog.WriteToLog(ex.Message);
            }
        }
        else
        {
            errlog.WriteToLog("UserSession is NULL");
        }
        try
        {
           string pwreset = Session["pwreset"] as string;

           if (pwreset == "1")
            {
                Pwmessage.Visible = true;
            }
        }
        catch (Exception ex)
        {
        }
    }


    protected void btnContinue_Click(object sender, EventArgs e)
    {
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            if (hidClass.Value == "0")
            {
                Validation.InnerText = "Please select class";
                return;
            }
            else
            {
                Redirect = hidSelected.Value.ToString();

                sess.Redirect = Redirect;

                if (Redirect == "BI" || Redirect == "CD")
                {
                    sess.Classid = Convert.ToInt32(hidClass.Value);

                    if (Redirect == "BI") Response.Redirect("BiWeekly/StudentBinder/Home.aspx");
                }
            }
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

    }
    protected void AD_Click(object sender, EventArgs e)
    {
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            if (Redirect.ToString() == "AD") Response.Redirect("BiWeekly/Administration/AdminHome.aspx");
        }
        else
        {
            Response.Redirect("Login.aspx");
        }
    }

    protected void VT_Click(object sender, EventArgs e)
    {
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            if (Redirect == "VT") Response.Redirect("BiWeekly/VisualTool/homePage.aspx");
        }
        else
        {
            Response.Redirect("Login.aspx");
        }
    }
    protected void AS_Click(object sender, EventArgs e)
    {
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            if (Redirect == "AS") Response.Redirect("MelmarkR/");
        }
        else
        {
            Response.Redirect("Login.aspx");
        }
    }

    private void SetUserSession(int UserID)
    {
        try
        {
            if (UserID != 0)
            {
                clsData objData = new clsData();
                string strQuery = "SELECT Role.RoleId, Role.RoleDesc as RoleName, [User].UserId, [User].UserLName +', '+ [User].UserFName  AS LoginName,[User].Gender ," +
                    " [User].SchoolId FROM Role INNER JOIN RoleGroup ON Role.RoleId = RoleGroup.RoleId CROSS JOIN [User] WHERE        [User].UserId = " + UserID + "";
                DataTable Dt = objData.ReturnDataTable(strQuery, false);
                if (Dt == null) return;
                if (Dt.Rows.Count > 0)
                {
                    ArrayList ar = new ArrayList();
                    ArrayList arBinder = new ArrayList();
                    Hashtable hs = new Hashtable();

                    objSession = new clsSession();
                    objSession.IsLogin = true;
                    objSession.LoginTime = (DateTime.Now.ToShortTimeString()).ToString();
                    objSession.SchoolId = Convert.ToInt32(Dt.Rows[0]["SchoolId"]);
                    objSession.LoginId = Convert.ToInt32(Dt.Rows[0]["UserId"]);
                    objSession.UserName = Convert.ToString(Dt.Rows[0]["LoginName"]);
                    objSession.RoleId = Convert.ToInt32(Dt.Rows[0]["RoleId"]);
                    objSession.Gender = Convert.ToString(Dt.Rows[0]["Gender"]);
                    objSession.RoleName = Convert.ToString(Dt.Rows[0]["RoleName"]);
                    objSession.SessionID = Session.SessionID.ToString();
                    objSession.YearId = Convert.ToInt32(objData.FetchValue("SELECT AsmntYearId FROM AsmntYear WHERE CurrentInd='A'"));
                    objSession.AdminView = 1;
                    clsGeneral.PageLister(objSession.LoginId, objSession.SchoolId, out ar, out hs);
                    clsGeneral.PageBinderLister(objSession.LoginId, objSession.SchoolId, out arBinder);
                    objSession.perPage = ar;
                    objSession.perPageName = hs;
                    objSession.perPageBinder = arBinder;
                    Session["UserSession"] = objSession;


                    string Values = objSession.SchoolId + "#" + objSession.LoginId + "#" + objSession.Classid + "#" + objSession.UserName;
                    Session["Values"] = Values;
                }
            }
            else
            {
                Session.Abandon();
            }
        }
        catch (Exception exp)
        {
            Response.Redirect("~/ErrorPage/ContactAdmin.aspx");
            throw exp;
        }
    }

    protected void RD_Click(object sender, EventArgs e)
    {
        ClsErrorLog errlog = new ClsErrorLog();
        errlog.WriteToLog("Referral DB");

        //Session["Values"] = Values;     
        if (Session["Values"] != null)
        {
            errlog.WriteToLog("Session[Values]" + Session["Values"].ToString());
            string Values = Session["Values"].ToString();
            string[] arValues = Values.Split('#');
            int LoginId = Convert.ToInt16(arValues[1]);
            SetUserSession(LoginId);


            clsSession sess = (clsSession)Session["UserSession"];
            if (sess != null)
            {
                Hashtable hsh = sess.perPageName;
                errlog.WriteToLog("HAsh Table Page Name" + hsh);
                if (hsh.Contains("Referal") == true)
                {
                    Redirect = hidSelected.Value.ToString();
                    sess.Redirect = Redirect;
                    if (Redirect == "RD") Response.Redirect("ReferalDB/Dashboard/");
                }
                else
                {
                    string message = "Sorry , you have no permission";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<script type = 'text/javascript'>");
                    sb.Append("window.onload=function(){");
                    sb.Append("alert('");
                    sb.Append(message);
                    sb.Append("')};");
                    sb.Append("</script>");
                    Session["UserSession"] = null;
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
                }
            }
            else
            {
                errlog.WriteToLog("Session[UserSession]  is NULL");
                Response.Redirect("Login.aspx");
            }
        }
        else
        {
            errlog.WriteToLog("Session[Values] is NULL");
            Response.Redirect("Login.aspx");
        }
    }

    protected void CD_Click(object sender, EventArgs e)
    {
        ClsErrorLog errlog = new ClsErrorLog();
        errlog.WriteToLog("Client DB");
        //  errlog.WriteToLog("Session[Values]" + Session["Values"].ToString());
        clsData objData = new clsData();

        errlog.WriteToLog("Referral DB");

        //Session["Values"] = Values;     
        if (Session["Values"] != null)
        {
            errlog.WriteToLog("Session[Values]" + Session["Values"].ToString());
            string Values = Session["Values"].ToString();
            string[] arValues = Values.Split('#');
            int LoginId = Convert.ToInt16(arValues[1]);
            SetUserSession(LoginId);

            clsSession sess = (clsSession)Session["UserSession"];
            if (sess != null)
            {
                string strqry = " select count(1) from UserRoleGroup as urg  Join RoleGroupPerm as rgp on urg.RoleGroupId = rgp.RoleGroupId " +
                                "Join [Object] as o on rgp.ObjectId = o.ObjectId where urg.UserId = " + sess.LoginId + " and o.objectname='General Client' " +
                                "and o.ParntObjectId = (select ObjectId from [Object] where ObjectName = 'Client')";
                int GCCount = Convert.ToInt32(objData.FetchValue(strqry));
                if (GCCount == 0)
                {
                    Redirect = hidSelected.Value.ToString();
                    sess.Redirect = Redirect;
                    if (Redirect == "CD") Response.Redirect("ClientDB");
                }
                Hashtable hsh = sess.perPageName;
                if (hsh.Contains("General Client") == true)
                {
                    Redirect = hidSelected.Value.ToString();
                    sess.Redirect = Redirect;
                    if (Redirect == "CD") Response.Redirect("ClientDB");
                }
                else
                {
                    string message = "Sorry , you have no permission";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<script type = 'text/javascript'>");
                    sb.Append("window.onload=function(){");
                    sb.Append("alert('");
                    sb.Append(message);
                    sb.Append("')};");
                    sb.Append("</script>");
                    Session["UserSession"] = null;
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        else
        {
            errlog.WriteToLog("Session[Values] is NULL");
            Response.Redirect("Login.aspx");
        }
    }
    protected void logout_Click(object sender, EventArgs e)
    {
        //Session["UserSession"] = null;
        //Session.RemoveAll();
        //Session.Abandon();
        Response.Redirect("Logout.aspx");
    }

    //Added by Haritha to include UI
    protected void UI_Click(object sender, EventArgs e)
    {
        
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            if (Redirect == "UI") Response.Redirect("UIReportProject/testPage.aspx");
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

        

        /*
        clsSession sess = (clsSession)Session["UserSession"];
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            
        string testValue = (sess.LoginId).ToString();
        System.Security.Cryptography.DESCryptoServiceProvider md5Obj = new System.Security.Cryptography.DESCryptoServiceProvider();
        byte[] key = new byte[] {146, 43, 41, 160, 64, 185, 185, 121};
        md5Obj.Key = key;
        md5Obj.IV = key;
        ICryptoTransform desencrypt = md5Obj.CreateEncryptor();
        byte[] byteToHash = System.Text.Encoding.ASCII.GetBytes("testValue");
        byte[] result = desencrypt.TransformFinalBlock(byteToHash, 0, byteToHash.Length);
        string ret = BitConverter.ToString(result); ;// Convert.ToBase64String(ms.ToArray());
     
        if (Redirect == "UI") Response.Redirect("UINewEngland/UI_NE.aspx?Id="+ret.ToString());


        }
        else
        {
            Response.Redirect("Login.aspx");
        }
        */


    }


    protected void WB_Click(object sender, EventArgs e)
    {
        
        clsSession sess = (clsSession)Session["UserSession"];
        
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            //if (Redirect == "WB") Response.Redirect("WellBodycheck/WBCForm.aspx");
            if (Redirect == "WB") Response.Redirect("HealthTracker/HTMainPage/HTMainPg.aspx");
        }
        else
        {
            Response.Redirect("Login.aspx");
        }

       /*
        if (sess != null)
        {
            Redirect = hidSelected.Value.ToString();
            sess.Redirect = Redirect;
            
            string testValue = (sess.LoginId).ToString();
            System.Security.Cryptography.DESCryptoServiceProvider md5Obj = new System.Security.Cryptography.DESCryptoServiceProvider();
            byte[] key = new byte[] { 146, 43, 41, 160, 64, 185, 185, 121 };
            md5Obj.Key = key;
            md5Obj.IV = key;
            ICryptoTransform desencrypt = md5Obj.CreateEncryptor();
            byte[] byteToHash = System.Text.Encoding.ASCII.GetBytes("testValue");
            byte[] result = desencrypt.TransformFinalBlock(byteToHash, 0, byteToHash.Length);
            string ret = BitConverter.ToString(result); ;// Convert.ToBase64String(ms.ToArray());
            


            if (Redirect == "WB") Response.Redirect("WellBodycheck/WBCForm.aspx?Id=" + ret.ToString());

            

        }
        else
        {
            Response.Redirect("Login.aspx");
        }*/
    }




}