<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageSettings.aspx.cs" Inherits="ManageSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">
                Manage Settings
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Settings</a></li>
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
                        <i class="icon-cogs"></i>Settings</div>
                    <div class="tools">
                    </div>
                </div>
                <div class="portlet-body form">
                    <!-- BEGIN FORM-->
                    <form action="#" class="horizontal-form">
                    <h3 class="form-section">
                        Sip Account</h3>

                    <div class="row-fluid">
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="firstName">
                                    User Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="m-wrap span12" placeholder="UserName"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtUserName"
                                        ValidationGroup="Usr" ErrorMessage="UserName Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Login<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLogin" runat="server" CssClass="m-wrap span12" placeholder="Login"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtLogin"
                                        ValidationGroup="Usr" ErrorMessage="Login Required" Display="Dynamic" Font-Bold="true"
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
                                <label class="control-label" for="firstName">
                                    Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="m-wrap span12"
                                        placeholder="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                                        ValidationGroup="Usr" ErrorMessage="Password Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span6 ">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Server<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtServer" runat="server" CssClass="m-wrap span12" placeholder="Server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtServer"
                                        ValidationGroup="Usr" ErrorMessage="Server Required" Display="Dynamic" Font-Bold="true"
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
                                <label class="control-label" for="CallerID">
                                    Caller ID</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCallerId" runat="server" CssClass="m-wrap span12" placeholder="CallerID"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtCallerId"
                                        ValidationGroup="Usr" ErrorMessage="Caller Id Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>
                    <div class="form-actions">
                        <asp:Button runat="server" ID="btnSave" CssClass="btn blue" OnClick="btnSave_Click"
                            ValidationGroup="Usr" CausesValidation="true" Text="Save" />
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
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" Runat="Server">
<script>
    jQuery(document).ready(function () {
        App.init();
    });
</script>
</asp:Content>

