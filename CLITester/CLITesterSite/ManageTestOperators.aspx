<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageTestOperators.aspx.cs" Inherits="ManageTestOperators" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datepicker/css/datepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-daterangepicker/daterangepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-datetimepicker/css/datetimepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/chosen-bootstrap/chosen/chosen.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-multi-select/css/multi-select-metro.css" />
    <link href="assets/css/pages/search.css" rel="stylesheet" type="text/css" />
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
<%--        <div class="span3">
            <select id="ddClientId" class="span10" tabindex="1">
                <option value="">Select Operator</option>
                <asp:Repeater ID="rptCarriers" runat="server">
                    <ItemTemplate>
                        <option value='<%# Eval("Id") %>'><%# Eval("Name")  == null ? "" : Eval("Name") %></option>
                    </ItemTemplate>
                </asp:Repeater>
            </select>
        </div>--%>

        <div class="span3">
            <div class="control-group">
                <%--<label class="control-label">Operators List</label>--%>
                <div class="controls controls-row">
                    <select class="span10 select2" tabindex="1" id="selectOperator" name="selectOperator">
                        <option value="aa">All</option>
                        <asp:Repeater ID="rptOperators" runat="server">
                            <ItemTemplate>
                                <option value='<%# Eval("CountryPicture") %>~<%# Eval("Id") %>'><%# Eval("Country") %> - <%# Eval("Name") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                </div>
            </div>
        </div>

        <div class="span3">
            <div class="input-append date date-picker span12" data-date="12-02-2012" data-date-format="dd-mm-yyyy" data-date-viewmode="years">
			    <input id="txtStartDate" class="m-wrap span8 m-ctrl-medium date-picker" placeholder="From" readonly size="16" type="text" value="" />
                <span class="add-on"><i class="icon-calendar"></i></span>
			</div>
<%--       <div class="input-append date form_advance_datetime span12" data-date="2012-12-21T15:25:00Z">
                <input id="txtStartDate" type="text" value="" placeholder="From" readonly class="m-wrap span8">
                <span class="add-on"><i class="icon-remove"></i></span>
                <span class="add-on"><i class="icon-calendar"></i></span>
            </div>--%>
        </div>

        <div class="span3">
            <div class="input-append date date-picker span12" data-date="12-02-2012" data-date-format="dd-mm-yyyy" data-date-viewmode="years">
			    <input id="txtEndDate" class="m-wrap span8 m-ctrl-medium date-picker" placeholder="To" readonly size="16" type="text" value="" />
                <span class="add-on"><i class="icon-calendar"></i></span>
			</div>
<%--            <div class="input-append date form_advance_datetime span12" data-date="2012-12-21T15:25:00Z">
                <input id="txtEndDate" type="text" value="" placeholder="To" readonly class="m-wrap span8">
                <span class="add-on"><i class="icon-remove"></i></span>
                <span class="add-on"><i class="icon-calendar"></i></span>
            </div>--%>
        </div>

        <div class="span2">
            <button id="btnSearch" type="button" class="btn green">
                Search &nbsp; 
				<i class="m-icon-swapright m-icon-white"></i>
            </button>

            <button id="btnPrint" type="button" class="btn blue">
                Print &nbsp; 
				<i class="icon-print"></i>
            </button>

                <button id="btnClear" type="button" class="btn red">
                Clear &nbsp; 
				<i class="icon-remove"></i>
            </button>

