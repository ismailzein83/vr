<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="assets/plugins/jqvmap/jqvmap/jqvmap.css" rel="stylesheet" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.css" />
    <link href="assets/plugins/bootstrap-modal/css/bootstrap-modal.css" rel="stylesheet" type="text/css" />
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
                 <a href="ManageSchedules.aspx"><label class="pull-right"><b>&nbsp; Next schedule: </b><asp:Label ID="lblNxtSch" runat="server" /> &nbsp;</label>
                </a>
                <a><label class="pull-right">&nbsp;&nbsp;&nbsp;<b>Month of: </b><asp:Label ID="lblMonth" runat="server" /> &nbsp; -- </label>
                </a>
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
                            <tr>
                                <td>Completed: </td>
                                <td>
                                    <asp:Label ID="lblTotalCalls" runat="server" /></td>
                            </tr>
                            <tr>
                                <td>Remaining: </td>
                                <td>
                                    <asp:Label ID="lblRemaining" runat="server" /></td>
                            </tr>
                            <tr class="visibility: hidden">
                                <td>Failed: </td>
                                <td>
                                    <asp:Label ID="lblFailed" runat="server" /></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="TestCall.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6" data-desktop="span3">
            <div class="dashboard-stat yellow">
                <div class="visual">
                    <i class="icon-bar-chart"></i>
                </div>
                <div class="details">
                    <div class="desc">
                        <table>
                            <tr>
                                <td>Delivered: </td>
                                <td>
                                    <asp:Label ID="lblCLIDel" runat="server" /></td>
                            </tr>
                            <tr>
                                <td>Not Delivered: </td>
                                <td>
                                    <asp:Label ID="lblCLINonDel" runat="server" /></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="ManageTestOperators.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6  fix-offset" data-desktop="span3">
            <div class="dashboard-stat green">
                <div class="visual">
                    <i class="icon-ok-circle"></i>
                </div>
                <div class="details">
                    <div class="desc">
                        <h4>Best Supplier:</h4>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblBest1" runat="server" />
                                    %</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblBest2" runat="server" />
                                    %</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="ManageTestOperators.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
        <div class="span3 responsive" data-tablet="span6" data-desktop="span3">
            <div class="dashboard-stat red">
                <div class="visual">
                    <i class="icon-remove-circle"></i>
                </div>
                <div class="details">
                    <div class="desc">
                        <h4>Worst Supplier:</h4>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblWorst1" runat="server" />
                                    %</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblWorst2" runat="server" />
                                    %</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <a class="more" href="ManageTestOperators.aspx">View more <i class="m-icon-swapright m-icon-white"></i>
                </a>
            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="span6">
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
        <div class="span6">
            <!-- BEGIN PORTLET-->
            <div class="portlet solid bordered light-grey">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-bar-chart"></i>Statistics Total Calls</div>
                </div>
                <div class="portlet-body">
                    <div id="site_statistics2_loading">
                        <img src="assets/img/loading.gif" alt="loading" />
                    </div>
                    <div id="site_statistics2_content" class="hide">
                        <div id="site_statistics2" class="chart"></div>
                    </div>
                </div>
            </div>
            <!-- END PORTLET-->
        </div>
    </div>



    <div class="row-fluid" id="RegionalFeeds" runat="server">
        <div class="span6">
            <!-- BEGIN REGIONAL STATS PORTLET-->
            <div class="portlet">
                <div class="portlet-title">
                    <div class="caption"><i class="icon-globe"></i>Regional Stats</div>
                </div>
                <div class="portlet-body">
                    <div id="region_statistics_loading">
                        <img src="assets/img/loading.gif" alt="loading" />
                    </div>
                    <div id="region_statistics_content" class="hide">
                        <div class="btn-toolbar">
                            <div class="btn-group pull-right">
                            </div>
                        </div>
                        <div id="vmap_world" class="vmaps chart hide"></div>
                    </div>
                </div>
            </div>
            <!-- END REGIONAL STATS PORTLET-->
        </div>
        <div class="span6">
            <!-- BEGIN PORTLET-->
            <div class="portlet paddingless">
                <div class="portlet-title line">
                    <div class="caption"><i class="icon-bell"></i>Feeds</div>
                </div>
                <div class="portlet-body">
                    <!--BEGIN TABS-->
                    <div class="tabbable tabbable-custom">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#tab_1_1" data-toggle="tab">Schedule</a></li>
                            <li><a href="#tab_1_2" data-toggle="tab">Activities</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="tab_1_1">
                                <div class="scroller" style="height: 290px" data-always-visible="1" data-rail-visible="0">
                                    <ul class="feeds">
                                        <asp:Repeater ID="rptSchedules" runat="server">
                                            <ItemTemplate>
                                                <li>
                                                    <div class="col1">
                                                        <div class="cont">
                                                            <div class="cont-col1">
                                                                <div class='label <%# ((int)Eval("ActionType")) == 1 ? "label-success" : ((int)Eval("ActionType")) == 2 ? "label-warning" : ((int)Eval("ActionType")) == 3 ? "label-important" : "label-info"  %> '>
                                                                    <i class='<%# ((int)Eval("ActionType"))  == 1  ? " icon-plus" : ((int)Eval("ActionType"))  == 2  ? " icon-edit" : ((int)Eval("ActionType"))  == 3  ? " icon-remove" :"icon-bell"  %> '></i>
                                                                </div>
                                                            </div>
                                                            <div class="cont-col2">
                                                                <div class="desc">
                                                                    <asp:Label ID="Label7" runat="server" Text='<%# ((int)Eval("ActionType")) == 1 ? CallGeneratorLibrary.Repositories.ScheduleRepository.GetName((int)Eval("ObjectId")).ToString() +  " - New schedule added by: " + Eval("Name") : ((int)Eval("ActionType")) == 2 ? CallGeneratorLibrary.Repositories.ScheduleRepository.GetName((int)Eval("ObjectId")).ToString() +  " - Schedule modified by: " + Eval("Name") : ((int)Eval("ActionType")) == 3 ? "Schedule deleted by: " + Eval("Name") :    ((int)Eval("ActionType")) == 7 ? CallGeneratorLibrary.Repositories.ScheduleRepository.GetName((int)Eval("ObjectId")).ToString() + " - Schedule Failed"  :  CallGeneratorLibrary.Repositories.ScheduleRepository.GetName((int)Eval("ObjectId")).ToString() + " - Schedule done" %>  '></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col3">
                                                        <div class="date">
                                                            <asp:Label ID="Label1" runat="server" Text='<%#  ((DateTime)Eval("LogDate")).ToString("dd-MM-yyyy - HH:mm") %>  '></asp:Label>
                                                        </div>
                                                    </div>
                                                </li>

                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                            <div class="tab-pane" id="tab_1_2">
                                <div class="scroller" style="height: 290px" data-always-visible="1" data-rail-visible1="1">
                                    <ul class="feeds">
                                        <asp:Repeater ID="rptUsers" runat="server">
                                            <ItemTemplate>
                                                <li>
                                                    <div class="col1">
                                                        <div class="cont">
                                                            <div class="cont-col1">
                                                                <div class='label <%# ((int)Eval("ActionType"))  == 1  ? "label-success" : ((int)Eval("ActionType")) == 2 ? "label-warning" :  ((int)Eval("ActionType")) == 3 ? "label-important" : ((int)Eval("ActionType")) == 4 ? "label-info" : "label-default"  %> '>
                                                                    <i class='<%# ((int)Eval("ActionType"))  == 1  ? " icon-plus" : ((int)Eval("ActionType"))  == 2  ? " icon-edit" : ((int)Eval("ActionType"))  == 3  ? " icon-remove"  : "icon-user"  %> '></i>
                                                                </div>
                                                            </div>
                                                            <div class="cont-col2">
                                                                <div class="desc">
                                                                    <asp:Label ID="Label7" runat="server" Text='<%# ((int)Eval("ActionType")) == 4 ? "Login: " + Eval("Name") + " - " + Eval("IPAddress") : ((int)Eval("ActionType")) == 5 ? "Logout: " + Eval("Name")  + " - " + Eval("IPAddress") : ((int)Eval("ActionType")) == 1 ? CallGeneratorLibrary.Repositories.UserRepository.GetName((int)Eval("ObjectId")).ToString() +  " - New user added by: " + Eval("Name")  + " - " + Eval("IPAddress") : ((int)Eval("ActionType")) == 2 ? CallGeneratorLibrary.Repositories.UserRepository.GetName((int)Eval("ObjectId")).ToString() +  " - User modified by: " + Eval("Name")  + " - " + Eval("IPAddress") : ((int)Eval("ActionType")) == 3 ? "User deleted by: " + Eval("Name")  + " - " + Eval("IPAddress") : "" %>  '></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col3">
                                                        <div class="date">
                                                            <asp:Label ID="Label1" runat="server" Text='<%#  ((DateTime)Eval("LogDate")).ToString("dd-MM-yyyy - HH:mm") %>  '></asp:Label>
                                                        </div>
                                                    </div>
                                                </li>

                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!--END TABS-->
                </div>
            </div>
            <!-- END PORTLET-->
        </div>
    </div>
    <div class="clearfix"></div>

    <div id="ajax-modal" class="modal hide fade" tabindex="-1">

        <div class="modal-body">
            <table class="table table-striped table-bordered table-advance table-hover" id="myTable">
                <thead>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <div class="modal-footer">
            <button type="button" data-dismiss="modal" class="btn green">OK</button>
        </div>

    </div>


    <!-- END PAGE CONTENT-->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script src="assets/plugins/flot/jquery.flot.js" type="text/javascript"></script>
    <script src="assets/plugins/flot/jquery.flot.resize.js" type="text/javascript"></script>
    <script src="assets/plugins/jquery.pulsate.min.js" type="text/javascript"></script>
    <script src="assets/scripts/app.js"></script>
    <script src="assets/plugins/jqvmap/jqvmap/jquery.vmap.js" type="text/javascript"></script>
    <script src="assets/plugins/jqvmap/jqvmap/maps/jquery.vmap.world.js" type="text/javascript"></script>
    <script src="assets/myScript/sampleDataVmap.js"></script>

    <!-- BEGIN PAGE LEVEL PLUGINS -->
    <script src="assets/plugins/bootstrap-modal/js/bootstrap-modal.js" type="text/javascript"></script>
    <script src="assets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js" type="text/javascript"></script>
    <!-- END PAGE LEVEL PLUGINS -->

    <script>

        var UIModals = function () {
            $.fn.modalmanager.defaults.resize = true;
            $.fn.modalmanager.defaults.spinner = '<div class="loading-spinner fade" style="width: 200px; margin-left: -100px;"><img src="assets/img/ajax-modal-loading.gif" align="middle">&nbsp;<span style="font-weight:300; color: #eee; font-size: 18px; font-family:Open Sans;">&nbsp;Loading...</span></div>';

        };

    </script>

    <script>

        function showTooltip( x, y,  yy, id) {
            $('<div id="' + id + '" class="chart-tooltip"><div class="date"><\/div><div class="label label-info">' + yy + '<\/div><\/div>').css({
                position: 'absolute',
                display: 'none',
                top: y - 50,
                left: x - 40,
                border: '0px solid #ccc',
                padding: '2px 6px',
                'background-color': '#fff',
            }).appendTo("body").fadeIn(200);
        }

        function getArray(msg) {
            var arr = Object.keys(msg).map(function (k) {
                return [msg[k].ChartId, msg[k].Total];
            });
            return arr;
        }

        function loadChart(id, urlHandler) {
            if ($('#' + id).size() != 0) {

                var request = $.ajax({
                    url: urlHandler,
                    contentType: "application/json; charset=utf-8",
                    type: 'POST',
                    cache: false,
                    async: false
                });

                request.done(function (msg) {

                    var graphs = [];
                    var i = 0;

                    $('#' + id + '_loading').hide();
                    $('#' + id + '_content').show();

                    for (var i = 0; i < msg.length; i++) {
                        arrData = msg[i];
                        var lbl = '';
                        if (arrData.length > 0)
                            lbl = arrData[0].label;

                        graphs.push({
                            data: getArray(arrData),
                            label: lbl
                        });
                    }


                    var plot_statistics = $.plot($('#' + id), graphs, {
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
                    var tooltipId = 'tooltip' + id;

                    $('#' + id).bind("plothover", function (event, pos, item) {
                        
                        if (item) {
                            if (previousPoint != item.dataIndex) {
                                previousPoint = item.dataIndex;

                                $('#' + tooltipId).remove();
                                var x = item.datapoint[0].toFixed(2),
                                    y = item.datapoint[1].toFixed(2);

                                showTooltip( item.pageX, item.pageY, y, tooltipId);
                            }
                        } else {
                            $('#' + tooltipId).remove();
                            previousPoint = null;
                        }
                    });

                });

            }
        }

    </script>

    <script>

        var initCharts = function () {

            if (!jQuery.plot) {
                return;
            }

            loadChart("site_statistics", "HandlerGetChartCalls.ashx?status=2");
            loadChart("site_statistics2", "HandlerGetChartUserCalls.ashx");

        }

    </script>

    <script>

        var myCode = ["au", "bh", "by", "be", "bg", "ci", "cy", "dk", "eg", "fj", "fr", "ga", "gm", "de", "gh", "gn", "ir", "iq", "jo", "kw", "lb", "mw", "my", "ml", "ma", "nl", "ng", "om", "il", "qa", "ru", "sa", "sn", "sg", "za", "lk", "ch", "sy", "tr", "ae", "gb", "ye"];

        function include(arr, obj) {
            return (arr.indexOf(obj) != -1);
        }

        var Index = function () {

            return {

                //main function
                init: function () {
                    App.addResponsiveHandler(function () {
                        jQuery('.vmaps').each(function () {
                            var map = jQuery(this);
                            map.width(map.parent().width());
                        });

                    });

                },

                initJQVMAP: function () {

                    var showMap = function (name) {
                        jQuery('.vmaps').hide();
                        jQuery('#vmap_' + name).show();
                    }

                    var setMap = function (name) {
                        var data = {
                            map: 'world_en',
                            backgroundColor: null,
                            borderColor: '#333333',
                            borderOpacity: 0.5,
                            borderWidth: 1,
                            color: '#c6c6c6',
                            enableZoom: true,
                            hoverColor: '#c9dfaf',
                            hoverOpacity: null,
                            values: sample_data,
                            normalizeFunction: 'linear',
                            scaleColors: ['#b6da93', '#909cae'],
                            selectedColor: '#c9dfaf',
                            selectedRegion: null,
                            showTooltip: true,
                            onLabelShow: function (event, label, code) {
                                if (!include(myCode, code)) {
                                    event.preventDefault();
                                }
                            },
                            onRegionOver: function (event, code) {
                                if (!include(myCode, code)) {
                                    event.preventDefault();
                                }
                            },
                            onRegionClick: function (element, code, region) {
                                if (!include(myCode, code)) {
                                    event.preventDefault();
                                }
                                else {

                                    // create the backdrop and wait for next modal to be triggered
                                    $('body').modalmanager('loading');

                                    var request = $.ajax({
                                        url: "HandlerGetCountryMap.ashx",
                                        contentType: "application/json; charset=utf-8",
                                        type: 'POST',
                                        cache: false,
                                        data: JSON.stringify([{ "code": code.toUpperCase() }])
                                    });

                                    request.done(function (msg) {
                                        $('#myTable > tbody > tr').remove();
                                        $('#myTable > thead > tr').remove();

                                        $('#myTable > thead').append('<tr>' +
                                                                    '<td colspan ="4" style="color:black">' + region + '</td>' +
                                                                    '</tr>');


                                        $('#myTable > thead').append('<tr><th><i class="icon-briefcase"></i> Operator</th><th class="hidden-phone"><i class="icon-phone"></i> Delivered</th><th class="hidden-phone"><i class="icon-phone"></i> Not Delivered</th><th class="hidden-phone"><i class="icon-phone"></i> Total</th></tr>');


                                        $.each(msg, function (i, val) {

                                            $('#myTable > tbody').append('<tr>' +
                                                                        '<td>' + val.Operator + '</td>' +
                                                                        '<td>' + val.Delivered + ' %</td>' +
                                                                        '<td>' + val.NotDelivered + ' %</td>' +
                                                                        '<td>' + val.Total + '</td>' +
                                                                        '</tr>');
                                        });

                                        $('#ajax-modal').modal('show');
                                    });

                                }
                            }
                        };

                        data.map = name + '_en';
                        var map = jQuery('#vmap_' + name);
                        if (!map) {
                            return;
                        }
                        map.width(map.parent().parent().width());
                        map.show();
                        map.vectorMap(data);
                        map.hide();
                    }


                    jQuery('.vmaps').hide();
                    jQuery('#vmap_' + name).show();

                    setMap("world");
                    showMap("world");

                    jQuery('#regional_stat_world').click(function () {
                        showMap("world");
                    });

                    $('#region_statistics_loading').hide();
                    $('#region_statistics_content').show();

                    $('#region_statistics2_loading').hide();
                    $('#region_statistics2_content').show();

                },

            };

        }();

    </script>

    <script>

        jQuery(document).ready(function () {
            App.init();
            initCharts();
            Index.init();
            Index.initJQVMAP(); // init index page's custom scripts
            UIModals();
        });

    </script>
</asp:Content>

