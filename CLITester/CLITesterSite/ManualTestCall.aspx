<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManualTestCall.aspx.cs" Inherits="ManualTestCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--<link href="assets/font-awesome-4.2.0/css/font-awesome.min.css" rel="stylesheet" />--%>
    <link rel="stylesheet" type="text/css" href="assets/plugins/select2/select2_metro.css" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-multi-select/css/multi-select-metro.css" />
    <style>
        .hideTd {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN STYLE CUSTOMIZER -->
            <!-- END BEGIN STYLE CUSTOMIZER -->
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Manual Test Call <small>Please choose an operator and a route</small>
            </h3>
            <ul class="breadcrumb">
                <li>
                    <i class="icon-home"></i>
                    <a href="default.aspx">Home</a>
                    <i class="icon-angle-right"></i>
                </li>
                <li>
                    <a href="#">Manual Test Call</a>
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
                                <option value='<%# Eval("CountryPicture") %>~<%# Eval("Id") %>'><%# Eval("Country") %> - <%# Eval("Name") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                    </select>
                </div>
            </div>

        </div>
        <div class="span3">
            <div class="control-group">
                <label class="control-label">Route</label>
                <div class="controls">
                    <%--selectPrefix--%>

                    <select id="selectPrefix" name="selectPrefix" class="medium m-wrap" data-placeholder="Choose a route" tabindex="1">
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
            <a class="btn blue big" id="LnkAdd">Add <i class=" icon-plus "></i></a>
        </div>
        <div class="span2">
            <a class="btn green big" data-toggle="modal" href="#responsive" onclick='editRow()' id="LinkTestCall1" >Start <i class="m-icon-big-swapright m-icon-white"></i></a>            
        </div>
        <div class="span2">
            <a class="btn black big" id="LnkClear">Clear <i class=" icon-remove"></i></a>
        </div>
    </div>

    <div class="row-fluid">

        <div class="span12">
            <!-- BEGIN SAMPLE TABLE PORTLET-->
            <div class="portlet box red">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-weibo"></i>Manual Test Call</div>
                    <div class="tools">

                        <%--<a class="btn mini black" id=""><i class=""></i> Clear</a>--%>
                    </div>
                </div>
                <div class="portlet-body">
                    <table id="myTable" class="table table-hover">
                        <thead>
                            <tr>
                                <th class="span3">Name</th>
                                <th class="span1">Prefix</th>
                                <th class="span1">Route</th>
                                <th class="span3">Creation Date</th>
                                <th class="span3">End Date</th>
                                <th class="span3">PDD</th>
                                <th class="span3">Duration</th>
                                <th class="span2">Caller ID</th>
                                <th class="span2">Received Cli</th>
                                <th class="span2">Status</th>
                                <th class="span3">Message</th>
                                <th class="span1"></th>
                            </tr>
                        </thead>
                        <tbody>
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

    <div id="responsive" class="modal hide fade" tabindex="-1" data-width="160">
        <div class="modal-body fade in">
            <div data-always-visible="1" data-rail-visible1="1">

                <div class="span12">

                    <div class="portlet box red">
                        <div class="portlet-title">
                            <div class="caption">Confirm Test Call</div>
                        </div>
                        <div class="portlet-body">
                            <p><span class="icon icon-warning-sign"></span>
							    These test calls will be started and cannot be recovered. Are you sure?
							</p>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <div class="modal-footer form-actions">
            <button type="button" data-dismiss="modal" class="btn">Close</button>
             <asp:Button id="LinkTestCall"  data-dismiss="modal" class="btn blue" runat="server" Text="Start" />
        </div>
    </div>
    <asp:HiddenField runat="server" ID="imgSRC" />
    <!-- END PAGE CONTENT-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script type="text/javascript" src="assets/plugins/select2/select2.min.js"></script>
    <script src="assets/scripts/app.js"></script>
    <script>
        var arrayRow = [];



        var rmvArrayElement = function (array, item) {
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
                    if (msg.Status == "FAILED")
                        statusClass = "label-warning"
                    else
                        if (msg.Status == "WAITING")
                            statusClass = "label-warning"
                        else
                            if (msg.Status == "ERROR")
                                statusClass = "label-ERROR"
                            else
                                if (msg.Status == "FAS")
                                    statusClass = "label-inverse"
                                else
                                    if (msg.Status == "NO STATUS")
                                        statusClass = "label-default"

            var progressImg = "<img src=\"assets/img/input-spinner.gif\" />";

            if (msg.progressNbr == Progress0.Id && Progress0.flag) {
                var progressImg = "<img src=\"assets/img/phone-blue.png\" />";
                Progress0.flag = false;
            }

            if (msg.progressNbr == Progress1.Id && Progress1.flag) {
                if (msg.Status == "CLI DELIVERED")
                    var progressImg = "<img src=\"assets/img/phone-green.png\" />";
                else if (msg.Status == "CLI NOT DELIVERED")
                    var progressImg = "<img src=\"assets/img/phone-red.png\" />";
                else
                    var progressImg = "<img src=\"assets/img/phone-nostatus.png\" />";
                Progress1.flag = false;
            }


            if (msg.ErrorMessage == "null")
                msg.ErrorMessage = "";

            var dl1 = $('select[name=selectOperator]').val();
            var res = dl1.split("~");
            console.log("CountryPic: " + msg.CountryPic);
            var imgSrc = $('#<%=imgSRC.ClientID %>').val() + '';
            imgSrc = imgSrc + msg.CountryPic + '.png';

            $('#myTable > tbody').append('<tr>' +
                                 '<td class="hideTd">' + msg.idOp + '</td>' +
                                '<td class="hideTd">' + msg.Operator + '</td>' +
                                '<td class="hideTd">' + msg.CountryPic + '</td>' +
                              '<td><img src=' + imgSrc + ' alt="" /> ' + msg.OperatorId + '</td>' +
                              '<td>' + msg.phonePrefix + '</td>' +
                              '<td>' + msg.Prefix + '</td>' +
                              '<td>' + msg.CreationDate + '</td>' +
                              '<td>' + msg.EndDate + '</td>' +
                              '<td>' + msg.PDD + '</td>' +
                              '<td>' + msg.Duration + '</td>' +
                              '<td>' + msg.TestCli + '</td>' +
                              '<td>' + msg.ReceivedCli + '</td>' +
                              '<td><span class="label ' + statusClass + '  " >' + msg.Status + '</span></td>' +
                              '<td>' + msg.ErrorMessage + '</td>' +
                              '<td>' + progressImg + '</td>' +
                              '<td class="hideTd">' + msg.Id + '</td>' +
                              '<td><a  class="btn blue icn-only testCall" ><i class="icon-repeat icon-white"></i></a></td>' +
                              '<td><a class="btn red icn-only clearCall"><i class="icon-remove icon-white"></i></a></td></tr>');
        }

        var Clicked = false;
        var timer = null;
        var boolfinish = true;

        $('#LinkTestCall').bind('click', function (e) {
            e.preventDefault();
            if (boolfinish) {
                boolfinish = false;
                a_onClick();

            }
        });

        $('#LnkAdd').bind('click', function (e) {
            e.preventDefault();
            if (boolfinish)
                aAdd_onClick();
        });

        $('#LnkClear').bind('click', function (e) {
            e.preventDefault();
            aClear_onClick();
            boolfinish = true;

            $('#LinkTestCall').removeClass('disabled');
            $('#LinkTestCall1').removeClass('disabled');
            $('#LnkAdd').removeClass('disabled');

        });

        function aAdd_onClick() {

            var dl2 = $('select[name=selectPrefix]').val();
            var dl1 = $('select[name=selectOperator]').val();
            var res = dl1.split("~");

            var imgSrc = $('#<%=imgSRC.ClientID %>').val() + '';
            imgSrc = imgSrc + res[0] + '.png';

            $('#myTable > tbody').append('<tr>' +
                                 '<td class="hideTd">0</td>' +
                                 '<td class="hideTd">' + res[1] + '</td>' +
                                  '<td class="hideTd">' + res[0] + '</td>' +
                              '<td><img src=' + imgSrc + ' alt="" /> ' + $('#selectOperator :selected').text() + '</td>' +
                              '<td>' + dl2 + '</td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td class="hideTd">0</td>' +
                              '<td><a  class="btn blue icn-only testCall" ><i class="icon-repeat icon-white"></i></a></td>' +
                              '<td><a class="btn red icn-only clearCall"><i class="icon-remove icon-white"></i></a></td></tr>');

        }

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


        function testCall(idOperator, idPrefix) {
            $('#<%=lblError.ClientID %>').html('');
            $('#<%=lblSuccess.ClientID %>').html('');
            $('#divError').css('visibility', 'hidden');

            var request = $.ajax({
                url: "HandlerTestOperator.ashx",
                contentType: "application/json; charset=utf-8",
                type: 'POST',
                cache: false,
                data: JSON.stringify([{ "id1": idOperator, "id2": idPrefix }])
            });

            request.done(function (msg) {
                if (msg.needRedirect) {
                    window.location = msg.Redirect;
                }

                if (msg.isDone == false) {
                    $('#divError').css('visibility', 'visible');
                    $('#<%=lblError.ClientID %>').html('Sorry, you cant add a test call since your balance is empty');

                }
                else {
                    msg.idOp = idOperator;
                    arrayRow.push(msg.Id);
                    //addTable(msg);

                    if (Clicked == false) {

                        Clicked = true;

                        resetTimer();
                        timer = window.setInterval(function () {
                            if (arrayRow.length == 0) { aClear_onClick(); return; }

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
                                    val.idOp = idOperator;
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
                                    boolfinish = true;

                                    $('#LinkTestCall').removeClass('disabled');
                                    $('#LinkTestCall1').removeClass('disabled');
                                    $('#LnkAdd').removeClass('disabled');
                                    arrayRow = [];
                                }
                            });
                        }, 3000);
                    }

                }
            });

            request.fail(function (xhr, ajaxOptions, thrownError) {
                //$('#divSuccess').css('visibility', 'hidden');
                //$('#divError').css('visibility', 'visible');
                $('#<%=lblError.ClientID %>').html(xhr.status + ' - ' + thrownError);
                $('#<%=lblSuccess.ClientID %>').html('');

                //$('select[name=selectOperator]').removeAttr('disabled');
                //$('select[name=selectPrefix]').removeAttr('disabled');
                //$('#LinkTestCall').removeClass('disabled');

                //isClick = true;

            });
        }

        function a_onClick() {
            arrayRow = [];
            $('#LinkTestCall').addClass('disabled');
            $('#LinkTestCall1').addClass('disabled');
            $('#LnkAdd').addClass('disabled');

            $('#myTable > tbody  > tr').each(function () {

                var op = $('td:nth-child(2)', $(this)).html();
                var prefix = $('td:nth-child(5)', $(this)).html();
                console.log("OP: " + op + " PRE: " + prefix);

                if (typeof op === "undefined") {
                    console.log("undefined");
                }
                else {
                    if(op != null && op != "null")
                        testCall(op, prefix);
                }

            });
        }


        $('#myTable').delegate('a.testCall', 'click', function (e) {

            var opId = $('td:first-child', $(this).parents('tr')).html();
            var prefix = $('td:nth-child(5)', $(this).parents('tr')).html();
            var opName = $('td:nth-child(4)', $(this).parents('tr')).html();
            var country = $('td:nth-child(3)', $(this).parents('tr')).html();


            var dl2 = $('select[name=selectPrefix]').val();
           
            var dl1 = $('select[name=selectOperator]').val();
            var res = dl1.split("~");

            var imgSrc = $('#<%=imgSRC.ClientID %>').val() + '';
            imgSrc = imgSrc + res[0] + '.png';


            $('#myTable > tbody').append('<tr>' +
                                 '<td class="hideTd">0</td>' +
                                  '<td class="hideTd">0</td>' +
                                  '<td class="hideTd">' + country + '</td>' +
                                 '<td class="hideTd">' + opId + '</td>' +
                              '<td><img src=' + imgSrc + ' alt="" /> ' + opName + '</td>' +
                              '<td>' + prefix + '</td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td></td>' +
                              '<td class="hideTd">0</td>' +
                              '<td><a  class="btn blue icn-only testCall" ><i class="icon-repeat icon-white"></i></a></td>' +
                              '<td><a class="btn red icn-only clearCall"><i class="icon-remove icon-white"></i></a></td></tr>');

        });

        $('#myTable').delegate('a.clearCall', 'click', function (e) {

            var id = $('td:nth-child(12)', $(this).parents('tr')).html();
            $(this).parents('tr').remove();
            rmvArrayElement(arrayRow, id);
        });

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

        jQuery(document).ready(function () {
            App.init();

            handleSelect2Modal();
        });

    </script>

</asp:Content>



