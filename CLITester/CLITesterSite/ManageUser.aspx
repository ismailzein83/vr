<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ManageUser.aspx.cs" Inherits="ManageUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">
                Manage User
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage User</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <div class="row-fluid">
        <div class="span12">
            <div class="portlet box blue">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-cogs"></i>User</div>
                    <div class="tools">
                        <a href="javascript:;" class="collapse"></a>
                    </div>
                </div>
                <div class="portlet-body form">
                    <!-- BEGIN FORM-->
                    <form action="#" class="horizontal-form">
                    <h3 class="form-section">
                        User</h3>

                    <div class="row-fluid">
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="firstName">
                                    First Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="m-wrap span12" placeholder="First Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtName"
                                          ErrorMessage="Name Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="firstName">
                                    Last Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="m-wrap span12" placeholder="Last Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtLastName"
                                          ErrorMessage="Last Name Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>

                        
                    <div class="row-fluid">
                         <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Username<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="m-wrap span12" placeholder="UserName"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtUserName"
                                        ErrorMessage="Username Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->

                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="firstName">
                                    Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" CssClass="m-wrap span12"
                                        placeholder="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                                         ErrorMessage="Password Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>

                    <!--/row-->
                    <div class="row-fluid">
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Mobile<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="m-wrap span12" placeholder="Mobile"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtMobile"
                                          ErrorMessage="Mobile Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Email<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="m-wrap span12" placeholder="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail"
                                          ErrorMessage="Email Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="Regularexpressionvalidator2" runat="server" ErrorMessage='<%# GetEntry("InvalidEmail") %>'
                                        Display="Dynamic" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        Font-Bold="true" ForeColor="Red" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>
                    <!--/row-->
              <%--      <div class="row-fluid">
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="CallerID">
                                    Caller ID</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCallerId" runat="server" CssClass="m-wrap span12" placeholder="CallerID"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtCallerId"
                                          ErrorMessage="Caller Id Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="CallerID">
                                    IP Switch</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtIPSwitch" runat="server" CssClass="m-wrap span12" placeholder="IP Switch"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtIPSwitch"
                                          ErrorMessage="IP Switch Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>--%>
                    <!--/row-->
<%--                    <div class="row-fluid">
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="firstName">
                                    Active</label>
                                <div class="controls">
                                    <asp:CheckBox ID="chkIsActive" class="span6 m-wrap" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                       <div class="span6">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Super Admin</label>
                                <div class="controls">
                                    <asp:CheckBox ID="chkIsSuperAdmin" class="span6 m-wrap" runat="server" />
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>--%>
                    <div class="form-actions">
                        <asp:Button runat="server" ID="btnSave" CssClass="btn blue" OnClick="btnSave_Click"
                              CausesValidation="true" Text="Save" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn" OnClick="btnCancel_Click"
                            CausesValidation="false" Text="Cancel" />
                    </div>
                    </form>
                    <!-- END FORM-->
                </div>
            </div>
        </div>
    </div>
    <!-- END PAGE CONTENT-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>
