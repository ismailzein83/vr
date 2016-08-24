<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageSchedules.aspx.cs" Inherits="ManageSchedules" %>

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
            $('#txtDate').val('');
            $('#txtStartDate').val('');
            $('#txtEndDate').val('');
            $('#txtTime').val('');
            $('#<%=HdTable.ClientID %>').val('');
            $('#chkEnabled').prop('checked', false).change()
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
                        <asp:Button ID="btnSave" class="btn blue btn-large" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return saveSchedule();" />
                        <button class="btn " onclick="cancel();">Cancel</button>
                        <%-- <a href="javascript:;" class="collapse"></a>
                        <a href="#portlet-config" data-toggle="modal" class="config"></a>
                        <a href="javascript:;" class="reload"></a>
                        <a href="javascript:;" class="remove"></a>--%>
                    </div>
                </div>
                <div class="portlet-body" style="display: block;">
                    <div class="span6">
                        
                       <%-- <div class="form-horizontal span6">--%>
                            <div class="alert alert-error hide">
                                <button class="close" data-dismiss="alert"></button>
                                You have some form errors. Please check below.
                            </div>
                            <asp:HiddenField ID="HdnId" runat="server" />

                            <div class="">
                                <label class="span6">Name<span class="required">*</span></label>
                                <div class="">
                                    <asp:TextBox ID="txtName" class="span6 m-wrap valStringR" data-required="1" runat="server"></asp:TextBox>
                                </div>
                            </div>
