<%@ Page Title="" Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="TABS.Plugins.Framework.Pages.PageWithCode.aspx.cs" Inherits="ModulePages_PageWithCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlace" Runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentTitlePlace" Runat="Server">Redirect and Code Behind</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="X" Runat="Server">
    <asp:Button ID="btnTest" OnClick="btnTest_Clicked" Text="Redirect Me" runat="server" />
</asp:Content>