<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageTestNumberGroups.aspx.cs" Inherits="ManageTestNumberGroups" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-toggle-buttons/static/stylesheets/bootstrap-toggle-buttons.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-timepicker/compiled/timepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-daterangepicker/daterangepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/chosen-bootstrap/chosen/chosen.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-multi-select/css/multi-select-metro.css" />
    <script type="text/javascript">
        function disableAddBtn2() {

            $('#addBtn').attr('disabled', 'disabled');

            $('#<%=HdnId.ClientID %>').val('');
            $('#<%=txtName.ClientID %>').val('');

            clearValidationMsg();
            $("#addDiv").show();
        }
    </script>
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Manage Test Number Groups <small>Add a group</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Test Number Groups</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->

    <div class="row-fluid" id="addDiv" style="display: none">

        <div class="span12">
            <!-- BEGIN SAMPLE TABLE PORTLET-->
            <div class="portlet">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-cog"></i>Group</div>
                    <div class="tools">
                        <asp:Button ID="btnSave" class="btn blue btn-large" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return getTestNumberGroups();" />
                        <button class="btn " onclick="cancel();">Cancel</button>
                    </div>
                </div>
                <div class="portlet-body" style="display: block;">
                    <div class="span6">

                        <div class="form-horizontal span12">
                            <div class="alert alert-error hide">
                                <button class="close" data-dismiss="alert"></button>
                                You have some form errors. Please check below.
                            </div>
                            <asp:HiddenField ID="HdnId" runat="server" />

                            <div class="control-group">
                                <label class="control-label span3">Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtName" class="span6 m-wrap valStringR" data-required="1" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- END SAMPLE TABLE PORTLET-->
        </div>
    </div>

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
                    <div class="caption"><i class="icon-reorder"></i>Test Number Groups List</div>
                    <div class="actions">
                        <div class="btn-group span2">
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="table-toolbar">
                        <div class="btn-group span2">
                            <button class="btn green" id="addBtn" onclick="addTestNumberGroup();  return false;">Add New <i class="icon-plus"></i></button>
                        </div>
                        <div class="control-group span10 form-horizontal">
                        </div>
                    </div>
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_1">
                        <thead>
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Name
                                </th>
                                <th>Edit
                                </th>
                                <th>Delete
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptTestNumberGroups" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </td>
                                        <td class="center">
                                            <a href="#" onclick='editRow(<%# Eval("Id") %> ,<%# "\"" + Eval("Name") + "\"" %> )'>Edit</a>
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
    <script type="text/javascript" src="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.js"></script>

    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-timepicker/js/bootstrap-timepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/jquery-multi-select/js/jquery.multi-select.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-toggle-buttons/static/js/jquery.toggle.buttons.js"></script>
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

        function isDate(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 47 && charCode != 37 && charCode != 38 && charCode != 39 && charCode != 40) {
                return false;
            }
            return true;
        }


        var initTable1 = function () {

            /* Formatting function for row details */
            function fnFormatDetails(oTable, nTr) {
                var aData = oTable.fnGetData(nTr);
                var vData = aData[6];
                var rows = vData.split('!');
                var sOut = '<table class="table table-striped table-bordered table-advance table-hover">'
                               + ' <thead>'
                                  + ' <tr>'
                                       + '<th class="span2"><i class="icon-list"></i> Number</th>'
                                    + '</tr>'
                                + '</thead>'
                                + '<tbody>';
                for (var i = 0 ; i < rows.length; i++) {
                    sOut += '<tr><td>' + rows[i] + '</td></tr>';
                }

                sOut += '</tbody>';
                sOut += '</table>';

                return sOut;
            }

            /*
             * Insert a 'details' column to the table
             */
            //var nCloneTh = document.createElement('th');
            //var nCloneTd = document.createElement('td');
            //nCloneTd.innerHTML = '<span class="row-details row-details-close"></span>';

            //$('#sample_1 thead tr').each(function () {
            //    this.insertBefore(nCloneTh, this.childNodes[0]);
            //});

            //$('#sample_1 tbody tr').each(function () {
            //    this.insertBefore(nCloneTd.cloneNode(true), this.childNodes[0]);
            //});

            /*
             * Initialize DataTables, with no sorting on the 'details' column
             */
            var oTable = $('#sample_1').dataTable({
                "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [0] }
                ],
                "aaSorting": [[1, 'asc']],
                "aLengthMenu": [
                   [10, 15, 20, -1],
                   [10, 15, 20, "All"] // change per page values here
                ],
                // set the initial value
                "iDisplayLength": 10,
            });

            jQuery('#sample_1_wrapper .dataTables_filter input').addClass("m-wrap small"); // modify table search input
            jQuery('#sample_1_wrapper .dataTables_length select').addClass("m-wrap small"); // modify table per page dropdown
            jQuery('#sample_1_wrapper .dataTables_length select').select2(); // initialize select2 dropdown

            /* Add event listener for opening and closing details
             * Note that the indicator for showing which row is open is not controlled by DataTables,
             * rather it is done here
             */
            $('#sample_1').on('click', ' tbody td .row-details', function () {
                var nTr = $(this).parents('tr')[0];
                if (oTable.fnIsOpen(nTr)) {
                    /* This row is already open - close it */
                    $(this).addClass("row-details-close").removeClass("row-details-open");
                    oTable.fnClose(nTr);
                }
                else {
                    /* Open this row */
                    $(this).addClass("row-details-open").removeClass("row-details-close");
                    oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), 'details');
                }
            });
        }

    </script>
    <!-- END TABLE  SCRIPTS -->

    <script>

        var disableAddBtn = function () {
            $('#addBtn').attr('disabled', 'disabled');
        }

        var cancel = function () {
            emptyFields();
            $("#addDiv").hide();
            $('#addBtn').removeAttr('disabled');
            $('#myTable > tbody > tr').remove();
            arrayRow = [];
        }
        var emptyFields = function () {
            $('#<%=HdnId.ClientID %>').val('');
            $('#<%=txtName.ClientID %>').val('');
            clearValidationMsg();

        }

        var addTestNumberGroup = function () {
            disableAddBtn();
            emptyFields();
            $("#addDiv").show();

        }

        var editRow = function (id, name) {
                emptyFields();
                $('#<%=HdnId.ClientID %>').val(id);
            $('#<%=txtName.ClientID %>').val(name);

                $("#addDiv").show();
            }

            jQuery(document).ready(function () {
                App.init();
                handleValidation2();

                if (!jQuery().dataTable) {
                    return;
                }

                initTable1();

            });
    </script>
</asp:Content>


