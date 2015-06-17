<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="UnNormalizedCalls.aspx.cs" Inherits="UnNormalizedCalls" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
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
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget gray">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Filters</h4>
                  
                </div>
                <div class="widget-body" style="display: block;">
                    <table>
                        <tr>
                            <td class="caption">Source
                            </td>
                            <td >
                                <Telerik:RadComboBox ID="ddlSearchSwitches" runat="server"      ></Telerik:RadComboBox>
                            </td>
                            <td class="validation">
                                <asp:RequiredFieldValidator ID="rfvSwitch" runat="server" Display="Dynamic"
                                    ControlToValidate="ddlSearchSwitches" InitialValue="0"
                                    ErrorMessage="You should select a switch" ValidationGroup="Search"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="caption">From Date
                            </td>
                            <td class="inputData" colspan="2">
                                <Telerik:RadDatePicker ID="dtpFromDate" runat="server" Width="220"
                                    DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                </Telerik:RadDatePicker>
                            </td>
                        </tr>
                        <tr>
                            <td class="caption">To Date
                            </td>
                            <td class="inputData">
                                <Telerik:RadDatePicker ID="dtpToDate" runat="server"
                                    DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                </Telerik:RadDatePicker>
                            </td>
                            <td class="validation">
                                <asp:CompareValidator ID="dateCompareValidator" runat="server" ControlToValidate="dtpToDate"
                                    ControlToCompare="dtpFromDate" Operator="GreaterThan" Type="Date" ValidationGroup="Search"
                                    ErrorMessage="To Date should be greater than From Date." CssClass="error">
                                </asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="caption">Party
                            </td>
                            <td class="inputData" colspan="2">
                                <asp:RadioButtonList ID="rblstSearchParty" runat="server" TextAlign="Right" CssClass="radioButtons"
                                    RepeatDirection="Horizontal" CellPadding="8" RepeatLayout="Table">
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;
                            </td>
                            <td colspan="2">
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary"
                                    ValidationGroup="Search" CausesValidation="true"
                                    OnClick="btnSearch_Click">
                                    <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger"
                                    CausesValidation="false"
                                    OnClick="btnClear_Click">
                                    <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>
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
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>
                            <asp:BoundField HeaderText="Trunk name" DataField="TrunckName" />
                            <asp:BoundField HeaderText="Party" DataField="Party" />
                            <asp:BoundField HeaderText="Calls Count" DataField="CallsCount" />
                            <asp:BoundField HeaderText="Calls Duration" DataField="CallsDurations" />
                            <asp:BoundField HeaderText="Prefix" DataField="Prefix" />
                            <asp:BoundField HeaderText="Length" DataField="Length" />
                            <asp:TemplateField >
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnToRule" runat="server" CssClass="command btn-success"
                                        CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="MakeRule" ToolTip="Add rule">
                                        <i class="icon-plus"></i> Rule
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
     <!---   new Rule  ---->

    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Rule Details</h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <table class="table">
                                            <tr>
                                                <td class="caption required">Switch</td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlSwitches" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="RequiredFieldValidator1" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlSwitches" InitialValue="0"
                                                        ErrorMessage="The switch should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                                <td class="space10"></td>
                                                <td class="caption required">Trunk Name</td>
                                                <td>
                                                    <Telerik:RadComboBox ID="ddlTruncks" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvTruncks" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlTruncks" InitialValue="0"
                                                        ErrorMessage="A trunk should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required">Party</td>
                                                <td class="inputData" colspan="4">
                                                    <asp:RadioButtonList ID="rblstParty" runat="server" CssClass="radioButtons"
                                                        RepeatDirection="Horizontal" RepeatLayout="Table"
                                                        CellPadding="8">
                                                    </asp:RadioButtonList>
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvParty" runat="server" Display="Dynamic"
                                                        ControlToValidate="rblstParty"
                                                        ErrorMessage="A Party should be chosen" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                                <%-- <td class="space10"></td>
                                                <td class="caption required">Trunck Name</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtTrunckName" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvTrunck" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtTrunckName" ErrorMessage="<br />Trunck should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>--%>
                                            </tr>
                                            <tr id="trStats" runat="server" visible="false">
                                                <td class="caption">Calls Count</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtCallsCount" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:CompareValidator ID="cvCallsCount" runat="server" Display="Dynamic"
                                                        CssClass="error"
                                                        Operator="DataTypeCheck" Type="Double" ControlToValidate="txtCallsCount"
                                                        ErrorMessage="'Calls Count' field should be a Decimal"></asp:CompareValidator>
                                                </td>
                                                <td class="space10"></td>
                                                <td class="caption">Duration</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtDuration" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:CompareValidator ID="cvDurationDecimal" runat="server" Display="Dynamic"
                                                        CssClass="error"
                                                        Operator="DataTypeCheck" Type="Double" ControlToValidate="txtDuration"
                                                        ErrorMessage="'Duration' field should be a Decimal"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required">Prefix</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtPrefix" runat="server" MaxLength="3"></asp:TextBox>
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
                                                <td class="caption">Supplement</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtSupplement" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="space10"></td>
                                                <td  runat="server" id="tdIgnore"  class="caption required">Ignore</td>
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
                                                <td class="caption">Update Rule</td>
                                                <td colspan="4" class="">
                                                    <table width="100%" class="no-padding">
                                                        <tr class="label-mini">
                                                            <td>Prefix</td>
                                                            <td></td>
                                                            <td>Start index</td>
                                                            <td>Length</td>
                                                            <td>Suffix</td>
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
                                                                <%--ErrorMessage="You should fill the <b>prefix</b> and/or <b>substring index <u>and</u> length</b> and/or the <b>suffix</b>"--%>
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
                        <tr class="no-border">
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



    <!----    --->






</asp:Content>
