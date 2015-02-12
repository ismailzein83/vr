<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelatedNumbers.aspx.cs" Inherits="RelatedNumbers" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%">


        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">
                            <div class="row-fluid" id="divFilter" runat="server">
                                <div class="span12">
                                    <div class="widget blue">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i>Search Filters</h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <table >
                                                            <tr>
                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%= Resources.Resources.MobileOperator %>
                                                                </td>
                                                                <td></td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="ddlMobileOperators" runat="server">
                                                                    </telerik:RadComboBox>
                                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvMobileOperators" runat="server" ControlToValidate="ddlMobileOperators" ErrorMessage="MobileOperator should not be empty " ValidationGroup="Import"></asp:RequiredFieldValidator>

                                                                </td>



                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%= Resources.Resources.Report %>
                                                                </td>
                                                                <td></td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="ddlReport" runat="server" EnableTextSelection="true">
                                                                    </telerik:RadComboBox>
                                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvReport" runat="server" ControlToValidate="ddlReport" ErrorMessage="Report should not be empty " ValidationGroup="Import"></asp:RequiredFieldValidator>

                                                                </td>



                                                                <td class="caption">
                                                                    <asp:Label ID="lblAllowedExtensions" runat="server" ForeColor="Gray" Font-Italic="true" Text="Select files to import (.xls,.xlsx,.xml)"></asp:Label>

                                                                </td>
                                                                <td></td>
                                                                <td>
                                                                    <telerik:RadUpload ID="ruImportedFile" OverwriteExistingFiles="true" InitialFileInputsCount="1" MaxFileInputsCount="1" runat="server" AllowedFileExtensions="xls,xlsx,xml" ControlObjectsVisibility="None" />
                                                                </td>


                                                            </tr>
                                                            <tr>
                                                                <td colspan="12">
                                                                    <table cellpadding="3" cellspacing="2" border="0" align="center">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:LinkButton ID="btnConfirm" runat="server" CssClass="btn btn-success" OnClick="btnConfirm_Click" CausesValidation="true" ValidationGroup="Confirm">
                                                             <i class="icon-save icon-white"></i>
                                                                 Confirm
                                                                                </asp:LinkButton>


                                                                            </td>
                                                                            <td>

                                                                                <asp:LinkButton ID="btnImport" runat="server" CssClass="btn btn-inverse" OnClick="btnImport_Click" CausesValidation="true" ValidationGroup="Import">
                                                             <i class="icon-barcode icon-white"></i>
                                                                 Import
                                                                                </asp:LinkButton>





                                                                            </td>
                                                                            <td class="hspace"></td>
                                                                            <td>

                                                                                <asp:LinkButton ID="btnImportClear" runat="server" CssClass="btn btn-danger" OnClick="btnImportClear_Click" CausesValidation="False">
                                                             <i class="icon-undo icon-white"></i>
                                                                 Clear
                                                                                </asp:LinkButton>



                                                                            </td>

                                                                        </tr>
                                                                    </table>

                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="row-fluid" id="divData" runat="server">
                                            <div class="span12">
                                                <div class="widget blue">
                                                    <div class="widget-title">
                                                        <h4><i class="icon-reorder"></i>Search Results</h4>

                                                    </div>
                                                    <div class="widget-body" style="display: block;">
                                                        <telerik:RadGrid ID="gvImportedCalls" runat="server"
                                                            AutoGenerateColumns="False">

                                                            <MasterTableView>
                                                                <Columns>
                                                                    <telerik:GridBoundColumn DataField="Related Number" UniqueName="RelatedNumber" />
                                                                </Columns>
                                                            </MasterTableView>
                                                        </telerik:RadGrid>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                    </td>
                                </tr>
                            </table>


                        </td>
                    </tr>
                </table>
            </td>
        </tr>

    </table>
</asp:Content>

