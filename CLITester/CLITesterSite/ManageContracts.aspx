<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageContracts.aspx.cs" Inherits="ManageContracts" %>


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
            <h3 class="page-title">Manage Contracts
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Contracts</a></li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <%--<div class="row-fluid">
        <div class="span8">

            <!-- BEGIN EXAMPLE TABLE PORTLET-->
            <div class="portlet box blue">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-cogs"></i>Recharge settings</div>
                    <div class="tools">
                    </div>
                </div>
                <div class="portlet-body form">
                    <!-- BEGIN FORM-->
                    <form action="#" class="horizontal-form">
                    <h3 class="form-section">
                        Recharge</h3>

                    <div class="row-fluid">
                        <div class="span3">
                            <div class="control-group">
                                <label class="control-label" for="beginDate">
                                    Begin Date<span class="required">*</span></label>
                                <div class="controls">
                                    <div class="input-append date date-picker span12" data-date="12-02-2012" data-date-format="dd-mm-yyyy" data-date-viewmode="years">
			                            <input id="txtStartDate" class="m-wrap span8 m-ctrl-medium date-picker" placeholder="Begin Date" readonly size="16" type="text" value="" />
                                        <span class="add-on"><i class="icon-calendar"></i></span>
			                        </div>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                        <div class="span3">
                            <div class="control-group">
                                <label class="control-label" for="NumberOfMonths">
                                    Number Of Months<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNumberOfMonths" onkeypress='return isNumber(event)'  runat="server" CssClass="m-wrap span12" placeholder="Number Of Months"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtNumberOfMonths"
                                        ValidationGroup="Usr" ErrorMessage="Number Of Months Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="span3">
                            <div class="control-group">
                                <label class="control-label" for="lastName">
                                    Value<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtValue" onkeypress='return isNumber(event)'  runat="server" CssClass="m-wrap span12" placeholder="Value"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtValue"
                                        ValidationGroup="Usr" ErrorMessage="Value Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>
                    <!--/row-->
                    <div class="form-actions">

                        
            <asp:Button ID="btnSave" class="btn blue" runat="server" Text="Save" OnClick="btnSave_Click" />

                       
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn" OnClick="btnCancel_Click"  Text="Cancel" />
                    </div>
                    </form>
                    <!-- END FORM-->
                </div>
            </div>
            <!-- END EXAMPLE TABLE PORTLET-->

        </div>

            <div class="span4">

            <!-- BEGIN EXAMPLE TABLE PORTLET-->
            <div class="portlet box blue">
                <div class="portlet-title">
                    <div class="caption">
                        <i class="icon-star"></i>Bonus</div>
                    <div class="tools">
                    </div>
                </div>
                <div class="portlet-body form">
                    <!-- BEGIN FORM-->
                    <form action="#" class="horizontal-form">
                    <h3 class="form-section">
                        Add Bonus</h3>

                    <div class="row-fluid">
                        <!--/span-->
                        <div class="span6">
                            <div class="control-group">
                                <label class="control-label" for="BonusValue">
                                    Bonus Value<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtBonusValue" onkeypress='return isNumber(event)' runat="server" CssClass="m-wrap span12" placeholder="Bonus Value"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBonusValue"
                                        ErrorMessage="Bonus Value Required" Display="Dynamic" Font-Bold="true"
                                        ForeColor="Red">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <!--/span-->
                    </div>
                    <!--/row-->
                    <div class="form-actions">

                          <asp:Button ID="btnSaveBonus" class="btn blue" runat="server" Text="Save" OnClick="btnSaveBonus_Click" />


                        <asp:Button runat="server" ID="btnCancelBonus" CssClass="btn" OnClick="btnCancel_Click" Text="Cancel" />
                    </div>
                    </form>
                    <!-- END FORM-->
                </div>
            </div>
            <!-- END EXAMPLE TABLE PORTLET-->

        </div>
    </div>--%>

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
                    <div class="caption"><i class="icon-reorder"></i>Contracts Log</div>
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
                                    <input type="checkbox" checked data-column="2">ChargeType</label>
                                <label>
                                    <input type="checkbox" checked data-column="3">Calls</label>
                                <label>
                                    <input type="checkbox" checked data-column="4">Period Type</label>
                                <label>
                                    <input type="checkbox" checked data-column="5">Period</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                     <div class="table-toolbar">
                        <div class="btn-group span2">
                            <asp:LinkButton ID="btnNew" runat="server"  ToolTip="Add New" CssClass="btn green" OnClick="btnNew_Click" >Add New
                                <i class="icon-plus"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="control-group span10 form-horizontal">
                        </div>
                    </div>
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Name
                                </th>
                                <th>Creation Date
                                </th>
                                <th>Charge Type
                                </th>
                                <th>Calls
                                </th>
                                <th>Period Type
                                </th>
                                <th>Period
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptContracts" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("CreationDate") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("ChargeType") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("NumberOfCalls") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("Period") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("NumberOfPeriod") %>'></asp:Label>
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
    <script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/plugins/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>
    <script type="text/javascript" src="assets/plugins/jquery-multi-select/js/jquery.multi-select.js"></script>
    <script src="assets/plugins/data-tables/fnReloadAjax.js"></script>
    <script src="assets/scripts/app.js"></script>


    <script>
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
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
            handleDatetimePicker();
            handleDatePickers();

            if (!jQuery().dataTable) {
                return;
            }

            initTable2();

        });
    </script>
</asp:Content>


