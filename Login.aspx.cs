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

public partial class Admin_Login : System.Web.UI.Page
{

    public Int32 UserId = 0;
    clsData objData = null;
    bool IsLogined = false;
    clsAciveDirectory objActiveLogin = null;
    clsSession objSession = null;

    protected void Page_Load(object sender, EventArgs e)
    {


        if (IsPostBack == false)
        {
            //txtPassword.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + lnkLogin.UniqueID + "').click();return false;}} else {return true}; ");
            //txtUserName.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + lnkLogin.UniqueID + "').click();return false;}} else {return true}; ");
            //txtUserName.Focus();
        }
    }


    private int SQLServerLogined(string IsActiveLogin)
    {

        //string IsActiveLogin = ConfigurationManager.AppSettings["IsActiveLogin"].ToString();

        DataTable Dt;
        String Password;

        string strQry = "";
        if (IsActiveLogin == "Y")
        {
            strQry = "SELECT UserId,Login,Password FROM [User] WHERE Upper(Login) = Upper(CONVERT(varbinary(180),'" + txtUserName.Text.Trim() + "'))  AND ActiveInd<>'D'";
        }
        else
        {
            strQry = "SELECT UserId,Login,CONVERT(varchar,Password) FROM [User] WHERE Upper(Login) = Upper(CONVERT(varbinary(180),'" + txtUserName.Text.Trim() + "'))   AND (Password=CONVERT(varbinary(180),'" + txtPassword.Text.Trim() + "') or Password=CONVERT(varbinary(180),'')) AND ActiveInd<>'D'";
        }
        try
        {

            objData.Dispose();
            UserId = Convert.ToInt32(objData.FetchValue(strQry));
            Dt = objData.ReturnDataTable(strQry, true);
            if (Dt != null)
            {
                if (Dt.Rows.Count != 0)
                {
                    UserId = Convert.ToInt32(Dt.Rows[0][0]);
                    Password = Convert.ToString(Dt.Rows[0][2]);


                    if (Password == "")
                    {
                        if (txtPassword.Text.Trim() != "")
                        {
                            // update new password
                            String passwrdupdate = "Update [user] set password=CONVERT(varbinary(180),'" + txtPassword.Text.Trim() + "') where Userid=" + UserId;
                            objData.Execute(passwrdupdate);
                            Session["pwreset"] = "1";
                        }
                        else
                        {
                            tdMessage.InnerText = "Enter New Valid Password";
                            Exception exp1 = new Exception("blank Password");
                            throw exp1;

                        }

                    }
                }

            }
        }
        catch (Exception exp)
        {
            tdMessage.InnerText = "You seem to have entered wrong credentials ,Please Try Again !";
            throw exp;
        }
        return UserId;

    }


    private bool ActiveLogin()
    {
        ArrayList ar = new ArrayList();
        string IsActiveLogin = ConfigurationManager.AppSettings["IsActiveLogin"].ToString();
        string DomainName = ConfigurationManager.AppSettings["DomainName"].ToString();

        string USServer1 = ConfigurationManager.AppSettings["USServer1"].ToString();
        string USServer2 = ConfigurationManager.AppSettings["USServer2"].ToString();
        string USServer3 = ConfigurationManager.AppSettings["USServer3"].ToString();

        ar.Add(USServer1);
        ar.Add(USServer2);
        ar.Add(USServer3);

        if (IsActiveLogin == "Y")
        {
            try
            {

                objActiveLogin = new clsAciveDirectory();
                if (objActiveLogin.IsActiveDirectoryLogin(ar, DomainName, txtUserName.Text.Trim(), txtPassword.Text.Trim()) == true)
                {
                    IsLogined = true;
                }
                else
                {
                    IsLogined = false;
                }

            }
            catch
            {

            }
        }
        else
        {
            IsLogined = true;
        }
        return IsLogined;
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

                Session.Clear();

            }
        }
        catch (Exception exp)
        {
            Response.Redirect("~/ErrorPage/ContactAdmin.aspx");
            throw exp;
        }
    }
    protected void lnkLogin_Click(object sender, EventArgs e)
    {
        try
        {

            if (txtUserName.Text == "")
            {
                tdMessage.InnerText = "You seem to have entered wrong credentials !";
                txtUserName.Focus();
                tdMessage.Visible = true;
                return;
            }
            else if (txtPassword.Text == ""  )
            {
                tdMessage.InnerText = "You seem to have entered wrong credentials !";
                txtPassword.Focus();
                tdMessage.Visible = true;
                return;
            }
            objData = new clsData();
            if (txtPassword.Text == "") return;

            string IsActiveLogin = ConfigurationManager.AppSettings["IsActiveLogin"].ToString();

            if (ActiveLogin() == true)
            {
                UserId = SQLServerLogined(IsActiveLogin);
            }
            else
            {
                UserId = SQLServerLogined("N");
            }

            if (UserId > 0)
            {
                SetUserSession(UserId);
            }

            clsSession sess = (clsSession)Session["UserSession"];

            if (sess != null)
            {
                if (sess.LoginId != 0)
                {
                    if (sess.LoginId > 0)
                    {
                        ClsSessionErrorlog sesserrlog = new ClsSessionErrorlog();
                        sesserrlog.WriteToLog(DateTime.Now.ToString() + ',' + sess.LoginTime.ToString() + ',' + sess.LoginId.ToString() + ',' + sess.UserName + ',' + sess.RoleId + ',' + sess.RoleName + ',' + sess.SchoolId + ',' + sess.SessionID + ',' + "Login" + ',' + sess.Classid);

                        Response.Redirect("LoginContinue.aspx");
                    }
                    else
                    {
                        tdMessage.Visible = true;
                        tdMessage.InnerText = "You seem to have entered wrong credentials ,Please Try Again !";
                    }
                }
                else
                {
                    tdMessage.Visible = true;
                    tdMessage.InnerText = "You seem to have entered wrong credentials ,Please Try Again !";
                }
            }
            else
            {
                tdMessage.Visible = true;
                tdMessage.InnerText = "You seem to have entered wrong credentials ,Please Try Again !";

            }

        }
        catch (Exception ex)
        {
            //Response.Redirect("~/ErrorPage/ContactAdmin.aspx");
            throw ex;
        }
        finally
        {

        }
    }

    protected void lnkBtnAdmin_Click1(object sender, EventArgs e)
    {
        try
        {


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    protected void imgLoginSel_Click(object sender, ImageClickEventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}