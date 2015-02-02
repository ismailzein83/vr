<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EmailTemplates.aspx.cs" Inherits="EmailTemplates" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
      var limitNum = 10;

      function checkLength(sender, args) {
          var editorText = args.Value;
          args.IsValid = editorText.length > limitNum;
      }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
 
      <table cellpadding="0" cellspacing="0" width="100%" >
        <tr id="trAddEdit" runat="server" visible="false">
            <td class="section">
                <table width="100%">
                    <tr>
                        <td ></td>
                        <td class="top">
                            <asp:Label ID="lblSectionName" runat="server"></asp:Label></td>
                    </tr>
                    <tr >
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td ></td>
                        <td>
                            <table>
                                <tr>
                                    <td class="caption"><%=Resources.Resources.Name %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtName" runat="server" Enabled="False"></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="caption"><%=Resources.Resources.Subject %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                          <telerik:RadTextBox ID="txtSubject" runat="server" style="width:750px !important;"   ></telerik:RadTextBox>
                            <asp:RequiredFieldValidator CssClass="error" ID="rfvSubject" runat="server" ControlToValidate="txtSubject" ErrorMessage="Subject should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="caption"><%=Resources.Resources.IsActive %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                      <asp:CheckBox ID="chkIsActive" runat="server"></asp:CheckBox>
                                       
                                    </td>
                                </tr>


                                 <tr>
                                    <td class="caption"><%=Resources.Resources.Tokens %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                         <asp:ListView ID="lvTokens" runat="server" DataKeyNames="Id" >
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
                                    <td class="caption"><%=Resources.Resources.MessageBody %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                       <asp:CustomValidator runat="server" ID="cvMessageBody" ControlToValidate="txtMessageBody" ValidationGroup="Save" ClientValidationFunction="checkLength">Please fill your content. </asp:CustomValidator>  

                            <telerik:RadEditor ID="txtMessageBody" Runat="server" ToolsFile="~/Xml/BasicTools.xml" EditModes="Design"   EnableResize="false" Width="750" Height="600" > </telerik:RadEditor>
                                       
                                    </td>
                                </tr>


                             




                                <tr>
                                    <td colspan="3" class=" commands">
                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                       <telerik:RadButton ID="btnSave" runat="server" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                            <Icon PrimaryIconUrl="Icons/save.png" />
                                        </telerik:RadButton>
                                        <telerik:RadButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" CausesValidation="False" >
                                            <Icon PrimaryIconUrl="Icons/cancel.png" />
                                        </telerik:RadButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">
                            <table  width="100%" valign="top">
                                <tr>
                                    <td>
                                        <table cellpadding="0" valign="top" cellspacing="0" >
                                            <tr>
                                                <td >&nbsp;</td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="caption">
                                                                <%=Resources.Resources.Name %>
                                                            </td>
                                                            <td ></td>
                                                            <td class="inputdata">
                                                                 <telerik:RadTextBox ID="txtSearchName" runat="server"></telerik:RadTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                               
                                              

                                          
                                                <td >&nbsp;</td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="caption">
                                                                <%=Resources.Resources.Status %>
                                                            </td>
                                                            <td ></td>
                                                            <td class="inputdata">
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




                                        </table>
                                        <tr id="trSearchCommands">
                                            <td class="commands">
                                                 <table cellpadding="3" cellspacing="2" border="0" align="center">
                                                    <tr>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearch" runat="server" CausesValidation="False"
                                                                OnClick="btnSearch_Click">
                                                                <Icon PrimaryIconUrl="Icons/search_16.png" />
                                                            </telerik:RadButton>
                                                        </td>
                                                        <td class="hspace"></td>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearchClear" runat="server" CausesValidation="False"
                                                                OnClick="btnSearchClear_Click" >
                                                                <Icon PrimaryIconUrl="Icons/clear.png" />
                                                            </telerik:RadButton>
                                                        </td>

                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </td>
                                </tr>
                            </table>
                        </td>
        </tr>
      
        <tr >
            <td valign="top"  >
                <table width="100%">
                    <tr>
                        <td  >
                             <telerik:RadGrid  AllowSorting="true" ID="gvEmailTemplates" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                            AllowPaging="true" OnItemCommand="gvEmailTemplates_ItemCommand"
                                            AutoGenerateColumns="False" OnNeedDataSource="gvEmailTemplates_NeedDataSource">
                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                                    <telerik:GridCheckBoxColumn DataField="IsActive" UniqueName="IsActive" />
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <telerik:RadButton ButtonType="StandardButton" ID="btnEdit" runat="server"
                                                                CommandArgument='<%#Eval("Id")%>' CommandName="Modify" Text='<%# Resources.Resources.Edit %>'>

                                                            <Icon PrimaryIconUrl="~/Icons/edit.gif"  />
                                                            </telerik:RadButton>

                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
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

