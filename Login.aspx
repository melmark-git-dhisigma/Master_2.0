<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Admin_Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">


<head>
    <%--<link rel="icon" type="image/x-icon" href="images/metaIcon.ico" />--%>
    <link rel="shortcut icon" type="image/x-icon" href="#" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EnvisionSmart</title>
    <link href="Scripts/style.css" rel="stylesheet" type="text/css" />


    <style type="text/css">
        .drpClass {
            border: 1px solid #d7cece;
            background-color: white;
            color: #676767;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 13px;
            line-height: 26px;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            padding: 2px 2px 0px 10px;
            width: 250px;
            height: 35px;
            margin: 30px 0 0 10px;
        }

        .adminLimkBtn {
            background: url("1371730581_admin.PNG") left top no-repeat;
            border: 0;
            color: #FFFFFF;
            cursor: pointer;
            float: right;
            font-size: 12px;
            font-weight: bold;
            height: 52px;
            margin-right: 91px;
            padding: 0;
            text-align: left;
            width: 50px;
        }
    </style>
    
</head>

<body>
    <form id="form1" runat="server">
        <div id="container">
            <div id="logo"> 
                <img src="Images/Logo.png" width="297" height="76" align="middle" />
            </div>
            <div id="login-panel">
                <div id="Logindiv" runat="server" visible="true">
                    <asp:Panel ID="Panel1" runat="server" DefaultButton="lnkLogin">
                        <ul class="loginul">
                            <li class="login">
                                <asp:TextBox ID="txtUserName" runat="server" value="User Name" onBlur="if(this.value=='') this.value='User Name'" autocomplete="off" onFocus="if(this.value =='User Name' ) this.value=''" class="user-textbox"></asp:TextBox>
                            </li>
                            <li class="login">
                                <asp:TextBox ID="txtPassword" runat="server" value="" autocomplete="off"  type="password" class="pwd-textbox" placeholder="*******"></asp:TextBox>
                            </li>
                            <li>
                                <div id="tdMessage" runat="server" style="color: white; font-weight: bold;"></div>
                            </li>
                            <li class="button-panel">
                                <asp:LinkButton ID="lnkLogin" CssClass="login-button" Text="" runat="server" OnClick="lnkLogin_Click"></asp:LinkButton></li>
                            <%-- <li class="forgot-panel">Forgot your password? <a href="#">Click here</a></li>--%>
                        </ul>
                    </asp:Panel>
                </div>


            </div>



        </div>



    </form>
</body>
</html>
