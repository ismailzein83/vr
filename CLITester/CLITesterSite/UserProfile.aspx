<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="UserProfile.aspx.cs" Inherits="UserProfile" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="assets/plugins/chosen-bootstrap/chosen/chosen.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages/profile.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugins/bootstrap-switch/static/stylesheets/bootstrap-switch-metro.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-tags-input/jquery.tagsinput.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">User Profile <small>change user profile</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">User Profile</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->

    <div class="row-fluid">
        <div class="span12">
            <div id="divError" runat="server" visible="false" class="alert alert-error">
                <button class="close" data-dismiss="alert"></button>
                <strong>Error!</strong>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
            <div id="divSuccess" runat="server" visible="false" class="alert alert-success">
                <button class="close" data-dismiss="alert"></button>
                <strong>Success!</strong><asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </div>
        </div>
        <div class="span12">
            <div class="span3">
                <ul class="ver-inline-menu tabbable margin-bottom-10">
                    <li class="active">
                        <a data-toggle="tab" href="#tab_1-1">
                            <i class="icon-cog"></i>
                            Personal info
                        </a>
                        <span class="after"></span>
                    </li>
                    <li><a data-toggle="tab" href="#tab_3-3"><i class="icon-lock"></i>Change Password</a></li>
                    <li runat="server" id="liSettings" ><a data-toggle="tab" href="#tab_4-4"><i class="icon-eye-open"></i>Call Settings</a></li>
                    <li><a data-toggle="tab" href="#tab_5-5"><i class="icon-info-sign"></i>Profile History</a></li>
                </ul>
            </div>
            <div class="span9">
                <div class="tab-content">
                    <div id="tab_1-1" class="tab-pane active">
                        <div style="height: auto;" id="accordion1-1" class="accordion collapse">
                           
                                <label class="control-label">First Name</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="m-wrap span4" placeholder="First Name"></asp:TextBox>
                                <label class="control-label">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="m-wrap span4" placeholder="Last Name"></asp:TextBox>
                                <label class="control-label">Mobile Number</label>
                                <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="m-wrap span4" placeholder="Mobile Number"></asp:TextBox>
                                <label class="control-label">Email</label>

                                <div class="input-icon left">
                                    <i class="icon-envelope"></i>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="m-wrap span4" placeholder="Email Address"></asp:TextBox>
                                </div>
                                <label class="control-label">Website Url</label>
                                <asp:TextBox ID="txtWebsiteUrl" runat="server" CssClass="m-wrap span4" placeholder="http://www.mywebsite.com"></asp:TextBox>
                           
                        </div>
                    </div>
                    <div id="tab_3-3" class="tab-pane">
                        <div style="height: auto;" id="accordion3-3" class="accordion collapse">
                            <label class="control-label">Current Password</label>
                            <asp:TextBox ID="txtPassword" type="password" class="m-wrap span4" runat="server" CssClass="m-wrap span4"></asp:TextBox>
                            <label class="control-label">New Password</label>
                            <asp:TextBox ID="txtNewPassword" type="password" class="m-wrap span4" runat="server" CssClass="m-wrap span4"></asp:TextBox>
                            <label class="control-label">Re-type New Password</label>
                            <asp:TextBox ID="txtRetypePassword" type="password" class="m-wrap span4" runat="server" CssClass="m-wrap span4"></asp:TextBox>
                        </div>
                    </div>
                    <div id="tab_4-4" class="tab-pane">
                        <div style="height: auto;" id="accordion4-4" class="accordion collapse">
                            
                                <label class="control-label" style="visibility:hidden" >Switch IP</label>
                                <%--<asp:HiddenField ID="hdnIP" runat="server" />--%>
                                <%--<input class="span4 m-wrap" runat="server" id="input_ipv4" type="text" />--%>
                                <asp:TextBox ID="txtSwitchIp" Visible="false" runat="server" CssClass="m-wrap span4" placeholder="Switch IP"></asp:TextBox>
                                <label class="control-label">Caller ID</label>
                                <asp:TextBox ID="txtCallerId" runat="server" CssClass="m-wrap span4" placeholder="Caller ID"></asp:TextBox>
                           
                        </div>
                    </div>
                    <div id="tab_5-5" class="tab-pane">
                        <div style="height: auto;" id="accordion5-5" class="accordion collapse">
                            <div class="tab-pane profile-classic row-fluid">
                                <ul class="unstyled span4">
                                    <li><span>Creation Date:</span>
                                        <asp:Label ID="lblDate" runat="server" CssClass=" m-wrap span6 pull-right"></asp:Label></li>
                                    <li><span>Last Login Date:</span>
                                        <asp:Label ID="lblLoginDate" runat="server" CssClass="m-wrap span6 pull-right"></asp:Label></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="space5"></div>
                <div class="submit-btn">
                    <asp:Button ID="btnSave" class="btn green" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" class="btn" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                </div>
            </div>
            <!--end span9-->
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script type="text/javascript" src="assets/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js"></script>
    <script type="text/javascript" src="assets/plugins/jquery.input-ip-address-control-1.0.min.js"></script>
    <script src="assets/scripts/app.js"></script>

    <script>
        var handleIPAddressInput = function () {
            $('#input_ipv4').ipAddress();

            var s = document.getElementById("hdnIP");
            $('#input_ipv4').val(s.value);
            //$('#input_ipv6').ipAddress({ v: 6 });
        }

        jQuery(document).ready(function () {
            App.init();
            handleIPAddressInput();
        });
    </script>
</asp:Content>
