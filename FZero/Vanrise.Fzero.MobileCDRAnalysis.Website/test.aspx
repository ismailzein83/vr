<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <telerik:RadSlider ID="RadSlider1" runat="server"  ItemType="Item"      Height="50px" Length="300" Width="1000px">
                <Items>
                    <telerik:RadSliderItem runat="server" Text="-75%" Value="0.25" />
                    <telerik:RadSliderItem runat="server" Text="-50%" Value="0.50" />
                    <telerik:RadSliderItem runat="server" Text="-25%" Value="0.75" />
                    <telerik:RadSliderItem runat="server" Text="0%" Value="1"  />
                    <telerik:RadSliderItem runat="server" Text="25%" Value="1.25" />
                    <telerik:RadSliderItem runat="server" Text="50%" Value="1.50" />
                    <telerik:RadSliderItem runat="server" Text="75%" Value="0.75" />
                    
                </Items>
            </telerik:RadSlider>

        </div>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
    </form>
</body>
</html>
