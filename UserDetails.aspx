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
        .auto-style5 {
            width: 134px;
            height: 33px;
        }
        .auto-style6 {
            height: 33px;
        }
        .auto-style7 {
            margin-left: 70px;
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
                        <asp:Label ID="fname_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Last Name</td>
                    <td>
                        <asp:Label ID="lname_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Credit Card Info</td>
                    <td>
                        <asp:Label ID="creditCard_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Email Address</td>
                    <td>
                        <asp:Label ID="email_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style5">Password</td>
                    <td class="auto-style6">
                        <asp:Label ID="password_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Date of Birth</td>
                    <td>
                        <asp:Label ID="dob_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Photo</td>
                    <td>
                        <asp:Label ID="photo_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4" colspan="2">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="chgPassBtn" runat="server" Text="Change Password" />
                        <asp:Button ID="logOutBtn" runat="server" CssClass="auto-style7" OnClick="logOutBtn_Click" Text="Log Out" Width="171px" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <form id="form1" runat="server">
    </form>
</body>
</html>
