<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="TONEAPI.WebForm2" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            color: #3333CC;
        }
        .auto-style2 {
            color: #FF3300;
        }
        .auto-style3 {
            color: #0000FF;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <span class="auto-style1"><strong>T.One V2 test API</strong></span><br />
        <br />
        <strong>
        <asp:Label ID="Label1" runat="server" Text="URL" Width="65px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" Width="180px" CssClass="auto-style2">http://192.168.110.195:8103</asp:TextBox>
        
        <asp:Label ID="Label7" runat="server" Text="Configuration" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox8" runat="server" Height="22px" CssClass="auto-style2">mvtsprodemov2</asp:TextBox>
        <asp:Label ID="Label4" runat="server" Text="Whs DB" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox9" runat="server" CssClass="auto-style2">mvtsprodemov2</asp:TextBox>
        <asp:Label ID="Label8" runat="server" Text="Analysis" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox10" runat="server" CssClass="auto-style2">mvtsprodemov2</asp:TextBox>
        
        
        <br class="auto-style2" />
        <asp:Label ID="Label2" runat="server" Text="Username" Width="65px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server" Width="180px" CssClass="auto-style2">admin@vanrise.com</asp:TextBox>
        <asp:Label ID="Label9" runat="server" Text="IP" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox11" runat="server" CssClass="auto-style2">192.168.110.195</asp:TextBox>
        <asp:Label ID="Label10" runat="server" Text="username" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox12" runat="server" CssClass="auto-style2">sa</asp:TextBox>
        <asp:Label ID="Label11" runat="server" Text="Password" Width="100px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox13" runat="server" CssClass="auto-style2"></asp:TextBox>
        </strong>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Password" Width="65px" CssClass="auto-style1"></asp:Label>
        <asp:TextBox ID="TextBox3" runat="server" Width="180px" CssClass="auto-style1">1</asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <span class="auto-style3">Token </span>:&nbsp;&nbsp;&nbsp; <asp:TextBox ID="TextBox7" runat="server" Width="180px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Start API Test" Width="116px" />
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
