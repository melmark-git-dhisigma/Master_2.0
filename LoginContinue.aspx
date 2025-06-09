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
        /* Basic styling for the popup */
        .popup {
            display: none;
            position: fixed;
            left: 50%;
            top: 50%;
            transform: translate(-50%, -50%);
            border: 1px solid #ccc;
            padding: 20px;
            background: #fff;
            box-shadow: 0 0 10px rgba(0,0,0,0.5);
            z-index: 1003;
            text-align: center;
            font-family: Arial, sans-serif;
        }

        .popup-overlay {
            display: none;
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 1003;
        }

        .popup-button {
            padding: 5px 10px;
            margin: 10px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            color: #fff;
        }

        .button-logout {
            background-color: #d9534f;
        }

        .button-stay {
            background-color: #5cb85c;
        }

        .popup-button:hover {
            opacity: 0.8;
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
    <!-- Popup overlay -->
    <div id="popupOverlay" class="popup-overlay"></div>

    <!-- Popup window -->
    <div id="popup" class="popup">
        <p id="popupMessage"></p>
        <button class="popup-button button-logout" onclick="logout()">Logout</button>
        <button class="popup-button button-stay" onclick="closeIdlePopup()">Stay Signed In</button>
    </div>

    <script>
        // Define a variable for idle time in milliseconds
        var idleTimeInMilliseconds = 300000; // 5 minutes in milliseconds
        var idleStartTime; // Timestamp when the user became idle
        var countdownTimerInterval;
        var idleTimer;
        var popupVisible = false;

        // Start the idle timer
        function startIdleTimer() {
            idleStartTime = new Date().getTime();
            idleTimer = setTimeout(showPopup, idleTimeInMilliseconds);
        }

        // Reset the idle timer without closing the popup
        function resetIdleTimer() {
            if (!popupVisible) {
                idleStartTime = new Date().getTime();
                clearTimeout(idleTimer);
                idleTimer = setTimeout(showPopup, idleTimeInMilliseconds);
            }
        }

        // Show the popup and start the countdown timer
        function showPopup() {
            if (document.hidden) {
                showNotification();
                flashTitle();
            }
            openPopup();
            if (!popupVisible) {
                startCountdownTimer();
                popupVisible = true;
            }
        }

        // Start the countdown timer
        function startCountdownTimer() {
            updateCountdownTimerMessage();
            countdownTimerInterval = setInterval(updateCountdownTimerMessage, 1000); // Update every second
        }

        // Update the countdown timer message
        function updateCountdownTimerMessage() {
            var currentTime = new Date().getTime();
            var elapsedMilliseconds = currentTime - idleStartTime;

            var hours = Math.floor(elapsedMilliseconds / 3600000);
            var minutes = Math.floor((elapsedMilliseconds % 3600000) / 60000);
            var seconds = Math.floor((elapsedMilliseconds % 60000) / 1000);

            document.getElementById('popupMessage').textContent = 'You have been idle for ' + hours + ' hour(s) ' + minutes + ' minute(s) ';// + seconds + ' second(s).';
        }

        // Flash the title
        function flashTitle() {
            var originalTitle = document.title;
            var flash = true;
            var flashInterval = setInterval(function() {
                document.title = flash ? 'New Message' : originalTitle;
                flash = !flash;
            }, 1000);

            document.addEventListener('visibilitychange', function() {
                if (!document.hidden) {
                    clearInterval(flashInterval);
                    document.title = originalTitle;
                }
            });
        }

        function stopFlashingTitle() {
            clearInterval(flashInterval);
            document.title = originalTitle;
        }

        // Show a notification
        function showNotification() {
            if ("Notification" in window && Notification.permission === "granted") {
                new Notification('You have been idle for 5 minutes. Do you want to stay signed in?');
            }
        }

        // Open the popup
        function openPopup() {
            document.getElementById('popup').style.display = 'block';
            document.getElementById('popupOverlay').style.display = 'block';
        }

        // Close the popup
        function closeIdlePopup() {
            document.getElementById('popup').style.display = 'none';
            document.getElementById('popupOverlay').style.display = 'none';
            clearInterval(countdownTimerInterval);
            popupVisible = false;
            resetIdleTimer();
        }

        // Logout the user
        function logout() {
            window.location.href = "javascript:__doPostBack('logout','')"; // Change the URL to your actual logout URL
        }

        // Event listeners to reset the idle timer on user activity without closing the popup or resetting the countdown timer
        window.onload = startIdleTimer;
        window.onmousemove = resetIdleTimer;
        window.onmousedown = resetIdleTimer; // Catches touchscreen presses as well
        window.ontouchstart = resetIdleTimer;
        window.onclick = resetIdleTimer;     // Catches touchpad clicks as well
        window.onkeydown = resetIdleTimer;
        window.addEventListener('scroll', resetIdleTimer, true); // Catches scrolling

        // Handle visibility change
        document.addEventListener('visibilitychange', function() {
            if (document.visibilityState === 'visible' && new Date().getTime() - idleStartTime >= idleTimeInMilliseconds) {
                showPopup();
            }
        });

        // Request notification permission
        document.addEventListener('DOMContentLoaded', (event) => {
            if ("Notification" in window && Notification.permission !== "granted") {
                Notification.requestPermission();
        }
        });
    </script>
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
            <asp:Label ID="Welmessage" runat="server" Font-Bold="True" Width="100%" Visible="false" ForeColor="#006600"></asp:Label><%--ForeColor="#FF3300"--%>
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
