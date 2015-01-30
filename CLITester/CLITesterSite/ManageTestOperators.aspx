<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageTestOperators.aspx.cs" Inherits="ManageTestOperators" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.css" />
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">History Test Calls
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">History Test Calls</a></li>
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
            <div id="divSuccess" runat="server" visible="false" class="alert alert-success">
                <button class="close" data-dismiss="alert"></button>
                <strong>Success!</strong><asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </div>
            <!-- BEGIN EXAMPLE TABLE PORTLET-->
            <div class="portlet box blue">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-reorder"></i>History Test Calls List</div>
                    <div class="actions">
                        <div class="btn-group">
                            <a class="btn" href="#" data-toggle="dropdown">Columns
										<i class="icon-angle-down"></i>
                            </a>
                            <div id="sample_2_column_toggler" class="dropdown-menu hold-on-click dropdown-checkboxes pull-right">
                                <label>
                                    <input type="checkbox" checked data-column="0">Name</label>
                                <label>
                                    <input type="checkbox" checked data-column="1">Schedule</label>
                                 <label>
                                    <input type="checkbox" checked data-column="2">Creation Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="3">End Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="4">Test Cli</label>
                                <label>
                                    <input type="checkbox" checked data-column="5">Received Cli</label>
                                <label>
                                    <input type="checkbox" checked data-column="6">Status</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Name
                                </th>
                                <th>Schedule
                                </th>
                                <th>Creation Date
                                </th>
                                <th>End Date
                                </th>
                                 <th>Test Cli</th>
                                <th>Received Cli
                                </th>
                                 <th>Status
                                </th>
                            </tr>
                        </thead>
                        <tbody class="table table-striped table-bordered table-advance table-hover">
                            <asp:Repeater ID="rptCarriers" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td class="highlight">
                                            <div class='<%# Eval("Status") == null ? "none1" : (int)Eval("Status") == 1 ? "success" : "danger" %>'></div>&nbsp;
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Operator.FullName") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text='<%#  Eval("Schedule") == null ? " " : Eval("Schedule.DisplayName") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label  ID="Label1" runat="server" Text='<%#  Eval("CreationDate") == null ? " " :  ((DateTime)Eval("CreationDate")).ToString("yyyy-MM-dd HH:mm:ss") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%#  Eval("EndDate") == null ? " " : ((DateTime)Eval("EndDate")).ToString("yyyy-MM-dd HH:mm:ss") %>'></asp:Label>
                                        </td>
                                        
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("TestCli") %>'></asp:Label>
                                        </td>
                                        
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("ReceivedCli") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label5" class='<%# Eval("Status") == null ? "label label-danger" : (int)Eval("Status") == 1 ? "label label-success" : "label label-important" %>' runat="server" Text='<%# Eval("Status") == null ? "No status" : (int)Eval("Status") == 1 ? "CLI Delivered" : "CLI not delivered" %>'></asp:Label>
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
    <script type="text/javascript" src="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.js"></script>
    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="assets/scripts/app.js"></script>

    <!-- BEGIN TABLE  SCRIPTS -->
    <script>
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }

        var initTable2 = function () {
            var oTable = $('#sample_2').dataTable({
                "aoColumnDefs": [
                    { "aTargets": [0] }
                ],
                "aaSorting": [[2, 'desc']],
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
    </script>
    <!-- END TABLE  SCRIPTS -->

    <script>
        jQuery(document).ready(function () {
            App.init();
            handleValidation1();


            if (!jQuery().dataTable) {
                return;
            }

            initTable2();

        });
    </script>
</asp:Content>

