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
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Start API Test" Width="116px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Start GUI Test" style="height: 26px" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Unit Test" Width="144px" />
        <br />
        Token :
        <asp:TextBox ID="TextBox7" runat="server" Width="1076px"></asp:TextBox>
        <br />
        <br />
        <hr />
        <br />
        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Height="290px" Width="1665px">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <asp:Label ID="Modules" runat="server" style="text-align: center; font-weight: 700" Text="Modules" Width="428px"></asp:Label>
        <asp:Label ID="Label5" runat="server" style="text-align: center; font-weight: 700" Text=" Successful" Width="428px"></asp:Label>
        <asp:Label ID="Label6" runat="server" style="text-align: center; font-weight: 700" Text="Failed" Width="428px"></asp:Label>
    
    </div>
        <asp:TextBox ID="TextBox4" runat="server" BackColor="#A5C3DE" Height="600px" TextMode="MultiLine" Width="420px" style="color: #FFFFFF; background-color: #5D7B9D"></asp:TextBox>
        <asp:TextBox ID="TextBox5" runat="server" BackColor="White" Height="600px" TextMode="MultiLine" Width="420px"></asp:TextBox>
        <asp:TextBox ID="TextBox6" runat="server" BackColor="#FF6666" Height="600px" TextMode="MultiLine" Width="420px" style="color: #FFFFFF; background-color: #4D6082"></asp:TextBox>
    </form>
</body>
</html>
