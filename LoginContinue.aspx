<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginContinue.aspx.cs" Inherits="LoginContinue" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="Scripts/jquery-1.8.0.js"></script>
    <link href="Scripts/LoginCont.css" rel="stylesheet" />


    <style type="text/css">
        .visit {
            background-color: rgb(0, 128, 0) !important;
            border-radius: 5px 5px 5px 5px;
        }

        .logout, .logout:link, .logout:visited {
            background: url("../Images/Logout.png") no-repeat scroll 6px 0 rgba(0, 0, 0, 0);
            color: #0073b8;
            display: block;
            float: left;
            font-family: arial;
            font-size: 13px;
            font-weight: bold;
            height: 18px;
            padding: 0 0 0 27px;
            text-align: left;
            text-decoration: none;
            width: 18px;
            cursor:pointer;
            margin-left: 0;
            margin-right: 0;
            margin-bottom: 0;
        }
    </style>
    <script type="text/javascript">
        function setClass(elem) {

            $('#hidSelected').val(elem);

            var imgPosX = 30;
            var imgPosY = 120;

            var block = "block";
            var none = "none";

            $(AD).removeClass('visit');
            $(BI).removeClass('visit');
            $(VT).removeClass('visit');
            //$(AS).removeClass('visit');
            $(RD).removeClass('visit');
            $(CD).removeClass('visit');

            //Below line commented by Haritha
            //$(RS).removeClass('visit');

            $(UI).removeClass('visit');

            $('#' + elem).addClass('visit');

            if (elem == "BI") {

                if ($(divClass).is(':hidden')) {
                    $(divClass).slideToggle();
                }

            }
            else if (elem == "RS") {
                window.open('<%=System.Configuration.ConfigurationManager.AppSettings["SharepointRpt"] %>', '_blank');
           }

            else {
                $(divClass).fadeOut("fast");
            }



        }
        function selectClass() {
            if ($('#drpClass').val() == "0") {

                $(Validation).text("Please select a class");
                return;
            }
            else {
                $(Validation).text("");
            }
        }
        function getClass() {
            $('#hidClass').val($('#drpClass option:selected').val());

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="logoutdiv" style="float: right; width: 7%; margin: 16px 13px 0px 0px;">
            <%--<asp:HyperLink class="logout" ID="HyperLink1" runat="server">Logout</asp:HyperLink>--%>
            <asp:LinkButton class="logout" ID="logout" runat="server" OnClick="logout_Click">Logout</asp:LinkButton>
        </div>
        <div id="maincontainer">
            <div class="logo">
                <img src="images/Logo.png" width="269" height="70" alt="" />
            </div>
            <asp:Label ID="Pwmessage" Text="Password Reset Successfully. " runat="server" ForeColor="DarkGreen" Visible="false" ></asp:Label>
            <div class="clear"></div>
            <div class="innercontainer">
                <div class="LoginContinue">
                </div>
                <div id="Validation" runat="server">
                </div>
                <div class="clear"></div>
                <div class="menuContainer">
                    <asp:LinkButton ID="AD" class="administrati" OnClientClick="setClass(this.id);" runat="server" OnClick="AD_Click" />
                    <a id="BI" class="biweekly" runat="server" onclick="setClass(this.id);" />
                    <asp:LinkButton ID="VT" class="visualtool" runat="server" OnClientClick="setClass(this.id);" OnClick="VT_Click" />
                    <%--  <asp:LinkButton ID="AS" class="analytics" runat="server" OnClientClick="setClass(this.id);" OnClick="AS_Click" />--%>
                    <asp:LinkButton ID="CD" class="clientDB" runat="server" OnClientClick="setClass(this.id);" OnClick="CD_Click" />
                    <asp:LinkButton ID="RD" class="referralDB" runat="server" OnClientClick="setClass(this.id);" OnClick="RD_Click" />
                    <%-- <asp:LinkButton ID="RS" class="Reports" runat="server" OnClientClick="setClass(this.id);" />--%>
                    <asp:LinkButton ID="UI" class="UIDB" runat="server" OnClientClick="setClass(this.id);" OnClick="UI_Click" />
                    
                    <asp:LinkButton ID="WB" class="WellBody" runat="server" OnClientClick="setClass(this.id);" OnClick="WB_Click"/>
                </div>

                <div class="clear"></div>


                <div id="divClass">
                    Select Class
                    <select id="drpClass" runat="server" onchange="getClass()" style="border-radius: 3px; width: 320px;height:30px; border: solid 1px #999999;"></select>

                    <asp:Button ID="btnContinue" Style="background-color: #006552; font-weight: bold;height:40px;margin:5px; color: #FFFFFF; border-radius: 3px; border: solid 1px #006552;" runat="server" OnClientClick="selectClass();" Text="Continue" OnClick="btnContinue_Click" />

                </div>
                <img src="Images/down.png" alt="" style="display: none; position: absolute;" id="down" />
                <input type="hidden" runat="server" value="0" id="hidSelected" />
                <input type="hidden" runat="server" value="0" id="hidClass" />
            </div>
        </div>

    </form>
</body>
</html>
