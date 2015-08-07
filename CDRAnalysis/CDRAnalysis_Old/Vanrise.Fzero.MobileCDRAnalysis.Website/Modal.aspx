<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="Modal.aspx.cs" Inherits="Modal" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
      <script>
        modal.title = "Delete confirmation";
        modal.message = "Are you sure you want to delete this item?";
        </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="body">
    <asp:LinkButton runat="server" OnClientClick="return showDialog(modal.title, modal.message, true);">Delete</asp:LinkButton>
</asp:Content>
