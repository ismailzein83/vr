<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageCarriers.aspx.cs" Inherits="ManageCarriers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" href="assets/plugins/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/bootstrap-fileupload/bootstrap-fileupload.css" />
    <!-- END PAGE LEVEL STYLES -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
        <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Manage Carriers <small>You can add carriers from an excel sheet</small>
            </h3>
            <ul class="breadcrumb">
                <li><i class="icon-home"></i><a href="default.aspx">Home</a><i class="icon-angle-right">
                </i></li>
                <li><a href="#">Manage Carriers</a></li>
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
                    <div class="caption"><i class="icon-reorder"></i>Carriers List</div>
                    <div class="actions">
                        <div class="btn-group">
                            <a class="btn" href="#" data-toggle="dropdown">Columns
										<i class="icon-angle-down"></i>
                            </a>
                            <div id="sample_2_column_toggler" class="dropdown-menu hold-on-click dropdown-checkboxes pull-right">
                                <label>
                                    <input type="checkbox" checked data-column="0">Name</label>
                                <label>
                                    <input type="checkbox" checked data-column="1">Route</label>
                                <label>
                                    <input type="checkbox" checked data-column="2">Short Name</label>
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
                            <label class="control-label">Upload From Excel (.csv)</label>
                            <div class="controls">
                                <div class="fileupload fileupload-new" data-provides="fileupload">
                                    <div class="input-append">
                                        <div class="uneditable-input">
                                            <i class="icon-file fileupload-exists"></i>
                                            <span class="fileupload-preview"></span>

                                        </div>
                                        <span class="btn btn-file">

                                            <span class="fileupload-new">Select file</span>
                                            <span class="fileupload-exists">Change</span>
                                            <input type="file" class="default" runat="server" id="fileAttachment" />

                                        </span>
                                        <a href="#" class="btn fileupload-exists" data-dismiss="fileupload">Remove</a>
                                    </div>
                                    <asp:Button ID="btnImport" class="btn green cancel" runat="server" Text="Import" OnClick="btnImport_Click" />
                                    <a href="Resources/CarriersTemplate.csv" class="btn purple">CSV Template <i class="icon-download"></i></a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <table class="table table-striped table-bordered table-hover table-full-width" id="sample_2">
                        <thead>
                            <tr>
                                <th visible="false" runat="server" id="th1">Id
                                </th>
                                <th>Name
                                </th>

                                <th>Route
                                </th>
                                <th>Short Name
                                </th>
                                <th>Edit
                                </th>
                                <th>Delete
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptCarriers" runat="server">
                                <ItemTemplate>
                                    <tr class="odd gradeX">
                                        <td runat="server" id="tdId" visible="false">
                                            <asp:Label ID="Label0" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Prefix") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("ShortName") %>'></asp:Label>
                                        </td>
                                        <td class="center">
                                            <a data-toggle="modal" href="#responsive" onclick='editRow(<%# Eval("Id") %> ,<%# "\"" + Eval("Name") + "\"" %> , <%# "\"" + Eval("Prefix") + "\""  %> ,<%#  "\"" + Eval("ShortName") + "\"" %> )'>Edit</a>

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


    <div id="responsive" class="modal hide fade " tabindex="-1" data-width="760">
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
                        <asp:HiddenField ID="HdnId" runat="server"/>
                        <div class="control-group">
                            <label class="control-label">Name<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox ID="txtName" class="span8 m-wrap valString" data-required="1" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="control-group">
                            <label class="control-label">Route<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox ID="txtPrefix" onkeypress='return isNumber(event)' class="span8 m-wrap valDigits" data-required="1" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="control-group">
                            <label class="control-label">Short Name<span class="required">*</span></label>
                            <div class="controls">
                                <asp:TextBox ID="txtShortName"   class="span8 m-wrap valStringRemote" data-required="1" runat="server"></asp:TextBox>
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
            $('#<%=txtName.ClientID %>').val('');
            $('#<%=txtPrefix.ClientID %>').val('');
            $('#<%=txtShortName.ClientID %>').val('');
            $('#<%=lblTitle.ClientID%>').html('Add New Carrier');
            clearValidationMsg();
        }

        var editRow = function (id, name, prefix, shortname) {

            emptyFields();
            $('#<%=HdnId.ClientID %>').val(id);
            $('#<%=txtName.ClientID %>').val(name);
            $('#<%=txtPrefix.ClientID %>').val(prefix);
            $('#<%=txtShortName.ClientID %>').val(shortname);
            $('#<%=lblTitle.ClientID%>').html('Edit Carrier');
        }



        jQuery(document).ready(function () {

            App.init();
            handleValidation1();
            var form1 = $('#form_sample_1');
            form1.find('.valStringRemote').each(function () {
                $(this).rules('add', {
                    required: true,
                    minlength: 3,
                    remote: {
                        url: "HandlerRemoteValidation.ashx",
                        type: "POST",
                        cache: false,
                        dataType: "json",
                        data: {
                            id: function () {
                                return $('#<%=HdnId.ClientID %>').val();
                            }
                        },
                        dataFilter: function (response) {
                            return checkSuccess(response);
                        }
                    },
                    messages: {
                        remote: "Short Name Exist"
                    }
                });
            });

            if (!jQuery().dataTable) {
                return;
            }

            initTable2();

        });
    </script>
</asp:Content>

