<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TestCall.aspx.cs" Inherits="TestCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--<link href="assets/font-awesome-4.2.0/css/font-awesome.min.css" rel="stylesheet" />--%>
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-multi-select/css/multi-select-metro.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN STYLE CUSTOMIZER -->
            <!-- END BEGIN STYLE CUSTOMIZER -->
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Test Call <small>Please choose an operator and a prefix</small>
            </h3>
            <ul class="breadcrumb">
                <li>
                    <i class="icon-home"></i>
                    <a href="default.aspx">Home</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li>
                    <a href="#">Test Call</a>
                </li>
              
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <div class="row-fluid">
        <div class="span3">
            <div class="control-group">
                <label class="control-label">Operators List</label>
                <div class="controls controls-row">
                    <select class="span10 select2" tabindex="1" id="selectOperator" name="selectOperator">
                        <asp:Repeater ID="rptOperators" runat="server">
                            <ItemTemplate>
                                <option value='<%# Eval("CountryPicture") %>~<%# Eval("Id") %>'><%# Eval("Name") %> - <%# Eval("Country") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                </div>
            </div>

        </div>
        <div class="span3">
            <div class="control-group">
                <label class="control-label">Prefix</label>
                <div class="controls">
                    <select class="medium m-wrap" tabindex="1" name="selector2">
                        <asp:Repeater ID="rptCarriers" runat="server">
                            <ItemTemplate>
                                <option value='<%# Eval("Prefix") %>'><%# Eval("ShortName") %> - <%# Eval("Prefix") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                </div>
            </div>

        </div>
        <div class="span2">
            <a class="btn green big" id="LinkTestCall">Test Call <i class="m-icon-big-swapright m-icon-white"></i></a>
        </div>
        <div class="span3">
            <a class="btn black big" id="LnkClear">Clear <i class=" icon-remove "></i></a>
        </div>
    </div>

    <div class="row-fluid">

        <div class="span12">
            <!-- BEGIN SAMPLE TABLE PORTLET-->
            <div class="portlet box red">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-weibo"></i>Test Call</div>
                    <div class="tools">
                        
					    <%--<a class="btn mini black" id=""><i class=""></i> Clear</a>--%>
					</div>
                </div>
                <div class="portlet-body">
                    <table id="myTable" class="table table-hover">
                        <thead>
                            <tr>
                                <th class="span2">Name</th>
                                <th class="span1">Prefix</th>
                                <th class="span3">Creation Date</th>
                                <th class="span3">End Date</th>
                                <th class="span2">Test Cli</th>
                                <th class="span2">Received Cli</th>
                                <th class="span2">Status</th>
                                 <th class="span1"></th>
                                <th class="span4">Message</th>
                               
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <!-- END SAMPLE TABLE PORTLET-->
        </div>

    </div>
    <div class="row-fluid">

        <div class="alert alert-success" style="visibility: hidden" id="divSuccess">
            <button class="close" data-dismiss="alert"></button>
            <strong></strong>
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </div>
        <div id="divError" style="visibility: hidden" class="alert alert-error">
                <button class="close" data-dismiss="alert"></button>
                <strong>Failure!</strong>
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
    </div>

    <!-- END PAGE CONTENT-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script src="assets/scripts/app.js"></script>
    <script>
        var arrayRow = [];

        var rmvArrayElement = function (array,item) {
            var index = array.indexOf(item);
            if (index > -1) {
                array.splice(index, 1);
            }
        }

        var addTable = function (msg) {
            
            var Progress0 = { Id: "0", Message: " => Number placed to Generator ", flag: true };
            var Progress1 = { Id: "1", Message: "SecMessage", flag: true };

            var statusClass = "label-default";
            if (msg.Status == "CLI NOT DELIVERED")
                statusClass = "label-important"
            else
                if (msg.Status == "CLI DELIVERED")
                    statusClass = "label-success"
                else
                    if (msg.Status == "EXPIRED")
                        statusClass = "label-inverse"
                    else
                        if (msg.Status == "WAITING")
                            statusClass = "label-warning"
                        else
                            if (msg.Status == "ERROR")
                                statusClass = "label-ERROR"
                            else
                                if (msg.Status == "NO STATUS")
                                    statusClass = "label-default"

            var progressImg = "<img src=\"assets/img/input-spinner.gif\" />";
            //msg.progressNbr

            if (msg.progressNbr == Progress0.Id && Progress0.flag) {
                var progressImg = "<img src=\"assets/img/phone-blue.png\" />";
                Progress0.flag = false;
            }

            if (msg.progressNbr == Progress1.Id && Progress1.flag) {
                if (msg.Status == "CLI DELIVERED")
                    var progressImg = "<img src=\"assets/img/phone-green.png\" />";
                if (msg.Status == "CLI NOT DELIVERED")
                    var progressImg = "<img src=\"assets/img/phone-red.png\" />";
                else
                    var progressImg = "<img src=\"assets/img/phone-nostatus.png\" />";
                Progress1.flag = false;
            }


            if (msg.ErrorMessage == "null")
                msg.ErrorMessage = "";

            $('#myTable > tbody').append('<tr>' +
                              '<td>' + msg.OperatorId + '</td>' +
                              '<td>' + msg.Prefix + '</td>' +
                              '<td>' + msg.CreationDate + '</td>' +
                              '<td>' + msg.EndDate + '</td>' +
                              '<td>' + msg.TestCli + '</td>' +
                              '<td>' + msg.ReceivedCli + '</td>' +
                              '<td><span class="label ' + statusClass + '  " >' + msg.Status + '</span></td>' +
                              '<td>' + progressImg + '</td>' +
                              '<td>' + msg.ErrorMessage + '</td>');
        }

        var Clicked = false;
        var timer = null;

        $('#LinkTestCall').bind('click', function (e) {
            e.preventDefault();
            a_onClick();
        });

        $('#LnkClear').bind('click', function (e) {
            e.preventDefault();
            aClear_onClick();
        });
        
        function resetTimer() {
            window.clearInterval(timer);
            timer = null;
        }

        function aClear_onClick() {
            resetTimer();
            Clicked = false;

            arrayRow = [];

            $('#myTable > tbody').html('');

        }

        
        function a_onClick() {
            
            $('#<%=lblError.ClientID %>').html('');
            $('#<%=lblSuccess.ClientID %>').html('');

            var dl1 = $('select[name=selectOperator]').val();
            var dl2 = $('select[name=selector2]').val();
            var res = dl1.split("~");
            var request = $.ajax({
                url: "HandlerTestOperator.ashx",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                cache: false,
                data: JSON.stringify([{ "id1": res[1], "id2": dl2 }])
            });


            request.done(function (msg) {
                if (msg.needRedirect) {
                    window.location = msg.Redirect;
                }
                
                arrayRow.push(msg.Id);
                addTable(msg);

                if (Clicked == false) {

                    Clicked = true;
                    
                    resetTimer();
                    timer = window.setInterval(function () {

                        var request1 = $.ajax({
                            url: "HandlerGetTestOperator.ashx",
                            contentType: "application/json; charset=utf-8",
                            type: 'POST',
                            cache: false,
                            async: false,
                            data: JSON.stringify([{ "id": arrayRow }])
                        });

                        request1.fail(function (xhr, ajaxOptions, thrownError) {
                            //$('#divSuccess').css('visibility', 'hidden');
                            //$('#divError').css('visibility', 'visible');
                            $('#<%=lblError.ClientID %>').html(xhr.status + ' - ' + thrownError);
                            $('#<%=lblSuccess.ClientID %>').html('');
                            console.log('failll');
                            resetTimer();

                    });

                        request1.done(function (msg) {

                        if (msg[0].needRedirect) {
                            window.location = msg[0].Redirect;
                        }

                        var successLblVal = $('#<%=lblSuccess.ClientID %>').html();

                            $('#myTable > tbody > tr').remove();
                            var IsFinished = "true";
                          
                            $.each(msg, function (i, val) {

                                addTable(val);
                                
                                if (val.EndDate == null || val.EndDate == "") {
                                    IsFinished = "false";
                                    //rmvArrayElement(arrayRow, val.Id);
                                }
                            });

                            //if (arrayRow.length == 0) {
                            if (IsFinished == "true") {
                                resetTimer();
                                Clicked = false;
                            }
                    });
                }, 3000);

                }
            });

            request.fail(function (xhr, ajaxOptions, thrownError) {
                //$('#divSuccess').css('visibility', 'hidden');
                //$('#divError').css('visibility', 'visible');
                $('#<%=lblError.ClientID %>').html(xhr.status + ' - ' + thrownError);
                $('#<%=lblSuccess.ClientID %>').html('');

                $('select[name=selectOperator]').removeAttr('disabled');
                $('select[name=selector2]').removeAttr('disabled');
                $('#LinkTestCall').removeClass('disabled');

                //isClick = true;

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

        jQuery(document).ready(function () {
            App.init();

            handleSelect2Modal();
        });

    </script>

</asp:Content>

