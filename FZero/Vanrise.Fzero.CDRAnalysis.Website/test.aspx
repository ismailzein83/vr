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

            <div class="navbar navbar-fixed-top">
                <div class="navbar-inner">
                    <div class="container">
                        <br />
                        <h1>advinadv.co.uk</h1>
                        <div class="nav-collapse collapse">
                            <ul class="nav">
                                <li class="active"><a href="index.html">Home</a> </li>
                                <li class=""><a href="#sectionID2">About Us</a> </li>
                                <li class=""><a href="#sectionID3">Services</a> </li>
                                <li class=""><a href="#sectionID4">Portfolio</a> </li>
                                <li class=""><a href="contact.html">Contact</a> </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>




        </div>
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
    </form>
</body>
</html>
