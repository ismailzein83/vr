<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="TONEAPI.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            color: #FF0000;
        }
        .auto-style2 {
            width: 729px;
            margin-left: 40px;
        }
        .auto-style3 {
            width: 756px;
        }
        #form1 {
            background-color: #7B97B5;
        }
        .auto-style4 {
            width: 58px;
            margin-left: 40px;
        }
        .auto-style5 {
            width: 577px;
            margin-left: 40px;
        }
        .auto-style6 {
            text-align: center;
        }
        .auto-style7 {
            background-color: #5D7B9D;
        }
        .auto-style8 {
            color: #00FF00;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="background-color: #6D8AAA">
    
        <div class="auto-style6">
            <strong>
    
        <span class="auto-style8">T.One V2 GUI Automation Testing</span><span class="auto-style1"><br />
            </span>
        </strong><br class="auto-style7" />
        Settings
        </div>
        <table border="1" style="width:100%;">
            <tr>
                <td>
        <asp:Label ID="Label1" runat="server" Text="URL" Width="65px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox1" runat="server" Width="265px" style="background-color: #99FF33">http://192.168.110.195:8103</asp:TextBox>
                </td>
                <td>
        <asp:Label ID="Label7" runat="server" Text="Wait browser time" Width="135px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox8" runat="server" Width="265px" style="background-color: #99FF33">2000</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="Label2" runat="server" Text="Username" Width="65px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox2" runat="server" Width="266px" style="background-color: #99FF33">admin@vanrise.com</asp:TextBox>
                </td>
                <td>
        <asp:Label ID="Label8" runat="server" Text="Configuration DB" Width="155px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox9" runat="server" Width="517px" style="background-color: #99FF33">Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="Label3" runat="server" Text="Password" Width="65px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox3" runat="server" Width="265px" style="background-color: #99FF33">1</asp:TextBox>
                </td>
                <td>
        <asp:Label ID="Label9" runat="server" Text="Business Entity DB" Width="155px"></asp:Label>
                </td>
                <td>
        <asp:TextBox ID="TextBox10" runat="server" Width="516px" style="background-color: #99FF33">Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=no@cce$$dev</asp:TextBox>
                </td>
            </tr>
        </table>
        <table border="1" style="width:100%;">
            <tr>
                <td class="auto-style2" colspan="2">Get and the load the web application menu and its corresponding sub menus and store them into DB, this action is essential for the GUI Automation process</td>
                <td class="auto-style3">
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="Get Menu Pages" BorderStyle="Groove" Height="40px"  style="background-color: #00CC00" Width="147px" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Start API Test" Width="116px" style="background-color: #00FF00" Visible="False" />
                </td>
            </tr>
            <tr>
                <td class="auto-style5">Starts the GUI Testing of a Certain module selected from the dropdown&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </td>
                <td class="auto-style4">
                    <asp:DropDownList ID="DropDownList1" runat="server" Height="20px" Width="147px">
                    </asp:DropDownList>
                    </td>
                <td class="auto-style3">
                    <asp:Button ID="Button5" runat="server" BorderStyle="Groove" Height="40px" OnClick="Button5_Click" style="background-color: #00CC00" Text="Start Selected Module" Width="147px" />
                    <asp:Button ID="Button6" runat="server" OnClick="Button6_Click" Text="Button" />
                </td>
            </tr>
            <tr>
                <td class="auto-style2" colspan="2">Start All Application GUI Testing</td>
                <td class="auto-style3">
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Start All GUI" style="background-color: #00CC00;" BorderStyle="Groove" Height="37px" Width="147px" />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Unit Test" Width="144px" />
        <asp:TextBox ID="TextBox7" runat="server" Width="102px" Visible="False"></asp:TextBox>
                    <asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="Button" />
                    <asp:TextBox ID="TextBox11" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <hr />
        <br />
        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" Height="290px" Width="1665px" AllowCustomPaging="True" AllowPaging="True" EnableSortingAndPagingCallbacks="True" PageSize="50" ShowFooter="True" ShowHeaderWhenEmpty="True">
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
        <asp:TextBox ID="TextBox5" runat="server" BackColor="White" Height="600px" TextMode="MultiLine" Width="420px" style="background-color: #CCCCCC"></asp:TextBox>
        <asp:TextBox ID="TextBox6" runat="server" BackColor="#FF6666" Height="600px" TextMode="MultiLine" Width="420px" style="color: #FFFFFF; background-color: #4D6082"></asp:TextBox>
    </form>
</body>
</html>
