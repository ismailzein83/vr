<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EmailTemplates.aspx.cs" Inherits="EmailTemplates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        var limitNum = 10;

        function checkLength(sender, args) {
            var editorText = args.Value;
            args.IsValid = editorText.length > limitNum;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%" class="page">
        <tr id="trAddEdit" runat="server" visible="false">
            <td>
                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget gray">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Manage Item</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table cellpadding="2" cellspacing="2" width="100%">


                                    <tr>
                                        <td></td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>Name</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtName" runat="server" Enabled="False"></telerik:RadTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Subject</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSubject" runat="server" Style="width: 750px !important;"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvSubject" runat="server" ControlToValidate="txtSubject" ErrorMessage="Subject should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Is Active</td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkIsActive" runat="server"></asp:CheckBox>

                                                    </td>
                                                </tr>


                                                <tr>
                                                    <td>Tokens</td>
                                                    <td></td>
                                                    <td>
                                                        <asp:ListView ID="lvTokens" runat="server" DataKeyNames="Id">
                                                            <AlternatingItemTemplate>
                                                                <li style="">
                                                                    <asp:Label ID="TokenLabel" runat="server" Text='<%# Eval("Token") %>' />
                                                                    <br />
                                                                </li>
                                                            </AlternatingItemTemplate>



                                                            <ItemSeparatorTemplate>
                                                                <br />
                                                            </ItemSeparatorTemplate>
                                                            <ItemTemplate>
                                                                <li style="">
                                                                    <asp:Label ID="TokenLabel" runat="server" Text='<%# Eval("Token") %>' />
                                                                    <br />
                                                                </li>
                                                            </ItemTemplate>
                                                            <LayoutTemplate>
                                                                <ul id="itemPlaceholderContainer" runat="server" style="">
                                                                    <li runat="server" id="itemPlaceholder" />
                                                                </ul>
                                                                <div style="">
                                                                </div>
                                                            </LayoutTemplate>

                                                        </asp:ListView>
                                                    </td>
                                                </tr>





                                                <tr>
                                                    <td>Message Body</td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CustomValidator runat="server" ID="cvMessageBody" ControlToValidate="txtMessageBody" ValidationGroup="Save" ClientValidationFunction="checkLength">Please fill your content. </asp:CustomValidator>

                                                        <telerik:RadEditor ID="txtMessageBody" runat="server" ToolsFile="~/Xml/BasicTools.xml" EditModes="Design" EnableResize="false" Width="750" Height="600"></telerik:RadEditor>

                                                    </td>
                                                </tr>







                                                <tr>
                                                    <td colspan="3" class=" commands">
                                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" ValidationGroup="Save">
                                                                 <i class="icon-save icon-white"></i> Save </asp:LinkButton>

                                                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-inverse" OnClick="btnCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Cancel </asp:LinkButton>
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
        <tr id="trData" runat="server" valign="top">
            <td>
                <div class="row-fluid" id="divFilter" runat="server">
                    <div class="span12">
                        <div class="widget gray">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Filters</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table width="100%">
                                    <tr id="trSearch">
                                        <td valign="top">
                                            <table class="search" width="100%" valign="top">
                                                <tr>
                                                    <td>
                                                        <table cellpadding="0" valign="top" cellspacing="0" width="100%"   >
                                                            <tr>
                                                                <td>&nbsp;</td>
                                                                <td>
                                                                    <table>
                                                                        <tr>
                                                                            <td class="caption">Name
                                                                            </td>
                                                                            <td></td>
                                                                            <td class="inputdata">
                                                                                <telerik:RadTextBox ID="txtSearchName" runat="server"></telerik:RadTextBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>




                                                                <td>&nbsp;</td>
                                                                <td>
                                                                    <table>
                                                                        <tr>
                                                                            <td class="caption">Status
                                                                            </td>
                                                                            <td></td>
                                                                            <td >
                                                                                <asp:RadioButtonList ID="rblSearchStatus" RepeatDirection="Horizontal"
                                                                                    RepeatLayout="Table" CellPadding="2" CssClass="options"
                                                                                    runat="server">
                                                                                    <asp:ListItem Value="" Selected="True">All</asp:ListItem>
                                                                                    <asp:ListItem Value="True">Active</asp:ListItem>
                                                                                    <asp:ListItem Value="False">Inactive</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>



                                                            </tr>

                                                            <tr>
                                                                <td colspan="4"  align="center" style="padding:30px 0px 0px 0px">

                                                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>

                                                                    <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-inverse" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>

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
                <div class="row-fluid" id="divData" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Results</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <telerik:RadGrid AllowSorting="true" ID="gvEmailTemplates" runat="server" CellSpacing="0" PageSize="10" BorderColor="LightGray"
                        AllowPaging="true" OnItemCommand="gvEmailTemplates_ItemCommand"
                        AutoGenerateColumns="False" OnNeedDataSource="gvEmailTemplates_NeedDataSource">


                        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                <telerik:GridCheckBoxColumn DataField="IsActive" UniqueName="IsActive" />

                                <telerik:GridTemplateColumn>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="modifyButton" runat="server" CssClass="command btn-success"
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Modify" ToolTip="Edit">
                                        <i class="icon-pencil"></i> Edit
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

            </td>
        </tr>

    </table>



    





</asp:Content>

