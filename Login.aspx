<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 134px;
        }
        .auto-style4 {
            height: 26px;
        }
    </style>

    <script src="https://www.google.com/recaptcha/api.js?render=6LcXaHAeAAAAAPAFizR33X8IpXqCdQKCfrvfXuMf"></script>
    

<%--    <script type="text/javascript">
         function emailValidate() {
             var email = document.getElementById('<%=emailAddress_tb.ClientID %>').value;

             if (!/^\w+([.-]?\w+)@\w+([.-]?\w+)(.\w{2,3})+$/.test(email)) {
                 document.getElementById("emailChecker_lbl").innerHTML = "Email format incorrect";
                 document.getElementById("emailChecker_lbl").style.color = "Red";
             }
             else {
                 document.getElementById("emailChecker_lbl").innerHTML = "Email Valid";
                 document.getElementById("emailChecker_lbl").style.color = "Green";
             }
         }
    </script>--%>
</head>
<body>
    <form id="form2" runat="server">
        <div>
            <h2>Login</h2>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Email Address</td>
                    <td>
                        <asp:TextBox ID="emailAddress_tb" runat="server" Width="200px" required></asp:TextBox>
                        <asp:Label ID="emailChecker_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Password</td>
                    <td>
                        <asp:TextBox ID="password_tb" runat="server" Width="200px" required></asp:TextBox>
                        <asp:Label ID="pwdChecker_lbl" runat="server"></asp:Label>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4" colspan="2">
                        <asp:Label ID="error_msg_lbl" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="loginBtn" runat="server" CssClass="btn" Text="Login" Width="171px" OnClick="loginBtn_Click" />
                    </td>
                </tr>
            </table>
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                        <asp:Label ID="lbl_gScore" runat="server"></asp:Label>
        </div>
        
    </form>
    <p>
        Click here to <a href="Registration.aspx" class="btn">Register</a>!
    </p>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcXaHAeAAAAAPAFizR33X8IpXqCdQKCfrvfXuMf', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
    

</body>
</html>