<%--                        </div>--%>
                        <div class="control-group">
                            <label class="control-label span6">From<span class="required">*</span></label>
                            <div class="input-append date form_advance_datetime span6" style="margin-left: 0px;"  data-date="2012-12-21T15:25:00Z">
                                <input onchange='saveSchedule()' id="txtStartDate" style="width: calc(100% - 56px);" type="text" value="" placeholder="From" runat="server" readonly class="m-wrap span8"/>
                                <span class="add-on"><i class="icon-remove"></i></span>
                                <span class="add-on"><i class="icon-calendar"></i></span>
                            </div>
                        </div>

                            
                        
                        <div class="control-group">
                            <label class="control-label span6">To<span class="required">*</span></label>
                            <div class="input-append date form_advance_datetime span6"  style="margin-left: 0px;" data-date="2012-12-21T15:25:00Z">
                                <input onchange='saveSchedule()' id="txtEndDate" type="text" value="" placeholder="To" style="width: calc(100% - 56px);" runat="server" readonly class="m-wrap span8"/>
                                <span class="add-on"><i class="icon-remove"></i></span>
                                <span class="add-on"><i class="icon-calendar"></i></span>
                            </div>
                        </div>


                        <div class="">
                            <label class="span6">Occurs Every<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox ID="txtTime" onkeypress='return isNumber(event)' class="span6 m-wrap valDigits2" data-required="1" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label span6">Sip<span class="required">*</span></label>
                            <div class="controls">
                                <select id="selectSip"  class="span6 m-wrap" tabindex="1">
                                    <asp:Repeater ID="rptSipAccounts" runat="server">
                                        <ItemTemplate>
                                            <option value='<%# Eval("Id") %>'><%# Eval("Username") %></option>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </select>
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label span6">Enabled<span class="required">*</span></label>
                            <div class="controls">
                                <div class="success-toggle-button">
									<input id="chkEnabled" runat="server" type="checkbox" class="toggle"  />
								</div>
                            </div>
                        </div>
                    </div>
                    </div>
                    <div class="span6">
                        <asp:HiddenField ID="HdTable" runat="server" />
                        <div class="row-fluid">

                            <div class="control-group">
                                <div class="controls controls-row">
                                    <select name="" id="selectGroup" class="span4 m-wrap" tabindex="1">
                                        <asp:Repeater ID="rptGroups" runat="server">
                                            <ItemTemplate>
                                                <option value='<%# Eval("Id") %>'><%# Eval("Name") %></option>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </select>

                                    <asp:TextBox ID="txtNumber"  onkeypress='return isNumber(event)' class="span4 m-wrap disableValidation"  runat="server"></asp:TextBox>

                                    <a class="btn green span1" onclick="addToTable()"><i class="icon-plus"></i></a>
                                </div>
                            </div>


                        </div>

                        <div class="row-fluid">

                            <table class="table table-striped table-bordered table-advance table-hover" id="myTable">
                                <thead>
                                    <tr>
                                        <th style="display: none;"></th>
                                        <th><i class="icon-list"></i> Number</th>
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
                            <button class="btn green" id="addBtn" onclick="addSchedule();  return false;">Add New <i class="icon-plus"></i></button>
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
                                <th>Sip
                                </th>
                                <th>Start Date
                                </th>
                                <th>End Date
                                </th>
                                <th>Time
                                </th> 
                                <th style="display: none;">OperatorPrefix
                                </th>
                                <th>Enabled
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
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("SipAccount.UserName") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("StartDate") == null ? " " : Eval("StartDate", "{0:dd/MM/yyyy - HH:mm}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%#  Eval("EndDate") == null ? " " : Eval("EndDate", "{0:dd/MM/yyyy - HH:mm}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("OccursEvery") %>'></asp:Label>
                                        </td>
                                        
                                        <td style="display: none;">
                                            <%# Eval("OperatorPrefix") %>
                                        </td>
                                         <td>
                                             <span class='label <%# ((bool)Eval("IsEnabled")) == true ? "label-success" : "label-important" %> '><asp:Label ID="Label7" runat="server" Text='<%# ((bool)Eval("IsEnabled")) == true ? "Enabled" : "Disabled" %> '></asp:Label></span>
                                        </td>
                                        <td class="center">
                                            <a href="#" onclick='editRow(<%# Eval("Id") %> ,<%# "\"" + Eval("DisplayName") + "\"" %> , <%# "\"" + Eval("OccursEvery") + "\""  %> , <%# "\"" + Eval("SipAccount.Id") + "\""  %>  , <%#  "\"" +  ((DateTime)Eval("StartDate")).ToString("dd-MM-yyyy HH:mm") +"\""%> ,<%#  "\"" + ((DateTime)Eval("EndDate")).ToString("dd-MM-yyyy HH:mm") + "\"" %> , <%# "\"" + Eval("TestGroupId")  + "\""  %> ,  <%# "\"" + Eval("IsEnabled")  + "\""  %> )'>Edit</a>
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
    <asp:HiddenField ID="HiddenFieldSelectSip" runat="server" />
    <asp:HiddenField ID="HiddenFieldSelectGroup" runat="server" />
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

        var handleDatetimePicker = function () {
            $(".form_advance_datetime").datetimepicker({
                isRTL: App.isRTL(),
                format: "dd mm yyyy hh:ii",
                autoclose: true,
                todayBtn: true,
                startDate: "2016-01-01 00:00",
                pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
                minuteStep: 10
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
                    minDate: '01/01/2014'
                }
            );
        }

    </script>
    <!-- END TABLE  SCRIPTS -->

    <script>

        var handleToggleButtons = function () {
            if (!jQuery().toggleButtons) {
                return;
            }
           
            $('.success-toggle-button').toggleButtons({
                style: {
                    enabled: "success",
                    disabled: "danger"
                }
            });
          
        }

        var addToTable = function () {

            var groupId = $('#selectGroup').find(":selected").val();
            var number = $('#<%= txtNumber.ClientID %>').val();
            $('#<%= txtNumber.ClientID %>').val('');

            if (number == "") return;

            var request1 = $.ajax({
                url: "HandlerAddNumber.ashx",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                cache: false,
                async: false,
                data: JSON.stringify([{ "Id": groupId, "Number": number }])
            });

            request1.done(function (msg) {

                $('#myTable > tbody').append('<tr>' +
                                '<td style="display:none;">' + msg.Id + '</td>' +
                                '<td>' + msg.Number + '</td>' +
                                '<td><a href="#" class="btn mini black  pull-right deleteRow" ><i class="icon-trash"></i> Remove</a></td></tr>');

            });
            request1.error(function (msg) {
                alert("Error While adding Number");
            });
        }


        var saveSchedule = function () {
            var fromDate = $('#<%= txtStartDate.ClientID %>').val();
            if (fromDate == "") {
                alert("From date is required");
                return false;
            }

            var toDate = $('#<%= txtEndDate.ClientID %>').val();
            if (toDate == "") {
                alert("End date is required");
                return false;
            }

            if (fromDate != "" && toDate != "") {
                
                var from = fromDate.split(" ");
                var hour = from[3].split(":");
                var strtDt = new Date(from[2], from[1], from[0], hour[0], hour[1], 00);

                var from2 = toDate.split(" ");
                var hour2 = from2[3].split(":");
                var endDt = new Date(from2[2], from2[1], from2[0], hour2[0], hour2[1], 00);

                if (strtDt > endDt) {
                    alert("End date is less then start date");
                    return false;
                }
            }
            return true;
        }

        $(".deleteRow").live('click', function (event) {
            var id = $(this).parent().parent().children("td:first").html();
            var number = $(this).parent().parent().children("td").eq(1).html();

            var request1 = $.ajax({
                url: "HandlerDeleteNumber.ashx",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                cache: false,
                async: false,
                data: JSON.stringify([{ "Id": id, "Number": number }])
            });

            var thisElement = $(this);
            request1.done(function (msg) {
                thisElement.parent().parent().remove();
            });




        });

        var disableAddBtn = function () {
            $('#addBtn').attr('disabled', 'disabled');
            getNumbers();
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
            $('#txtDate').val('');
            $('#txtStartDate').val('');
            $('#txtEndDate').val('');
            $('#txtTime').val('');
            $('#<%=HdTable.ClientID %>').val('');
            $('#chkEnabled').prop('checked', false).change();
            clearValidationMsg();
            
        }

        var addSchedule = function ()
        {
            disableAddBtn();
            emptyFields();
            $("#addDiv").show();

        }

            var editRow = function (id, name, TimeFrequency, sipAccId, startDate, endDate, GroupId, IsEnabled) {
                $('#<%=HiddenFieldSelectGroup.ClientID %>').val(GroupId);
                emptyFields();
                $('#<%=HdnId.ClientID %>').val(id);
                $('#<%=txtName.ClientID %>').val(name);
                $('#<%=txtTime.ClientID %>').val(TimeFrequency);
                if (IsEnabled == "True")
                    $('#chkEnabled').prop('checked', true).change();
                else
                    $('#chkEnabled').prop('checked', false).change();

                $('#<%=txtStartDate.ClientID %>').val(startDate);
                $('#<%=txtEndDate.ClientID %>').val(endDate);
                $('.date-range').setDate();

                $("#selectSip").val(sipAccId);
            

                $('#selectGroup').find('option[value=' + GroupId + ' ]').attr('selected', true);
                $('#selectSip').find('option[value=' + sipAccId + ' ]').attr('selected', true);

                getNumbers();

                $("#addDiv").show();
            }

            var getNumbers = function () {
                var groupId = $('#selectGroup').find(":selected").val();

                var request1 = $.ajax({
                    url: "HandlerGetNumbers.ashx",
                    contentType: "application/json; charset=utf-8",
                    type: 'POST',
                    cache: false,
                    async: false,
                    data: JSON.stringify([{ "id": groupId }])
                });

                request1.done(function (msg) {

                    for (var i = 0 ; i < msg.length ; i++) {
                        $('#myTable > tbody').append('<tr>' +
                                    '<td style="display:none;">' + msg[i].Id + '</td>' +
                                   '<td>' + msg[i].Number + '</td>' +
                                   '<td><a href="#" class="btn mini black  pull-right deleteRow" ><i class="icon-trash"></i> Remove</a></td></tr>');
                    }
                });
            }

            jQuery(document).ready(function () {
                App.init();
                handleValidation2();
                handleToggleButtons();
                handleDateRangePickers();
                handleDatetimePicker();

                $('#selectGroup').on('change', function () {
                    $("#myTable > tbody").find("tr").remove();
                    getNumbers();
                    var groupId = $('#selectGroup').find(":selected").val();
                    $('#<%=HiddenFieldSelectGroup.ClientID %>').val(groupId);

                });

                $('#selectSip').on('change', function () {
                    var sipId = $('#selectSip').find(":selected").val();
                    $('#<%=HiddenFieldSelectSip.ClientID %>').val(sipId);

                });

                var groupId = $('#selectGroup').find(":selected").val();
                $('#<%=HiddenFieldSelectGroup.ClientID %>').val(groupId);

                var sipId = $('#selectSip').find(":selected").val();
                $('#<%=HiddenFieldSelectSip.ClientID %>').val(sipId);


                if (!jQuery().dataTable) {
                    return;
                }

                initTable1();

            });
    </script>
</asp:Content>


