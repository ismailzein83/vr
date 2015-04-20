<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="App_Themes/Styles/bootstrap.css" rel="stylesheet" />
    <%--<link href="App_Themes/Styles/font-awesome.css" rel="stylesheet" />--%>
    <link href="App_Themes/Styles/style.css" rel="stylesheet" />
   <%-- <link href="App_Themes/Styles/style-red.css" rel="stylesheet" />--%>
    <%--<link href="App_Themes/Styles/pagesStyle.css" rel="stylesheet" />--%>




    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <%--<script src="Scripts/ModalScript.js"></script>--%>
    <script src="Scripts/bootstrap.min.js"></script>
    <%--<script src="Scripts/common-scripts.js"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Button Styles</h2>
            <button type="button" class="btn btn-default">Default</button>
            <button type="button" class="btn btn-primary">Primary</button>
            <button type="button" class="btn btn-success">Success</button>
            <button type="button" class="btn btn-info">Info</button>
            <button type="button" class="btn btn-warning">Warning</button>
            <button type="button" class="btn btn-danger">Danger</button>
            <button type="button" class="btn btn-link">Link</button>
        </div>



    </form>
</body>
</html>
