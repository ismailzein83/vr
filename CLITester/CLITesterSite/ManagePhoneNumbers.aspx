<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManagePhoneNumbers.aspx.cs" Inherits="ManagePhoneNumbers" %>

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
            <h3 class="page-title">Manage Phones <small>You can add phone numbers with operators</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Phones</a></li>
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
                    <div class="caption"><i class="icon-reorder"></i>Phone Numbers List</div>
                    <div class="actions">
                        <div class="btn-group">
                            <a class="btn" href="#" data-toggle="dropdown">Columns
										<i class="icon-angle-down"></i>
                            </a>
                            <div id="sample_2_column_toggler" class="dropdown-menu hold-on-click dropdown-checkboxes pull-right">
                                <label>
                                    <input type="checkbox" checked data-column="0">Number</label>
                                <label>
                                    <input type="checkbox" checked data-column="1">Operator</label>
                                <label>
                                    <input type="checkbox" checked data-column="2">Prefix</label>
                                <label>
                                    <input type="checkbox" checked data-column="3">Creation Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="4">Last Call Date</label>
                                <label>
                                    <input type="checkbox" checked data-column="5">Status</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="portlet-body">
                    <div class="table-toolbar">
                        <div class="btn-group span2">
                            <a class="btn green" data-toggle="modal" href="#responsive" onclick="emptyFields()">Add New <i class="icon-plus"></i></a>
                        </div>
                        <div class="control-group span10 form-horizontal">
                        </div>
                    </div>
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Number
                                </th>
                                <th>Operator
                                </th>
                                <th>Prefix
                                </th>
                                <th>Creation Date
                                </th>
                                <th>Last Call Date
                                </th>
                                <th>Status
                                </th>
                                <th>Edit
                                </th>
                                <th>Delete
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPhones" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Number") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <img src='<%# GetURL() + Eval("Operator.CountryPicture") + ".png" %>' alt="" />
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Operator.Country") %>'></asp:Label>&nbsp;-&nbsp; 
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("Operator.Name") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("Prefix") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server"  Text='<%# Eval("CreationDate", "{0:dd/MM/yyyy HH:mm:ss}") %>' ></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server"  Text='<%# Eval("LastCallDate", "{0:dd/MM/yyyy HH:mm:ss}") %>' ></asp:Label>
                                        </td>
                                        <td>
                                            <span class='label <%# ((int)Eval("Status")) == 0 ? "label-success" : "label-important" %> '><asp:Label ID="Label7" runat="server" Text='<%# ((int)Eval("Status")) == 0 ? "Free" : "Busy" %> '></asp:Label></span>
                                        </td>
                                        <td class="center">
                                            <a data-toggle="modal" href="#responsive" onclick='editRow(<%# Eval("Id") %> ,<%# "\"" + Eval("Number") + "\"" %> ,<%# "\"" + Eval("Prefix") + "\"" %> , <%# "\"" + Eval("OperatorId") + "\""  %>)'>Edit</a>

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


    <div id="responsive" class="modal hide fade" data-width="760">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>
            <h3>
                <asp:Label ID="lblTitle" runat="server" Text="Add New Carrier"></asp:Label></h3>

        </div>
        <div class="modal-body">
            <div class="scroller" style="height: 300px" data-always-visible="1" data-rail-visible1="1">
                <!-- BEGIN FORM-->
                <div class="row-fluid">
                    <div class="form-horizontal span12 ">
                        <div class="alert alert-error hide">
                            <button class="close" data-dismiss="alert"></button>
                            You have some form errors. Please check below.
                        </div>
                        <asp:HiddenField ID="HdnId" runat="server" />
                        <div class="control-group">
                            <label class="control-label">Number<span class="required">*</span></label>
                            <div class="controls">
                                <div>
                                    <asp:TextBox ID="txtNumber" onkeypress='return isNumber(event)' class="span8 m-wrap" data-required="1" runat="server"></asp:TextBox>
                                </div>
                                    <span class=" block">Number Shouldn't start with zeroes or '+'</span>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">Prefix<span class="required">*</span></label>
                            <div class="controls">
                                <div>
                                    <asp:TextBox ID="txtPrefix" onkeypress='return isNumber(event)' class="span8 m-wrap" data-required="1" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">Operators</label>
                            <div class="controls controls-row">
                                <select class="span10 select2" tabindex="1" id="selectOperator" onchange="if (this.selectedIndex) OnChange();" name="selectOperator">
                                    <asp:Repeater ID="rptOperators" runat="server">
                                        <ItemTemplate>
                                            <option value='<%# Eval("CountryPicture") %>~<%# Eval("Id") %>'><%# Eval("Country") %> - <%# Eval("Name") %></option>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- END FORM-->

            </div>
        </div>
        <div class="modal-footer form-actions">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
            <asp:Button ID="btnSave" class="btn blue" runat="server" Text="Save" OnClick="btnSave_Click" />
        </div>
    </div>
    <asp:HiddenField ID="hdnOperatorId" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- BEGIN PAGE LEVEL PLUGINS -->
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="assets/plugins/data-tables/DT_bootstrap.js"></script>
    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="assets/scripts/app.js"></script>

    <!-- BEGIN TABLE  SCRIPTS -->
    <script>
        function OnChange(){
            var dl1 = $('select[name=selectOperator]').val();
            var res = dl1.split("~");
            $('#<%=hdnOperatorId.ClientID %>').val(res[1]);
        }
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
                "aaSorting": [[0, 'asc']],
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

        var emptyFields = function () {
            $('#<%=HdnId.ClientID %>').val('');
            $('#<%=txtNumber.ClientID %>').val('');
            $('#<%=txtPrefix.ClientID %>').val('');
            $('#<%=lblTitle.ClientID%>').html('Add Phone Number');
            clearValidationMsg();
        }

        var editRow = function (id, number, prefix, operatorId) {
            emptyFields();
            console.log(operatorId);
            $('#<%=HdnId.ClientID %>').val(id);
            $('#<%=txtNumber.ClientID %>').val(number);
            $('#<%=txtPrefix.ClientID %>').val(prefix);
            $("#selectOperator > option").each(function () {
                if (this.value.match(".*~" + operatorId)) {
                    $('select[name=selectOperator]').select2("val", this.value);
                    $('#<%=hdnOperatorId.ClientID %>').val(operatorId);
                }
            });
            $('#<%=lblTitle.ClientID%>').html('Edit Phone Number');
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

        jQuery(document).ready(function () {

            App.init();
            handleValidation1();
            handleSelect2Modal();
            OnChange();
            if (!jQuery().dataTable) {
                return;
            }

            initTable2();

        });
    </script>
</asp:Content>

