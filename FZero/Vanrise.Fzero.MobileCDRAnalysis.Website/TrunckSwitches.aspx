<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="TrunckSwitches.aspx.cs" Inherits="TrunckSwitches" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<asp:Content runat="server" ContentPlaceHolderID="head">
    <script>
        function CheckDirections(ctrl) {
            var direction1_In = document.getElementById('<%= rdlstDirection1.ClientID + "_0" %>');
            var direction1_Out = document.getElementById('<%= rdlstDirection1.ClientID + "_1" %>');
            var direction2_In = document.getElementById('<%= rdlstDirection2.ClientID + "_0" %>');
            var direction2_Out = document.getElementById('<%= rdlstDirection2.ClientID + "_1" %>');
            if ((direction1_In.checked && direction2_In.checked) ||
                (direction1_Out.checked && direction2_Out.checked)) {
                ctrl.checked = false;
                alert("The 2 trunks can't have the same directions");
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Trunks</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <table>
                        <tr>
                            <td class="caption">Trunk Name
                            </td>
                            <td class="inputData">
                                <Telerik:RadTextBox ID="txtSearchTrunckName"   runat="server"></Telerik:RadTextBox>
                            </td>
                        </tr>

                        <tr>
                            <td class="caption">Switch Name
                            </td>
                            <td class="inputData">
                                <Telerik:RadTextBox ID="txtSearchSwitchName"   runat="server"></Telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                    <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger" OnClick="btnClear_Click">
                                    <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>
                                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-success" OnClick="btnAdd_Click">
                                    <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>
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
                    <h4><i class="icon-reorder"></i>Trunks Data</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        DataKeyNames="Id, SwitchId, TrunckId"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>
                            <asp:BoundField HeaderText="Trunk Id" DataField="TrunckId" />
                            <asp:BoundField HeaderText="Switch Name" DataField="SwitchProfile.Name" />
                            <asp:BoundField HeaderText="Trunk Name" DataField="Name" />
                            <asp:BoundField HeaderText="Direction" DataField="Direction.Name" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="command btn-success"
                                        CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-danger"
                                        CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="Remove"
                                        OnClientClick="return confirm('Deleting this switch truck will also delete the switch trunk connection on the other side. </br> are you sure you want to delete them?');">
                                       <%-- OnClientClick="return showDialog(this, 'Delete Confirmation', 'Are you sure you want to delete this item?');">--%>
                                        <i class="icon-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Trunks Details</h4>
                </div>

                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr id="trTrunckId">
                            <td>
                                <table style="width:100%;" class="table">
                                    <tr>
                                        <td class="caption"><asp:Label ID="Label4" runat="server" Text="Trunk Id"></asp:Label></td>
                                        <td class="inputData">
                                            <Telerik:RadTextBox ID="txtTrunckId" runat="server" Enabled="false"></Telerik:RadTextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb"> <asp:Label ID="Label5" runat="server" Text="First Switch"></asp:Label></h4>
                                    <table class="table">
                                        <tr>
                                            <td class="caption required"> <asp:Label ID="Label6" runat="server" Text="Switch"></asp:Label></td>
                                            <td class="inputData">
                                                <Telerik:RadComboBox ID="ddlSwitches1" runat="server"    ></Telerik:RadComboBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvSwitch1" runat="server" Display="Dynamic"
                                                    ControlToValidate="ddlSwitches1" InitialValue="0"
                                                    ErrorMessage="The switch should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required"> <asp:Label ID="Label7" runat="server" Text="Trunk Name"></asp:Label></td>
                                            <td class="inputData">
                                                <Telerik:RadTextBox ID="txtTrunckName1" runat="server"></Telerik:RadTextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvTrunckName1" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtTrunckName1" ErrorMessage="Trunk Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required"> <asp:Label ID="Label1" runat="server" Text="Trunk Full Name"></asp:Label>  </td>
                                            <td class="inputData">
                                                <Telerik:RadTextBox ID="txtTrunckFullName1" runat="server"></Telerik:RadTextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvTrunckFullName1" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtTrunckFullName1" ErrorMessage="Trunk Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required"> <asp:Label ID="Label2" runat="server" Text="Direction"></asp:Label> </td>
                                            <td class="inputData">
                                                <asp:RadioButtonList ID="rdlstDirection1" runat="server"
                                                    RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="radioButtons">
                                                </asp:RadioButtonList>
                                                 <asp:RequiredFieldValidator CssClass="error" ID="rfvDirection1" runat="server" Display="Dynamic"
                                                    ControlToValidate="rdlstDirection1" InitialValue=""
                                                    ErrorMessage="The direction should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb"> <asp:Label ID="Label8" runat="server" Text="Second Switch"></asp:Label></h4>
                                    <table class="table">
                                        <tbody>
                                            <tr>
                                                <td class="caption required"> <asp:Label ID="Label9" runat="server" Text="Switch"></asp:Label></td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlSwitches2" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvSwitch2" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlSwitches2" InitialValue="0"
                                                        ErrorMessage="Switch should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required"><asp:Label ID="Label10" runat="server" Text="Trunk Name"></asp:Label></td>
                                                <td class="inputData">
                                                    <Telerik:RadTextBox ID="txtTrunckName2" runat="server"></Telerik:RadTextBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvTrunckName2" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtTrunckName2" ErrorMessage="Trunk Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required"> <asp:Label ID="Label3" runat="server" Text="Trunk Full Name"></asp:Label>   </td>
                                                <td class="inputData">
                                                    <Telerik:RadTextBox ID="txtTrunckFullName2" runat="server"></Telerik:RadTextBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvTrunckFullName2" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtTrunckFullName2" ErrorMessage="Trunk Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required"><asp:Label ID="Label11" runat="server" Text="Direction"></asp:Label></td>
                                                <td class="inputData">
                                                    <asp:RadioButtonList ID="rdlstDirection2" runat="server"
                                                        RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="radioButtons">
                                                    </asp:RadioButtonList>
                                                 <asp:RequiredFieldValidator CssClass="error" ID="rfvDirection2" runat="server" Display="Dynamic"
                                                    ControlToValidate="rdlstDirection2" InitialValue=""
                                                    ErrorMessage="The direction should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
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
                                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                    <i class="icon-save icon-white"></i>
                                          Save
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                    <i class="icon-ban-circle icon-white"></i>
                                          Cancel
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
