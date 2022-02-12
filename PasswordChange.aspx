<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordChange.aspx.cs" Inherits="SITConnect.PasswordChange" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">


        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 183px;
        }
        .auto-style4 {
            height: 26px;
        }
    </style>
    <script type="text/javascript">

        function pwValidate(e) {
            var item = e.nextElementSibling.getAttribute("id");
            var str = e.value;

            if (str.length < 12) {
                document.getElementById(item).innerHTML = "Password length must be at least 12 characters";
                document.getElementById(item).style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById(item).innerHTML = "Password require at least 1 number";
                document.getElementById(item).style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById(item).innerHTML = "Password require at least 1 upper case character";
                document.getElementById(item).style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById(item).innerHTML = "Password require at least 1 lower case character";
                document.getElementById(item).style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById(item).innerHTML = "Password require at least 1 special character";
                document.getElementById(item).style.color = "Red";
                return ("no_special_character");
            }

            document.getElementById(item).innerHTML = "Excellent!";
            document.getElementById(item).style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
            <h2>Password Change</h2>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Old Password</td>
                    <td>
                        <asp:TextBox ID="password_tb1" runat="server" Width="200px" onkeyup="pwValidate(this)" required></asp:TextBox>
                        <asp:Label ID="pwdChecker_lbl1" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">New Password</td>
                    <td>
                        <asp:TextBox ID="password_tb2" runat="server" Width="200px" required></asp:TextBox>
                        <asp:Label ID="pwdChecker_lbl2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Confirm Password</td>
                    <td>
                        <asp:TextBox ID="password_tb3" runat="server" Width="200px" required></asp:TextBox>
                        <asp:Label ID="pwdChecker_lbl3" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4" colspan="2">
                        <asp:Label ID="error_msg_lbl" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="SaveBtn" runat="server" OnClick="SaveBtn_Click" Text="Save Changes" Width="284px" />
                    </td>
                </tr>
            </table>
            </form>
</body>
</html>
