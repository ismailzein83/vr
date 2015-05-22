<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageSchedules.aspx.cs" Inherits="ManageSchedules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-timepicker/compiled/timepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-daterangepicker/daterangepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/chosen-bootstrap/chosen/chosen.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-multi-select/css/multi-select-metro.css" />
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Manage Schedules <small>Add a schedule to perform your test</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Schedules</a></li>
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
                    <div class="caption"><i class="icon-bell"></i>Schedule</div>
                    <div class="tools">
                        <asp:Button ID="btnSave" class="btn blue btn-large" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return getSchedules();" />
                        <button class="btn " onclick="cancel();">Cancel</button>
                        <%-- <a href="javascript:;" class="collapse"></a>
                        <a href="#portlet-config" data-toggle="modal" class="config"></a>
                        <a href="javascript:;" class="reload"></a>
                        <a href="javascript:;" class="remove"></a>--%>
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
                                <label class="control-label span2">Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtName" class="span6 m-wrap valStringR" data-required="1" runat="server"></asp:TextBox>
                                </div>
                            </div>

<%--                            <div class="control-group">
                                <label class="control-label span2">Ratio<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtOccurs" onkeypress='return isNumber(event)' class="span6 m-wrap valDigits2" data-required="1" runat="server"></asp:TextBox>
                                </div>
                            </div>--%>

                            <div class="control-group">
                                <label class="control-label span2">Date<span class="required">*</span></label>
                                <div class="controls">
                                    <div class="input-prepend">
                                        <span class="add-on"><i class="icon-calendar"></i></span>
                                        <input id="txtDate" runat="server" type="text" class="m-wrap date-range span12 valStringR" data-required="1" readonly />
                                    </div>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label span2">Time 1<span class="required">*</span></label>
                                <div class="controls">
                                    <div class="input-prepend bootstrap-timepicker-component">
                                        <span class="add-on"><i class="icon-time"></i></span>
                                        <input class="m-wrap m-ctrl-small timepicker-24 span12 valTimeR" type="text" data-required="1" id="txtTime" runat="server" />
                                    </div>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label span2">Time 2<span class="required"></span></label>
                                <div class="controls">
                                    <div class="input-prepend bootstrap-timepicker-component">
                                        <span class="add-on"><i class="icon-time"></i></span>
                                        <input class="m-wrap m-ctrl-small timepicker-24 span12 valTime" type="text" data-required="1" id="txtTime1" runat="server" />
                                        
                                    </div>
                                    <span class="help-block">Difference between times should be at least 2 hours</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="span6">
                        <asp:HiddenField ID="HdTable" runat="server" />
                        <div class="row-fluid">

                            <div class="control-group">
                                <div class="controls controls-row">
                                    <select name="" id="selectOperator" class="span5 select2">
                                        <asp:Repeater ID="rptOperators" runat="server">
                                            <ItemTemplate>
                                                <option value='<%# Eval("CountryPicture") %>~<%# Eval("Id") %>'><%# Eval("Country") %> - <%# Eval("Name") %></option>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </select>

                                    <select id="selectPrefix" class="span3 select2 m-wrap" data-placeholder="Choose a route" tabindex="1">
                                        <asp:Repeater ID="rptCarriers" runat="server">
                                            <ItemTemplate>
                                                <option value='<%# Eval("Id") %>'><%# Eval("ShortName") %> - <%# Eval("Prefix") %></option>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </select>                                    
                                    
                                    <input id="txtCount" onkeypress='return isNumber(event)'  class="span2 m-wrap" placeholder="Count" data-placeholder="Count" data-required="1" ></input>
                                   
                                    <a class="btn green span1" onclick="addToTable()"><i class="icon-plus"></i></a>
                                </div>
                            </div>


                        </div>

                        <div class="row-fluid">

                            <table class="table table-striped table-bordered table-advance table-hover" id="myTable">
                                <thead>
                                    <tr>
                                        <th style="display: none;"></th>
                                        <th><i class="icon-briefcase"></i> Operator</th>
                                        <th class="hidden-phone"><i class="icon-user"></i> Route</th>
                                        <th class="hidden-phone"><i class="icon-refresh"></i> Count</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>


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
                    <div class="caption"><i class="icon-reorder"></i>Schedules List</div>
                    <div class="actions">
                        <div class="btn-group span2">

                            <%--<a class="btn green"  onclick="emptyFields()">Add New <i class="icon-plus"></i></a>--%>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="table-toolbar">
                        <div class="btn-group span2">
                            <button class="btn green" id="addBtn" onclick="disableAddBtn(); emptyFields(); return false;">Add New <i class="icon-plus"></i></button>
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
                                <%--<th>Ratio
                                </th>--%>
                                <th>Start Date
                                </th>
                                <th>End Date
                                </th>
                                <th>Time 1
                                </th>
                                <th>Time 2
                                </th>
                                <th style="display: none;">OperatorPrefix
                                </th>
                                <th>View
                                </th>
                                <th>Edit
                                </th>
                                <th>Delete
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptSchedules" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Label>
                                        </td>
                                      <%--  <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("OccursEvery") %>'></asp:Label>
                                        </td>--%>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("StartDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("EndDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("SpecificTime", "{0:HH:mm}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("SpecificTime1", "{0:HH:mm}") %>'></asp:Label>
                                        </td>
                                        <td style="display: none;">
                                            <%# Eval("OperatorPrefix") %>
                                        </td>
                                        <td class="center">
                                            <asp:LinkButton class="delete" ID="lnkView"
                                                OnClick="btnView_Click" CommandArgument='<%# Eval("Id") %>' runat="server">View</asp:LinkButton>
                                        </td>
                                        <td class="center">
                                            <a href="#" onclick='editRow(<%# Eval("Id") %> ,<%# "\"" + Eval("DisplayName") + "\"" %> , <%# "\"" + Eval("OccursEvery") + "\""  %> , <%# "\"" + (((DateTime)Eval("SpecificTime")).ToString("HH:mm")) + "\""  %> , <%# "\"" + Eval("SpecificTime1") + "\""  %>  , <%#  "\"" + ((DateTime)Eval("StartDate")).ToString("dd-MM-yyyy") +"\""%> ,<%#  "\"" + ((DateTime)Eval("EndDate")).ToString("dd-MM-yyyy") + "\"" %> , <%# "\"" + Eval("OperatorPrefixId")  + "\""  %>  )'>Edit</a>
                                            <%--                                            <a href="#" runat="server" onserverclick="btnEdit_Click" onclick="disableAddBtn(); emptyFields();">Edit33</a>

                                            <asp:LinkButton ID="lnkEdit"
                                                OnClick="btnEdit_Click" CommandArgument='<%# Eval("Id") %>' OnClientClick="disableAddBtn(); emptyFields();" runat="server">Edit</asp:LinkButton>--%>
                                            
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

    <div class="row-fluid" id="viewDiv" runat="server">

        <div class="portlet box purple">
            <div class="portlet-title">
                <div class="caption"><i class="icon-reorder"></i>Schedule History </div>
                <div class="actions">
                    <small class="pull-left"> <i class="icon-warning-sign"></i> Last 100 records </small>&nbsp;
                    <button class="btn black" id="btnClose" onclick="CloseLst(); return false;"><i class="icon-remove"></i>Close</button>
                </div>
            </div>
            <div class="portlet-body">
                <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                    <thead>
                        <tr>
                            <th visible="false" runat="server" id="th2">Id
                            </th>
                            <th class="span3">Name
                            </th>
                            <th class="span2">Schedule
                            </th>
                            <th class="span3">Creation Date
                            </th>
                            <th class="span3">End Date
                            </th>
                            <th class="span2">Test Cli
                            </th>
                            <th class="span2">Received Cli
                            </th>
                            <th class="span2">Status
                            </th>
                        </tr>
                    </thead>
                    <tbody class="table table-striped table-bordered table-advance table-hover">
                        <asp:Repeater ID="rptHistory" runat="server">
                            <ItemTemplate>
                                <tr class="odd gradeX">
                                    <td runat="server" id="tdId" visible="false">
                                        <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                    </td>
                                    <td class="highlight">
                                        <div class='<%# Eval("Status") == null ? "none1" : (int)Eval("Status") == 1 ? "success" : "danger" %>'></div>
                                        &nbsp;
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Operator.FullName") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text='<%#  Eval("Schedule") == null ? " " : Eval("Schedule.DisplayName") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text='<%#  Eval("CreationDate") == null ? " " :  ((DateTime)Eval("CreationDate")).ToString("yyyy-MM-dd HH:mm:ss") %>'></asp:Label>
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
    <script type="text/javascript" src="assets/plugins/jquery-multi-select/js/jquery.multi-select.js"></script>
    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="assets/scripts/app.js"></script>

    <!-- BEGIN TABLE  SCRIPTS -->
    <script>
       

  

        function isNumber(evt) {

            var tval = $('#txtCount').val(),
                tlength = tval.length,
                set = 2,
                remain = parseInt(set - tlength);

            $('#txtCount').text(remain);
            if (remain <= 0 && evt.which !== 0 && evt.charCode !== 0) {
                $('#txtCount').val((tval).substring(0, tlength - 1))
            }

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
                                       + '<th class="span3"><i class="icon-briefcase"></i> Operator</th>'
                                       + '<th class="span3"><i class="icon-user"></i> Route</th>'
                                       + '<th class="hidden-phone"><i class="icon-refresh"></i> Count</th>'
                                    + '</tr>'
                                + '</thead>'
                                + '<tbody>';
                for (var i = 0 ; i < rows.length; i++) {
                    if (rows[i] != '') {
                        var row = rows[i].split('$');

                        if (row[0] != '' || row[1] != '' || row[2] != '' || row[1] != null || row[2] != null || row[0] != null) {
                            sOut += '<tr><td>' + row[0] + '</td><td>' + row[1] + '</td><td>' + row[2] + '</td></tr>';
                        }
                    }
                }

                sOut += '</tbody>';
                sOut += '</table>';

                return sOut;
            }

            /*
             * Insert a 'details' column to the table
             */
            var nCloneTh = document.createElement('th');
            var nCloneTd = document.createElement('td');
            nCloneTd.innerHTML = '<span class="row-details row-details-close"></span>';

            $('#sample_1 thead tr').each(function () {
                this.insertBefore(nCloneTh, this.childNodes[0]);
            });

            $('#sample_1 tbody tr').each(function () {
                this.insertBefore(nCloneTd.cloneNode(true), this.childNodes[0]);
            });

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

            var oTable2 = $('#sample_2').dataTable({
                "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [0] }
                ],
                "aaSorting": [[2, 'asc']],
                "aLengthMenu": [
                   [10, 15, 20, -1],
                   [10, 15, 20, "All"] // change per page values here
                ],
                // set the initial value
                "iDisplayLength": 10,
            });

            jQuery('#sample_1_wrapper .dataTables_filter input').addClass("m-wrap small"); // modify table search input
            jQuery('#sample_1_wrapper .dataTables_length select').addClass("m-wrap small"); // modify table per page dropdown
            //jQuery('#sample_1_wrapper .dataTables_length select').select2(); // initialize select2 dropdown

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


        var handleDateRangePickers = function () {
            if (!jQuery().daterangepicker) {
                return;
            }

            $('.date-range').daterangepicker(
                {
                    opens: (App.isRTL() ? 'left' : 'right'),
                    format: 'dd-MM-yyyy',
                    separator: ' to ',
                    startDate: Date.today().add({
                        days: -29
                    }),
                    endDate: Date.today(),
                    minDate: '01/01/2014',
                    maxDate: '12/31/2016',
                }
            );
        }

        var handleTimePickers = function () {

            if (jQuery().timepicker) {
                // $('.timepicker-default').timepicker();
                $('.timepicker-24').timepicker({
                    minuteStep: 1,
                    showSeconds: false,
                    showMeridian: false
                });
            }
        }

        var handleSelect2Modal = function () {

            function format(state) {
                if (!state.id) return state.text; // optgroup
                return "<img class='flag' src='assets/img/flags/" + state.id.toLowerCase().split('~')[0] + ".png'/>&nbsp;&nbsp;" + state.text;
            }

            function format2(state) {
                if (!state.id) return state.text; // optgroup
                return state.text;
            }

            
            $("#selectOperator").select2({
                allowClear: true,
                formatResult: format,
                formatSelection: format,
                escapeMarkup: function (m) {
                    return m;
                }
            });

            $("#selectPrefix").select2({
                allowClear: true,
                formatResult: format2,
                formatSelection: format2,
                escapeMarkup: function (m) {
                    return m;
                }
            });
        }

    </script>
    <!-- END TABLE  SCRIPTS -->

    <script>

        var arrayRow = [];
        var arrayRow2 = [];

        function removeA(arr) {
            var what, a = arguments, L = a.length, ax;
            while (L > 1 && arr.length) {
                what = a[--L];
                while ((ax = arr.indexOf(what)) != -1) {
                    arr.splice(ax, 1);
                }
            }
            return arr;
        }

        var addToTable = function () {
            var operator = $('#selectOperator').find(":selected").text();
            var prefix = $('#selectPrefix').find(":selected").text();
            var count = $('#txtCount').val();

            if (count == '')
                count = 1;

            var operatorId = $('#selectOperator').find(":selected").val().split('~')[1];
            var prefixId = $('#selectPrefix').find(":selected").val();
            var row = operatorId + '~' + prefixId + '~' + count;
            var row2 = operatorId + '~' + prefixId;

            if ($.inArray(row2, arrayRow2) !== -1) {
                return false;
            }

            $('#myTable > tbody').append('<tr>' +
                                '<td style="display:none;">' + row + '</td>' +
                                '<td style="display:none;">' + row2 + '</td>' +
                                '<td>' + operator + '</td>' +
                                '<td>' + prefix + '</td>' +
                                '<td>' + count + '</td>' +
                                '<td><a href="#" class="btn mini black  pull-right deleteRow" ><i class="icon-trash"></i> Remove</a></td></tr>');
            arrayRow.push(row);
            arrayRow2.push(row2);
            $('#txtCount').val('');
        }

        var addToTable2 = function (operatorId, operator, prefixId, prefix, count) {
            var row = operatorId + '~' + prefixId + '~' + count;
            var row2 = operatorId + '~' + prefixId;

            if ($.inArray(row2, arrayRow2) !== -1) {
                return false;
            }

            if (count == '')
                count = 1;

            $('#myTable > tbody').append('<tr>' +
                                '<td style="display:none;">' + row + '</td>' +
                                '<td style="display:none;">' + row2 + '</td>' +
                                '<td>' + operator + '</td>' +
                                '<td>' + prefix + '</td>' +
                                '<td>' + count + '</td>' +
                                '<td><a href="#" class="btn mini black  pull-right deleteRow" ><i class="icon-trash"></i> Remove</a></td></tr>');
            arrayRow.push(row);
            arrayRow2.push(row2);
        }

        $(".deleteRow").live('click', function (event) {
            var firstTd = $(this).parent().parent().children("td:first").html();
            
            console.log('first:: ' + firstTd);
            var secondTd = $(this).parent().parent().children().eq(1).html();
            console.log('sec:: ' + secondTd);
            removeA(arrayRow, firstTd);
            removeA(arrayRow2, secondTd);

            $(this).parent().parent().remove();


        });

        var disableAddBtn = function () {
            $('#addBtn').attr('disabled', 'disabled');
        }

        var cancel = function () {
            emptyFields();
            $("#addDiv").hide();
            $('#addBtn').removeAttr('disabled');
            $('#myTable > tbody > tr').remove();
            arrayRow = [];
            arrayRow2 = [];
        }

        var emptyFields = function () {
            $('#<%=HdnId.ClientID %>').val('');
            //$('#<=txtOccurs.ClientID %>').val('');
            $('#txtCount').val('');
            
            $('#<%=txtName.ClientID %>').val('');
            $('#txtDate').val('');
            $('#txtTime').val('');
            $('#txtTime1').val('');

            $('#<%=HdTable.ClientID %>').val('');

            clearValidationMsg();
            $("#addDiv").show();
        }

        var CloseLst = function () {
            $("#viewDiv").hide();
        }

        var editRow = function (id, name, occurs, SpecificTime, SpecificTime1, startDate, endDate, OperatorPrefix) {
            
            emptyFields();
            $('#<%=HdnId.ClientID %>').val(id);
            $('#<%=txtName.ClientID %>').val(name);
            //$('#<=txtOccurs.ClientID %>').val(occurs);
            var st = SpecificTime;
            var st1 = SpecificTime1;

            var hour = SpecificTime.substring(SpecificTime.indexOf(':') -2, SpecificTime.indexOf(':'));
            var min = st.substring(st.indexOf(':') + 1, st.indexOf(':') + 3);
            
            if (SpecificTime1 != "") {

                var pm = SpecificTime1.substring(SpecificTime1.length - 2, SpecificTime1.length);
                var hour1 = 0;
                if (pm == "PM") {
                    hour1 = SpecificTime1.substr(SpecificTime1.indexOf(':') - 2, 2);
                    
                    if( hour1 != "12")
                        hour1 = Number(Number(hour1) + 12);
                }
                else {
                    hour1 = SpecificTime1.substr(SpecificTime1.indexOf(':') - 2, 2);
                }

                var min1 = st1.substring(st1.indexOf(':') + 1, st1.indexOf(':') + 3);
                console.log(min1);
                $('#<%=txtTime1.ClientID %>').val(hour1 + ":" + min1);
            }
            else {
                $('#<%=txtTime1.ClientID %>').val("");
            }

            //var pp = '';
            //if (hour >= 12) {
            //    pp = "PM";
            //    hour = hour - 12;
            //}

            //else
            //    pp = "AM";

            //var pp = st.substring(st.indexOf(' ') + 1, st.indexOf(' ') + 3);
            //$('#<txtTime.ClientID %>').val(hour + ":" + min + " " + pp);
            $('#<%=txtTime.ClientID %>').val(hour + ":" + min);
            

            $('#<%=txtDate.ClientID %>').val(startDate + " to " + endDate);
            $('.date-range').setDate();

            var mySplitResult = OperatorPrefix.split("$");
            var j = 0; 
            for (i = 0; i < (mySplitResult.length - 1) / 5; i++) {
                addToTable2(mySplitResult[j], mySplitResult[j + 1], mySplitResult[j + 2], mySplitResult[j + 3], mySplitResult[j + 4]);
                j = j + 5;
            }
        }

        var getSchedules = function () {
            var value = '';
           
            $("#myTable > tbody > tr").each(function () {
                value = value + ';' + $(this).find('td:eq(0)').text();
            });
            console.log('getSchedules: ' + value);

            $('#<%=HdTable.ClientID %>').val(value);

            //if (value == '') {
            //    $("#form_sample_1").valid();
            //    return false;
            //}
            return true;
        }





        jQuery(document).ready(function () {
            App.init();
            handleValidation2();

            handleDateRangePickers();

            handleTimePickers();

            handleSelect2Modal();

            if (!jQuery().dataTable) {
                return;
            }

            initTable1();

        });
    </script>
</asp:Content>
