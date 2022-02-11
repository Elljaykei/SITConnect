<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

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

     <script type="text/javascript">
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

        function pwValidate() {
            var str = document.getElementById('<%=password_tb.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("pwdChecker_lbl").innerHTML = "Password length must be at least 12 characters";
                document.getElementById("pwdChecker_lbl").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("pwdChecker_lbl").innerHTML = "Password require at least 1 number";
                document.getElementById("pwdChecker_lbl").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("pwdChecker_lbl").innerHTML = "Password require at least 1 upper case character";
                document.getElementById("pwdChecker_lbl").style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("pwdChecker_lbl").innerHTML = "Password require at least 1 lower case character";
                document.getElementById("pwdChecker_lbl").style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("pwdChecker_lbl").innerHTML = "Password require at least 1 special character";
                document.getElementById("pwdChecker_lbl").style.color = "Red";
                return ("no_special_character");
            }

            document.getElementById("pwdChecker_lbl").innerHTML = "Excellent!";
            document.getElementById("pwdChecker_lbl").style.color = "Blue";
        }
     </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Registration</h2>
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">First Name</td>
                    <td>
                        <asp:TextBox ID="fName_tb" runat="server" Width="200px" required></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Last Name</td>
                    <td>
                        <asp:TextBox ID="lName_tb" runat="server" Width="200px" required></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Credit Card Info</td>
                    <td>
                        <asp:TextBox ID="creditCard_tb" runat="server" Width="200px" MinLength="16" MaxLength="16" required></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Email Address</td>
                    <td>
                        <asp:TextBox ID="emailAddress_tb" runat="server" Width="200px" onkeyup="emailValidate()" required></asp:TextBox>
                        <asp:Label ID="emailChecker_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Password</td>
                    <td>
                        <asp:TextBox ID="password_tb" runat="server" Width="200px" onkeyup="pwValidate()" required></asp:TextBox>
                        <asp:Label ID="pwdChecker_lbl" runat="server"></asp:Label>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Date of Birth</td>
                    <td>
                        <asp:TextBox ID="dob_tb" runat="server" Width="200px" Text='<%# Bind("DateofBirth", "{0:yyyy-MM-dd}") %>' TextMode="Date" required></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Photo</td>
                    <td>
                        <asp:TextBox ID="photo_tb" runat="server" Width="200px" required></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4" colspan="2">
                        <asp:Label ID="error_msg_lbl" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
        <asp:Button ID="registerBtn" runat="server" OnClick="registerBtn_Click" Text="Register" CssClass="btn" Width="343px" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
