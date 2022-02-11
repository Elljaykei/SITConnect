<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDetails.aspx.cs" Inherits="SITConnect.UserDetails" %>

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
</head>
<body>
    <form id="form2" runat="server">
        <div>
            <h2>User Details</h2>
            <p>&nbsp;</p>
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
    <form id="form1" runat="server">
    </form>
</body>
</html>
