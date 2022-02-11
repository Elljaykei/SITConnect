<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verification.aspx.cs" Inherits="SITConnect.Verification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">


        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 145px;
        }
        .auto-style4 {
            height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Verification</h2>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Verification Code</td>
                    <td>
                        <asp:TextBox ID="verCode_tb" runat="server" Width="200px" onkeyup="emailValidate()" required></asp:TextBox>
                        <asp:Label ID="verCode_lbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4" colspan="2">
                        <asp:Label ID="error_msg_lbl" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="verBtn" runat="server" CssClass="btn" OnClick="verBtn_Click" Text="Verify" Width="352px" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
