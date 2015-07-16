<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageUsers.aspx.cs" Inherits="ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">
                Manage Users
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Users</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <div class="row-fluid">
        <div class="span12">
            
                <!-- BEGIN EXAMPLE TABLE PORTLET-->
						<div class="portlet box blue">
							<div class="portlet-title">
								<div class="caption"><i class="icon-reorder"></i>Users List</div>
								<div class="actions">
									<div class="btn-group">
										<a class="btn" href="#" data-toggle="dropdown">
										Columns
										<i class="icon-angle-down"></i>
										</a>
										<div id="sample_2_column_toggler" class="dropdown-menu hold-on-click dropdown-checkboxes pull-right">
											<label><input type="checkbox" checked data-column="0">Name</label>
											<label><input type="checkbox" checked data-column="1">Username</label>
											<label><input type="checkbox" checked data-column="2">Email</label>
											<label><input type="checkbox" checked data-column="3">Creation Date</label>
                                            <label><input type="checkbox" checked data-column="4">Balance</label>
                                            <label><input type="checkbox" checked data-column="5">Remaining</label>
                                            <label><input type="checkbox" checked data-column="6">Active</label>
                                            <%--<label><input type="checkbox" checked data-column="5">Roles</label>--%>
										</div>
									</div>
								</div>
							</div>
							<div class="portlet-body">
                                <div class="table-toolbar">
                        <div class="btn-group">

                        <asp:LinkButton ID="btnNew" runat="server"  ToolTip="Add New" CssClass="btn green"
                        OnClick="btnNew_Click" >Add New
                            <i class="icon-plus"></i>
                        </asp:LinkButton>
                        </div>
                    </div>
								<table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
									<thead>
										<tr>
											<th visible="false" runat="server" id="th1">
                                    Id
                                </th>
                                <th>
                                    Name
                                </th>
                                <th>
                                    Username
                                </th>
                                <th>
                                    Email
                                </th>
                                <th>
                                    Creation Date
                                </th>
                                <th>
                                    Active
                                </th>
                                <th>
                                    Edit
                                </th>
                                <th>
                                    Delete
                                </th>
										</tr>
									</thead>
									 <tbody>
                            <asp:Repeater ID="rptUsers" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("CreationDate") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("Balance") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("RemainingBalance") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("IsActive") %>'></asp:Label>
                                        </td>
                                        <td class="center">
                                            <a class="edit" href="ManageUser.aspx?<%#  QueryStringModule.Encrypt("mId="+ Eval("Id") )%>">
                                                Edit</a>
                                        </td>
                                        <td class="center">
                                            <asp:LinkButton class="delete" ID="lnkDelete" OnClientClick="return confirm('Are you sure you want to delete?');"
                                                OnClick="btnDelete_Click" CommandArgument='<%# Eval("Id") %>' runat="server">Delete</asp:LinkButton>
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
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="assets/scripts/app.js"></script>
    <script>
        var initTable2 = function () {
            var oTable = $('#sample_2').dataTable({
                "aoColumnDefs": [
                    { "aTargets": [0] }
                ],
                "aaSorting": [[1, 'asc']],
                "aLengthMenu": [
                   [10, 15, 20, -1],
                   [10, 15, 20, "All"] // change per page values here
                ],
                // set the initial value
                "iDisplayLength": 10,
            });

            jQuery('#sample_2_wrapper .dataTables_filter input').addClass("m-wrap small"); // modify table search input
            jQuery('#sample_2_wrapper .dataTables_length select').addClass("m-wrap small"); // modify table per page dropdown
            //jQuery('#sample_2_wrapper .dataTables_length select').select2(); // initialize select2 dropdown

            $('#sample_2_column_toggler input[type="checkbox"]').change(function () {
                /* Get the DataTables object again - this is not a recreation, just a get of the object */
                var iCol = parseInt($(this).attr("data-column"));
                var bVis = oTable.fnSettings().aoColumns[iCol].bVisible;
                oTable.fnSetColumnVis(iCol, (bVis ? false : true));
            });
        }
        jQuery(document).ready(function () {
            App.init();

            
            if (!jQuery().dataTable) {
                return;
            }

            initTable2();
        });
    </script>
</asp:Content>
