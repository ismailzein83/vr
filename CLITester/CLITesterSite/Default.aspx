<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- BEGIN PAGE HEADER-->
    <div class="row-fluid">
        <div class="span12">
            <!-- BEGIN STYLE CUSTOMIZER -->
            <!-- END BEGIN STYLE CUSTOMIZER -->
            <!-- BEGIN PAGE TITLE & BREADCRUMB-->
            <h3 class="page-title">Dashboard <small>statistics and more</small>
            </h3>
            <ul class="breadcrumb">
                <li>
                    <i class="icon-home"></i>
                    <a href="default.aspx">Home</a>
                </li>
            </ul>
            <!-- END PAGE TITLE & BREADCRUMB-->
        </div>
    </div>
    <!-- END PAGE HEADER-->
    <!-- BEGIN PAGE CONTENT-->
    <div class="row-fluid">
        <div class="span3 responsive" data-tablet="span6" data-desktop="span3">
            <div class="dashboard-stat blue">
                <div class="visual">
                    <i class="icon-phone"></i>
                </div>
                <div class="details">
                    <div class="desc">
                        <table>
                            <tr><td>Completed: </td><td><asp:Label ID="lblTotalCalls" runat="server"/></td></tr>
                            <tr><td>Remaining: </td><td><asp:Label ID="lblRemaining" runat="server"/></td></tr>
                            <tr><td>Failed: </td><td><asp:Label ID="lblFailed" runat="server"/></td></tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="TestCall.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6" data-desktop="span3">
            <div class="dashboard-stat green">
                <div class="visual">
                    <i class="icon-ok-circle"></i>
                </div>
                <div class="details">
                    <div class="desc">
                        <table>
                            <tr><td>Delivered: </td><td><asp:Label ID="lblCLIDel" runat="server"/></td></tr>
                            <tr><td>Not Delivered: </td><td><asp:Label ID="lblCLINonDel" runat="server"/></td></tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="ManageTestOperators.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6  fix-offset" data-desktop="span3">
            <div class="dashboard-stat red">
                <div class="visual">
                    <i class="icon-remove-circle"></i>
                </div>
                <div class="details">
                    <div class="number"><asp:Label ID="lblCLINonDel2" runat="server"></asp:Label></div>
                    <div class="desc">Non Valid</div>
                </div>
                <a class="more" href="ManageTestOperators.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6" data-desktop="span3">
            <div class="dashboard-stat yellow">
                <div class="visual">
                    <i class="icon-bar-chart"></i>
                </div>
                <div class="details">
                    <div class="number">12 Points</div>
                    <div class="desc">Total Profit</div>
                </div>
                <a class="more" href="#">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
    </div>
    <div class="row-fluid">

        <div class="span12">
            <!-- BEGIN PORTLET-->
            <div class="portlet solid bordered light-grey">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-bar-chart"></i>Statistics</div>
                </div>
                <div class="portlet-body">
                    <div id="site_statistics_loading">
                        <img src="assets/img/loading.gif" alt="loading" />
                    </div>
                    <div id="site_statistics_content" class="hide">
                        <div id="site_statistics" class="chart"></div>
                    </div>
                </div>
            </div>
            <!-- END PORTLET-->
        </div>
    </div>
    <!-- END PAGE CONTENT-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    	<script src="assets/plugins/flot/jquery.flot.js" type="text/javascript"></script>
	<script src="assets/plugins/flot/jquery.flot.resize.js" type="text/javascript"></script>
	<script src="assets/plugins/jquery.pulsate.min.js" type="text/javascript"></script>
    <script src="assets/scripts/app.js"></script>
    <script>
      
        var initCharts = function () {
            if (!jQuery.plot) {
                return;
            }

            function showTooltip(title, x, y, contents ,xx,yy) {
                $('<div id="tooltip" class="chart-tooltip"><div class="date"><\/div><div class="label label-info">' + yy + '<\/div><\/div>').css({
                    position: 'absolute',
                    display: 'none',
                    top: y - 50,
                    left: x - 40,
                    border: '0px solid #ccc',
                    padding: '2px 6px',
                    'background-color': '#fff',
                }).appendTo("body").fadeIn(200);
            }

            function randValue() {
                return (Math.floor(Math.random() * (1 + 50 - 20))) + 10;
            }

            if ($('#site_statistics').size() != 0) {

                var request = $.ajax({
                    url: "HandlerGetChartCalls.ashx?status=2",
                    contentType: "application/json; charset=utf-8",
                    type: 'POST',
                    cache: false,
                    async: false
                });

                request.fail(function (xhr, ajaxOptions, thrownError) {

                });

                request.done(function (msg) {

                    var request2 = $.ajax({
                        url: "HandlerGetChartCalls.ashx?status=1",
                        contentType: "application/json; charset=utf-8",
                        type: 'POST',
                        cache: false,
                        async: false
                    });

                    request2.done(function (msg2) {

                        $('#site_statistics_loading').hide();
                        $('#site_statistics_content').show();

                        var arr = Object.keys(msg).map(function (k) {
                            return [msg[k].ChartId, msg[k].Total];
                        });

                        var arr2 = Object.keys(msg2).map(function (k) {
                            return [msg2[k].ChartId, msg2[k].Total ];
                        });

                        
                        var plot_statistics = $.plot($("#site_statistics"), [{
                            data: arr,
                            label: "Non Valid"
                        }, {
                            data: arr2,
                            label: "CLI Valid"
                        }
                        ], {
                            series: {
                                lines: {
                                    show: true,
                                    lineWidth: 2,
                                    fill: true,
                                    fillColor: {
                                        colors: [{
                                            opacity: 0.05
                                        }, {
                                            opacity: 0.01
                                        }
                                        ]
                                    }
                                },
                                points: {
                                    show: true
                                },
                                shadowSize: 2
                            },
                            grid: {
                                hoverable: true,
                                clickable: true,
                                tickColor: "#eee",
                                borderWidth: 0
                            },
                            colors: ["#d12610", "#52e136", "#52e136"],
                            xaxis: {
                                ticks: 31,
                                tickDecimals: 0
                            },
                            yaxis: {
                                ticks: 11,
                                tickDecimals: 0
                            }
                        });

                        var previousPoint = null;
                        $("#site_statistics").bind("plothover", function (event, pos, item) {
                            $("#x").text(pos.x.toFixed(2));
                            $("#y").text(pos.y.toFixed(2));
                            if (item) {
                                if (previousPoint != item.dataIndex) {
                                    previousPoint = item.dataIndex;

                                    $("#tooltip").remove();
                                    var x = item.datapoint[0].toFixed(2),
                                        y = item.datapoint[1].toFixed(2);

                                    showTooltip('24 Jan 2013', item.pageX, item.pageY, x + " = " + y, x, y);
                                }
                            } else {
                                $("#tooltip").remove();
                                previousPoint = null;
                            }
                        });

                    });

                    


                });

                
            }

        }

    </script>
    <script>

        jQuery(document).ready(function () {
            App.init();
            initCharts();
        });

    </script>
</asp:Content>

