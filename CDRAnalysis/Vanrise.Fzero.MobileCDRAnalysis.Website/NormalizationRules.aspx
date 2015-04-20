<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="NormalizationRules.aspx.cs" Inherits="NormalizationRules" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <script>
        function cvRuleValidation_ClientValidate(source, args) {

            if (document.getElementById("<%= txtPrefixToAdd.ClientID %>").value == "" &&
                document.getElementById("<%= txtSuffixToAdd.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringLength.ClientID %>").value == "") {
                args.IsValid = true;
            }
            else {
                if ((document.getElementById("<%= txtSubstringLength.ClientID %>").value != "" &&
                document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value == "")
                    ||
                    (document.getElementById("<%= txtSubstringLength.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value != "")) {
                    args.IsValid = false;
                }
                else {
                    args.IsValid = true;
                }
            }
        }

        function validateUpdateRule() {
            var customValidator = document.getElementById('<%= cvRuleValidation.ClientID %>');
            if (document.getElementById("<%= txtPrefixToAdd.ClientID %>").value == "" &&
                document.getElementById("<%= txtSuffixToAdd.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringLength.ClientID %>").value == "") {
                //customValidator.isvalid = false;
                //customValidator.style.display = "";
                customValidator.isvalid = true;
                customValidator.style.display = "none";
            }
            else {
                if ((document.getElementById("<%= txtSubstringLength.ClientID %>").value != "" &&
               document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value == "")
                    ||
                    (document.getElementById("<%= txtSubstringLength.ClientID %>").value == "" &&
                document.getElementById("<%= txtSubstringStartIndex.ClientID %>").value != "")) {
                    customValidator.isvalid = false;
                    customValidator.style.display = "";
                }
                else {
                    customValidator.isvalid = true;
                    customValidator.style.display = "none";
                }
            }
        }
    </script>
    
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Normalization Rules</h4>
                 
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width:100%">
                        <tr>
                            <td class="caption">Source
                            </td>
                            <td class="inputData">
                                <Telerik:RadComboBox ID="ddlSearchSwitches" runat="server"  ></Telerik:RadComboBox>
                            </td>
                        </tr>

                       
                        <tr>
                            <td class="caption">Party
                            </td>
                            <td class="inputData">
                                <asp:RadioButtonList ID="rblstSearchParty" runat="server" TextAlign="Right" CssClass="radioButtons"
                                    RepeatDirection="Horizontal" CellPadding="8" RepeatLayout="Table">
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="caption">Length
                            </td>
                            <td class="inputData">
                                <asp:TextBox ID="txtSearchLength" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="caption">Prefix
                            </td>
                            <td class="inputData">
                                <asp:TextBox ID="txtSearchPrefix" runat="server"></asp:TextBox>
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
                    <h4><i class="icon-reorder"></i>Normalization Rules Data</h4>
                  
                </div>
                <div class="widget-body" style="display: block;">
                    <div class="span12">
                        <div class="dataTables_length" id="sample_1_length">
                           
                        </div>
                    </div>
                    <div class="widget-body" style="display: block;">
                        <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                            OnRowCommand="gvData_RowCommand" HorizontalAlign="Left">
                            <Columns>
                                <asp:BoundField HeaderText="Switch Name" DataField="SwitchProfile.Name" />
                                <asp:BoundField HeaderText="Party" DataField="Party" />
                                <asp:BoundField HeaderText="Party Length" DataField="CallLength" />
                                <asp:BoundField HeaderText="Prefix" DataField="Prefix" />
                                <asp:BoundField HeaderText="Rule" DataField="UpdateStatement" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="command btn-success"
                                            CommandArgument='<%# Eval("Id") %>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-danger"
                                            CommandArgument='<%# Eval("Id") %>' CommandName="Remove" OnClientClick="return confirm('Are you sure you want to delete this rule?');">
                                        <i class="icon-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="row-fluid">
                        <div class="span6">
                            <div class="dataTables_info" id="sample_1_info">Displaying <%= pagination.StartRecordNumber %> to <%= pagination.EndRecordNumber %> of <%= pagination.RecordsCount %> rules</div>
                        </div>
                        <div class="span6">
                            <div class="dataTables_paginate paging_bootstrap pagination">
                                <span>Page </span>
                                <asp:DropDownList CssClass="input-small" runat="server" ID="ddlPages" AutoPostBack="true" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                                </asp:DropDownList>
                                <span>of </span>
                                <span id="lblPagesCount"><%= pagination.PagesCount %></span>
                            </div>
                             <label>rules per page

                                  <asp:DropDownList  ID="ddlPageSizes" CssClass="input-mini" runat="server"  AutoPostBack="true" OnSelectedIndexChanged="ddlPageSizes_SelectedIndexChanged">
                                    <Items>
                                        <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                        <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                        <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                    </Items>

                                </asp:DropDownList>
                                </label>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Rule Conditions</h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <table class="table">
                                            <tr>
                                                <td class="caption required">Source</td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlSwitches" runat="server"  ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvSwitch" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlSwitches" InitialValue="0"
                                                        ErrorMessage="The switch should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                                <td class="space10"></td>
                                                  <td class="caption required" runat="server" id="tdIgnore">Ignore</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtIgnore" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator ID="rfvIgnore" runat="server" Display="Dynamic"
                                                        CssClass="error" ControlToValidate="txtIgnore" ValidationGroup="Save"
                                                        ErrorMessage="'Ignore' field should not be empty"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="cvIgnoreInteger" runat="server" Display="Dynamic"
                                                        CssClass="error" ValidationGroup="Save"
                                                        Operator="DataTypeCheck" Type="Integer" ControlToValidate="txtIgnore"
                                                        ErrorMessage="'Ignore' field should be a Number"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required">Party</td>
                                                <td class="inputData" >
                                                    <asp:RadioButtonList ID="rblstParty" runat="server" CssClass="radioButtons"
                                                        RepeatDirection="Horizontal" RepeatLayout="Table"
                                                        CellPadding="8" AutoPostBack="True" OnSelectedIndexChanged="rblstParty_SelectedIndexChanged">
                                                    </asp:RadioButtonList>
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvParty" runat="server" Display="Dynamic"
                                                        ControlToValidate="rblstParty"
                                                        ErrorMessage="A Party should be chosen" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                                 <td class="space10"></td>
                                                 <td class="caption required">Length</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtLength" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:CompareValidator ID="cvLengthInteger" runat="server" Display="Dynamic"
                                                        CssClass="error" ValidationGroup="Save"
                                                        Operator="DataTypeCheck" Type="Integer" ControlToValidate="txtLength"
                                                        ErrorMessage="'Length' field should be a Number"></asp:CompareValidator>
                                                </td>
                                             
                                            </tr>
                                            
                                            <tr>
                                                <td class="caption required">Prefix</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtPrefix" runat="server" MaxLength="3"></asp:TextBox>
                                                </td>
                                                <td class="space10"></td>
                                                <td ></td>
                                                <td ></td>
                                               
                                            </tr>
                                          

                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>

                    </table>
                </div>
            </div>

            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Rule To Apply </h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <table class="table">
                                            <tr>
                                                <td class="caption">Update Rule</td>
                                                <td colspan="4" class="">
                                                    <table width="100%" class="no-padding">
                                                        <tr class="label-mini">
                                                            <td>Prefix To Add</td>
                                                            <td></td>
                                                            <td>Start index</td>
                                                            <td>Length</td>
                                                            <td>Suffix To Add</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="txtPrefixToAdd" runat="server" CssClass="input-mini" MaxLength="3"
                                                                    onchange="validateUpdateRule();">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td style="width: 198px; min-width: 198px;"><b>+ Substring(</b> [CDPN] | [CGPN]  <b>,</b> </td>
                                                            <td>
                                                                <asp:TextBox CssClass="input-mini" ID="txtSubstringStartIndex" runat="server"
                                                                    onchange="validateUpdateRule();">
                                                                </asp:TextBox>
                                                                <b>,</b>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtSubstringLength" runat="server" CssClass="input-mini"
                                                                    onchange="validateUpdateRule();">
                                                                </asp:TextBox>
                                                                <b>) + </b>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtSuffixToAdd" runat="server" CssClass="input-mini" MaxLength="3"
                                                                    onchange="validateUpdateRule();"></asp:TextBox></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td></td>
                                                            <td>
                                                                <asp:CompareValidator ID="cvSubstringStartIndexNumber" runat="server" Display="Dynamic"
                                                                    CssClass="error"
                                                                    Operator="DataTypeCheck" Type="Integer" ControlToValidate="txtSubstringStartIndex"
                                                                    ErrorMessage="Start Index should be a Number" />
                                                            </td>
                                                            <td>
                                                                <asp:CompareValidator ID="cvSubstringLengthNumber" runat="server" Display="Dynamic"
                                                                    CssClass="error"
                                                                    Operator="DataTypeCheck" Type="Integer" ControlToValidate="txtSubstringLength"
                                                                    ErrorMessage="Length should be a Number" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="5">
                                                                <asp:CustomValidator ID="cvRuleValidation" runat="server" Display="Dynamic"
                                                                    ValidationGroup="Save" CssClass="error"
                                                                    ErrorMessage="You should enter the substring start index <u> and </u> the substring length"
                                                                    OnServerValidate="cvRuleValidation_ServerValidate"
                                                                    ClientValidationFunction="cvRuleValidation_ClientValidate" />
                                                                
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>

                    </table>
                </div>
            </div>

            <div class="no-border" style="text-align: center">
                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                    <i class="icon-save icon-white"></i>
                                          Save
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                    <i class="icon-ban-circle icon-white"></i>
                                          Cancel
                </asp:LinkButton>
            </div>

        </div>



    </div>
</asp:Content>
