<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageTestCalls.aspx.cs" Inherits="ManageTestCalls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
            <h3 class="page-title">Data Calls
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Data Calls</a></li>
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
                    <div class="caption"><i class="icon-reorder"></i>Data Calls List</div>
                    <div class="actions">
                        <div class="btn-group">
                            <a class="btn" href="#" data-toggle="dropdown">Columns
										<i class="icon-angle-down"></i>
                            </a>
                            <div id="sample_2_column_toggler" class="dropdown-menu hold-on-click dropdown-checkboxes pull-right">
                                <label>
                                    <input type="checkbox" checked data-column="0">Id</label>
                                <label>
                                    <input type="checkbox" checked data-column="1">Name</label>
                                 <label>
                                    <input type="checkbox" checked data-column="2">Creation Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="3">End Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="4">Total Seconds</label>
                                <label>
                                    <input type="checkbox" checked data-column="5">MSISDN</label>
                                <label>
                                    <input type="checkbox" checked data-column="6">Received Cli</label>
                                <label>
                                    <input type="checkbox" checked data-column="7">Request Id</label>
                                <label>
                                    <input type="checkbox" checked data-column="8">Start Call</label>
                                <label>
                                    <input type="checkbox" checked data-column="9">End Call</label>
                                <label>
                                    <input type="checkbox" checked data-column="10">Response Code</label>
                                <label>
                                    <input type="checkbox" checked data-column="11">Error Message</label>
                            </div>
                        </div>
                    </div>
                    <div class="tools">
                        <a class="btn green" id="LnkRefresh">Refresh <i class=" icon-refresh "></i></a>
                    </div>
                </div>
                <div class="portlet-body">
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th>Id
                                </th>
                                <th>Name
                                </th>
                                <th>Create
                                </th>
                                <th>End
                                </th>
                                 <th>-
                                </th>                             
                                 <th>MSISDN
                                </th>
                                 <th>Received Cli
                                </th>
                                 <th>ReqID
                                </th>
                                 <th>StartC
                                </th>
                                 <th>EndC
                                </th>
                                 <th>
                                </th>
                                   <th>Error Message
                                </th>
                            </tr>
                        </thead>
                        <tbody class="table table-striped table-bordered table-advance table-hover">
                            <asp:Repeater ID="rptCarriers" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td>
                                            <div class='<%# Eval("Status") == null ? "none1" : (int)Eval("Status") == 1 ? "success" : "danger" %>'></div>&nbsp;
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id").ToString().PadLeft(4) %>'></asp:Label>
                                        </td>
                                        <td class="highlight">
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label  ID="Label1" runat="server" Text='<%#  Eval("CreationDate") == null ? "": ((DateTime)Eval("CreationDate")).ToString("dd-MM-yyyy HH:mm:ss") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("EndDate") == null ? "": ((DateTime)Eval("EndDate")).ToString("HH:mm:ss") %>'></asp:Label>
                                        </td>
                                         <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# (Eval("CreationDate") != null && Eval("EndDate") != null) ? (((DateTime)Eval("EndDate") - (DateTime)Eval("CreationDate")).TotalSeconds).ToString("0") : "" %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("MSISDN")  == null ? "" : Eval("MSISDN") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("ReceivedCli") == null ? "" : Eval("ReceivedCli")  %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Eval("RequestId") == null ? "" : Eval("RequestId") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Eval("StartDate") == null ? "": ((DateTime)Eval("StartDate")).ToString("HH:mm:ss") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Eval("EndCall") == null ? "": ((DateTime)Eval("EndCall")).ToString("HH:mm:ss") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("ResponseCode") == null ? "" : Eval("ResponseCode")  %>'></asp:Label>
                                        </td>
                                              <td>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("ErrorMessage") %>'></asp:Label>
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
                "aaSorting": [[0, 'desc']],
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

            $('#LnkRefresh').bind('click', function (e) {
                location.reload();
            });

            if (!jQuery().dataTable) {
                return;
            }

            initTable2();



        });
    </script>
</asp:Content>