<%--            <a onclick="btnSave_Click" target="_blank">
            <img src="./images/print.png" style="border: 0;" alt="Print" /></a>

            <asp:Button ID="btnPrint" class="btn blue btn-large" runat="server" Text="Print" OnClick="btnSave_Click"  />--%>
        </div>
    </div>

    <div class="row-fluid">
        <div class="span12">
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
                                    <input type="checkbox" checked data-column="1">Creation Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="2">End Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="3">PDD</label>
                                <label>
                                    <input type="checkbox" checked data-column="4">Duration</label>
                                <label>
                                    <input type="checkbox" checked data-column="5">Schedule</label>
                                <label>
                                    <input type="checkbox" checked data-column="6">Caller ID</label>
                                <label>
                                    <input type="checkbox" checked data-column="7">Received Cli</label>
                                <label>
                                    <input type="checkbox" checked data-column="8">Status</label>
                                <label>
                                    <input type="checkbox" checked data-column="9">Error Message</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th class="span2">Name</th>
                                <th class="span2">Route</th>
                                <th class="span2">Creation Date</th>
                                <th class="span2">End Date</th>
                                <th class="span1">PDD</th>
                                <th class="span1">Duration</th>
                                <th class="span2">Schedule</th>
                                <th class="span2">Caller ID</th>
                                <th class="span2">Received Cli</th>
                                <th class="span2">Status</th>
                                <th class="span2">Error Message</th>
                            </tr>
                        </thead>
                        <tbody class="table table-striped table-bordered table-advance table-hover">
                            <tr>
                                <td colspan="13" class="dataTables_empty">Loading data from server
                                </td>
                            </tr>
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
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>
    <script type="text/javascript" src="assets/plugins/jquery-multi-select/js/jquery.multi-select.js"></script>
    <script src="assets/plugins/data-tables/fnReloadAjax.js"></script>
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
                "aLengthMenu": [
                   [10, 15, 20],
                   [10, 15, 20] // change per page values here
                ],
                // set the initial value
                "iDisplayLength": 10,
                'bFilter': false,
                "processing": true,
                'bServerSide': true,
                "bSort": false,
                'aoColumns': [null, null, null, null, null, null, null, null, null, null, null],
                'bStateSave': false,
                'sAjaxSource': 'SearchTestOpHandler.ashx?operatorId=0&startDate=' + $('#txtStartDate').val() + '&endDate=' + $('#txtEndDate').val(),
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

            $("#btnSearch").click(function () {
                var dl1 = $('select[name=selectOperator]').val();
                var res = dl1.split("~");
                var ajaxHandler = 'SearchTestOpHandler.ashx?operatorId=' + res[1] + '&startDate=' + $('#txtStartDate').val() + '&endDate=' + $('#txtEndDate').val();
                oTable.fnPageChange(0);
                setTimeout(function () {
                    oTable.fnReloadAjax(ajaxHandler);
                }, 500);
            });

            $("#btnPrint").click(function () {
                var dl1 = $('select[name=selectOperator]').val();
                var res = dl1.split("~");
                var opId = 0
                if (typeof res[1] === 'undefined')
                    opId = 0;
                else
                    opId = res[1];

                window.open('TestOperatorHistoryPrint.aspx?operatorId=' + opId + '&startDate=' + $('#txtStartDate').val() + '&endDate=' + $('#txtEndDate').val());
            });

            $("#btnClear").click(function () {
                $("#selectOperator").select2('val', 'aa');
                $('#txtStartDate').val("");
                $('#txtEndDate').val("");

                $("#btnSearch").click();
            });
        }

        var handleDatePickers = function () {

            if (jQuery().datepicker) {
                $('.date-picker').datepicker({
                    rtl: App.isRTL(),
                    format: "dd MM yyyy",
                });
            }
        }


        var handleDatetimePicker = function () {
            $(".form_advance_datetime").datetimepicker({
                isRTL: App.isRTL(),
                format: "dd MM yyyy - hh:ii",
                autoclose: true,
                todayBtn: true,
                startDate: "2015-01-01 00:00",
                pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
                minuteStep: 10
            });
        }

        var handleSelect2Modal = function () {

            function format(state) {
                if (!state.id) return state.text; // optgroup
                return "<img class='flag' src='assets/img/flags/" + state.id.toLowerCase().split('~')[0] + ".png'/>&nbsp;&nbsp;" + state.text;
            }

            $("#selectOperator").select2({
                allowClear: true,
                formatResult: format,
                formatSelection: format,
                escapeMarkup: function (m) {
                    return m;
                }
            });
        }
    </script>
    <!-- END TABLE  SCRIPTS -->

    <script>
        jQuery(document).ready(function () {
            App.init();
            handleValidation1();
            handleDatetimePicker();
            handleSelect2Modal();
            handleDatePickers();

            if (!jQuery().dataTable) {
                return;
            }

            initTable2();

        });
    </script>
</asp:Content>

