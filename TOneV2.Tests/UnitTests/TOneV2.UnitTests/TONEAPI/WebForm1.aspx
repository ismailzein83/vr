<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="TONEAPI.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        T.One V2 test API<br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="URL" Width="65px"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" Width="456px">http://192.168.110.195:8585</asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Username" Width="65px"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server" Width="266px">admin@vanrise.com</asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Password" Width="65px"></asp:Label>
        <asp:TextBox ID="TextBox3" runat="server" Width="265px">1</asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Start" Width="116px" />
        <br />
        Token :
        <asp:TextBox ID="TextBox7" runat="server" Width="1076px"></asp:TextBox>
        <br />
        <br />
        <hr />
        <br />
        <asp:Label ID="Modules" runat="server" style="text-align: center; font-weight: 700" Text="Modules" Width="428px"></asp:Label>
        <asp:Label ID="Label5" runat="server" style="text-align: center; font-weight: 700" Text=" Successful" Width="428px"></asp:Label>
        <asp:Label ID="Label6" runat="server" style="text-align: center; font-weight: 700" Text="Failed" Width="428px"></asp:Label>
    
    </div>
        <asp:TextBox ID="TextBox4" runat="server" BackColor="#A5C3DE" Height="600px" TextMode="MultiLine" Width="420px"></asp:TextBox>
        <asp:TextBox ID="TextBox5" runat="server" BackColor="White" Height="600px" TextMode="MultiLine" Width="420px"></asp:TextBox>
        <asp:TextBox ID="TextBox6" runat="server" BackColor="#FF6666" Height="600px" TextMode="MultiLine" Width="420px"></asp:TextBox>
    </form>
</body>
</html>
