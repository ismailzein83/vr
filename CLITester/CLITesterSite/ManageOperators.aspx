<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageOperators.aspx.cs" Inherits="ManageOperators" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Manage Operators <small>You can manage services for each operator</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Operators</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <div class="row-fluid">
        <div class="span12">
            <div id="divError" runat="server" visible="false" class="alert alert-error">
                <button class="close" data-dismiss="alert"></button>
                <strong>Error!</strong>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
            <!-- BEGIN EXAMPLE TABLE PORTLET-->
            <div class="portlet box blue">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-reorder"></i>Operators List</div>
                </div>
                <div class="portlet-body">
                    <div class="table-toolbar">
                        <div class="btn-group span2">
                            <asp:Button ID="btnSave" class="btn green cancel" runat="server" Text="Save" OnClick="btnSave_Click" />
                        </div>
                        <div class="control-group span10 form-horizontal">
                        </div>
                    </div>
                    <table width="100%" class="table-bordered table-striped table-condensed cf" >
                        <thead class="cf">
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Name
                                </th>

                                <th>MCC
                                </th>
                                <th>MNC
                                </th>
                                <th>Android
                                </th>
                                <th>Monty
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            
                            <asp:Repeater ID="rptCarriers"   runat="server"  >
                                <ItemTemplate>
                                    <tr>
                                        <td  data-title="Id"  visible="false" runat="server" id="th1">
                                            <asp:Label ID="Label0"  style="display:none;" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td data-title="Name" >
                                            <asp:Image  class="flag" ID="Image1" ImageUrl='<%# "~/assets/img/flags/"+ Eval("CountryPicture") + ".png" %>' runat="server" />

                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("FullName") %>'></asp:Label>
                                        </td>
                                        <td  data-title="MCC" style="text-align:center;">
                                            <asp:Label  ID="Label1" runat="server" Text='<%# Eval("mcc") %>'></asp:Label>
                                        </td>
                                        <td  data-title="MNC" style="text-align:center;">
                                            <asp:Label  ID="Label2" runat="server" Text='<%# Eval("mnc") %>'></asp:Label>
                                        </td>
                                        <td  data-title="Android" style="text-align:center;">
                                            <asp:CheckBox runat="server" ID="chkAndroid" Checked='<%# Eval("ServiceAndroid") %>' />
                                        </td>
                                        <td  data-title="Monty" style="text-align:center;">
                                            <asp:CheckBox  runat="server" ID="chkMonty" Checked='<%# Eval("ServiceMonty") %>' />
                                            <%--<input type="checkbox" runat="server" checked='<%# Eval("ServiceMonty") %>' value='<%#Eval("ServiceMonty")%>' id="chkMonty"/>--%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <!-- END EXAMPLE TABLE PORTLET-->


        </div>
    </div>
    <!-- END PAGE CONTENT -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- BEGIN PAGE LEVEL PLUGINS -->

    <script src="assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js" type="text/javascript"></script>
	<script src="assets/plugins/jquery.blockui.min.js" type="text/javascript"></script>  
	<script src="assets/plugins/jquery.cookie.min.js" type="text/javascript"></script>
	<script src="assets/plugins/uniform/jquery.uniform.min.js" type="text/javascript" ></script>

    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="assets/scripts/app.js"></script>


    <script>
        jQuery(document).ready(function () {
            App.init();

        });
    </script>
</asp:Content>

