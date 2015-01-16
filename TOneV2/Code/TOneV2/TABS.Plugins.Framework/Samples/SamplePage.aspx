﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.UI.Page" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnTest.Text += " Modified";
        }
    }

    void btnTest_Clicked(object sender, EventArgs e)
    {
        btnTest.Text = "I have been clicked";
    }
    
</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnTest" Text="Click Me" runat="server" OnClick="btnTest_Clicked" />     
    </div>
    </form>
</body>
</html>
