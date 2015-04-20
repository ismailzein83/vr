<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="ReportManagement.aspx.cs" Inherits="ReportManagement" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">

    <link rel='stylesheet' type='text/css' href="App_Themes/Styles/jquery-ui.css" />
    <script src="Scripts/jquery-ui.min.js"></script>


    <script type='text/javascript'>




        $(document).ready(function () {
            $('#theMenu').accordion({
                active: 'h3.selected',
                header: 'h3.head',
                alwaysOpen: true,
                animated: true,
                collapsible: true,
                showSpeed: 400,
                hideSpeed: 400
            });
        });


    </script>

    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget gray">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Filters</h4>

                </div>
                <div class="widget-body" style="display: block;">

                    <table cellspacing="0" cellpadding="0" style="width: 100%">
                        <tr>
                            <td style="width: 50%">
                                <table>

                                    <tr>
                                        <td style="padding-left: 20px; width: 150px">Report Number</td>
                                        <td class="inputData">

                                            <asp:TextBox runat="server" ID="txtReportNumber"></asp:TextBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 20px; width: 150px">Suspecious Number
                                        </td>
                                        <td class="inputData">

                                            <asp:TextBox runat="server" ID="txtSubscriberNumber"></asp:TextBox>
                                            <br />



                                        </td>
                                    </tr>


                                    <tr>
                                        <td style="padding-left: 20px; width: 150px">Status
                                        </td>
                                        <td class="inputData">

                                            <telerik:RadComboBox ID="ddlReportingStatus" runat="server"></telerik:RadComboBox>
                                            <br />



                                        </td>
                                    </tr>


                                </table>
                            </td>
                            <!--------------------------- right pane-------------------------------------------------->
                            <td>
                                <table>

                                    <tr>
                                        <td class="caption" style="padding-left: 20px; padding-top: 10px">From Date
                                        </td>
                                        <td class="inputData" colspan="2" style="padding-top: 15px">
                                            <telerik:RadDateTimePicker ID="dtpFromDate" runat="server" Width="220"
                                                DateInput-DisplayDateFormat="dd/MM/yyyy HH:mm:ss" DateInput-DateFormat="dd/MM/yyyy HH:mm:ss">
                                            </telerik:RadDateTimePicker>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="caption" style="padding-left: 20px; padding-bottom: 40px">To Date
                                        </td>
                                        <td class="inputData">
                                            <telerik:RadDateTimePicker ID="dtpToDate" runat="server" Width="220"
                                                DateInput-DisplayDateFormat="dd/MM/yyyy HH:mm:ss" DateInput-DateFormat="dd/MM/yyyy HH:mm:ss">
                                            </telerik:RadDateTimePicker>


                                        </td>
                                    </tr>


                                </table>
                            </td>
                            <!-----------------------------/right pane---------------------------------------------->
                        </tr>

                        <!-----------------------------check boxes --------------------------------->
                        <tr>
                            <td colspan="2" align="center" style="padding: 30px 0px 0px 0px">
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" ValidationGroup="Search">
                                                        <i class="icon-search icon-white"></i> Search </asp:LinkButton>

                                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger" OnClick="btnClear_Click">
                                    <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>

                                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-success" OnClick="btnAdd_Click">
                                                         <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>
                            </td>
                        </tr>
                        <!----------------------------- / check boxes --------------------------------->
                    </table>







                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divData" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Results</h4>
                    <span class="tools">
                        <a class="dropdown-toggle" href="#" data-toggle="dropdown">
                            <span class="username"></span></a>

                    </span>
                </div>
                <div class="widget-body" style="display: block;">

                    <telerik:RadGrid ID="gvData" runat="server" PageSize="10" AllowPaging="true" OnItemCommand="gvData_ItemCommand" OnNeedDataSource="gvData_NeedDataSource">
                        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                            <Columns>

                                <telerik:GridBoundColumn HeaderText="Status" DataField="ReportingStatu.Name" />
                                <telerik:GridBoundColumn HeaderText="Report ID" DataField="ReportID" />
                                <telerik:GridBoundColumn HeaderText="Report Number" DataField="ReportNumber" />
                                <telerik:GridBoundColumn HeaderText="Created On" DataField="ReportDate" />
                                <telerik:GridBoundColumn HeaderText="Sent On" DataField="SentDate" />




                                <telerik:GridTemplateColumn HeaderText="Numbers">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Font-Bold="true" ForeColor='<%# Eval("ReportDetails.Count").ToString()== "0" ? System.Drawing.Color.Red : System.Drawing.Color.Green %>' ID="lblNumbercount" Text='<%# Eval("ReportDetails.Count")%>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Description" DataField="Description" />
                                <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Details">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDetails" runat="server" CssClass="command btn-link" Visible='<%#((int)Eval("ReportingStatusID")== (int) Vanrise.Fzero.MobileCDRAnalysis.Enums.ReportingStatuses.ToBeSent) %>'
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Details" ToolTip="Details">
                                        <i class="icon-pencil"></i> 
                                        </asp:LinkButton>

                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>


                                <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Delete">
                                    <ItemTemplate>

                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-link" Visible='<%#((int)Eval("ReportingStatusID")== (int) Vanrise.Fzero.MobileCDRAnalysis.Enums.ReportingStatuses.ToBeSent) %>'
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Remove" ToolTip="Delete" OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                        <i class="icon-trash"></i> 
                                        </asp:LinkButton>


                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Export">
                                    <ItemTemplate>

                                        <asp:LinkButton ID="btnExport" runat="server" CssClass="command btn-link"
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Export" ToolTip="Export">
                                        <i class="icon-file"></i> 
                                        </asp:LinkButton>

                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Send">
                                    <ItemTemplate>

                                        <asp:LinkButton ID="btnSend" runat="server" CssClass="command btn-link" Visible='<%#((int)Eval("ReportingStatusID")== (int) Vanrise.Fzero.MobileCDRAnalysis.Enums.ReportingStatuses.ToBeSent) && Eval("ReportDetails.Count").ToString()!= "0" %>'
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Send" ToolTip="Send">
                                        <i class="icon-share-sign"></i> 
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>




                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>

                </div>
            </div>
        </div>
    </div>

    <!------------------------------------------------------  details          ----------------------------------->
    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget purple">
                <div class="widget-title">
                    <h4>Report Details</h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td>Report Status</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtDetailsReportStatus" ReadOnly="true"></asp:TextBox></td>
                                        <td>Report Number</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtDetailsReportNumber" ReadOnly="true"></asp:TextBox></td>
                                        <td>Creation Date</td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtDetailsCreationDate" ReadOnly="true"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <table cellspacing="0" cellpadding="1" class="table">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <div class="widget-body" style="display: block;">
                                                        <asp:GridView ID="gvDetails" runat="server" SkinID="GridDefault"
                                                            OnRowCommand="gvDetails_RowCommand">
                                                            <Columns>
                                                                <asp:BoundField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="Subscriber Number" DataField="SubscriberNumber" />
                                                                <asp:TemplateField ItemStyle-Width="30" HeaderText="Delete">
                                                                    <ItemTemplate>

                                                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-inverse" OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                                                            CommandArgument='<%#Eval("Id")%>' CommandName="Remove" ToolTip="Delete">
                                                                                 <i class="icon-trash"></i> 
                                                                        </asp:LinkButton>


                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>

                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="commandRight">
                                <asp:LinkButton ID="btnReturn" runat="server" CssClass="btn btn-danger" OnClick="btnReturn_Click" CausesValidation="false">
                                                             <i class="icon-ban-circle icon-white"></i> Return </asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="space10"></td>
                        </tr>

                    </table>

                </div>
            </div>
        </div>
    </div>
    <!------------------------------------------------------  details          ----------------------------------->
    <!------------------------------------------------------  Add New          ----------------------------------->
    <div class="row-fluid" id="divAddtion" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Add New Report</h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">

                        <tr>
                            <td>
                                <div class="span6 allborders" style="width: 650px">
                                    <h4 class="breadcrumb">Report Information</h4>

                                    <table cellspacing="0" cellpadding="1" class="table">
                                        <tbody>
                                            <tr>
                                                <td>Description
                                                </td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtDescription" runat="server" Width="500px"></asp:TextBox>
                                                    <br />
                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>


                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="space10"></td>
                        </tr>
                        <tr>
                            <td style="text-align: center;">
                                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" CausesValidation="true" ValidationGroup="Save" OnClick="btnSave_Click">
                                    <i class="icon-save icon-white"></i>
                                          Save
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-inverse" OnClick="btnCancel_Click" CausesValidation="false">
                                    <i class="icon-ban-circle icon-white"></i>
                                          Return
                                </asp:LinkButton>
                            </td>
                        </tr>

                    </table>

                </div>
            </div>
        </div>
    </div>
    <!------------------------------------------------------  details          ----------------------------------->
    <asp:HiddenField ID="hfReportID" runat="server" />
    <asp:HiddenField ID="hfReportDetailID" runat="server" />


</asp:Content>
