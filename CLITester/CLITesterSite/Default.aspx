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
                            <tr>
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
                <a class="more" href="#">View more <i class="m-icon-swapright m-icon-white"></i>
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
                    <div class="caption"><i class="icon-bar-chart"></i>Statistics demo</div>
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
                            <%--<li><a href="#tab_1_3" data-toggle="tab">Recent Users</a></li>--%>
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
                                        <%--	<li>
															<a href="#">
																<div class="col1">
																	<div class="cont">
																		<div class="cont-col1">
																			<div class="label label-success">                        
																				<i class="icon-bell"></i>
																			</div>
																		</div>
																		<div class="cont-col2">
																			<div class="desc">
																				New version v1.4 just lunched!   
																			</div>
																		</div>
																	</div>
																</div>
																<div class="col2">
																	<div class="date">
																		20 mins
																	</div>
																</div>
															</a>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-important">                      
																			<i class="icon-bolt"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			Database server #12 overloaded. Please fix the issue.                      
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	24 mins
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	30 mins
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-success">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	40 mins
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-warning">                        
																			<i class="icon-plus"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New user registered.                
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	1.5 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-success">                        
																			<i class="icon-bell-alt"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			Web server hardware needs to be upgraded. 
																			<span class="label label-inverse label-mini">Overdue</span>             
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	2 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label">                       
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	3 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-warning">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	5 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	18 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label">                       
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	21 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	22 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label">                       
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	21 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	22 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label">                       
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	21 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	22 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label">                       
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	21 hours
																</div>
															</div>
														</li>
														<li>
															<div class="col1">
																<div class="cont">
																	<div class="cont-col1">
																		<div class="label label-info">                        
																			<i class="icon-bullhorn"></i>
																		</div>
																	</div>
																	<div class="cont-col2">
																		<div class="desc">
																			New order received. Please take care of it.                 
																		</div>
																	</div>
																</div>
															</div>
															<div class="col2">
																<div class="date">
																	22 hours
																</div>
															</div>
														</li>--%>
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
                                        <%--                  <li>
                                            <a href="#">
                                                <div class="col1">
                                                    <div class="cont">
                                                        <div class="cont-col1">
                                                            <div class="label label-success">
                                                                <i class="icon-bell"></i>
                                                            </div>
                                                        </div>
                                                        <div class="cont-col2">
                                                            <div class="desc">
                                                                New order received 
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col2">
                                                    <div class="date">
                                                        10 mins
                                                    </div>
                                                </div>
                                            </a>
                                        </li>--%>
                                    </ul>
                                </div>
                            </div>
                            <%--  <div class="tab-pane" id="tab_1_3">
                                <div class="scroller" style="height: 290px" data-always-visible="1" data-rail-visible1="1">
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Robert Nilson</a>
                                                    <span class="label label-success">Approved</span>
                                                </div>
                                                <div>29 Jan 2013 10:45AM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-info">Pending</span>
                                                </div>
                                                <div>19 Jan 2013 10:45AM</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Eric Kim</a>
                                                    <span class="label label-info">Pending</span>
                                                </div>
                                                <div>19 Jan 2013 12:45PM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-important">In progress</span>
                                                </div>
                                                <div>19 Jan 2013 11:55PM</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Eric Kim</a>
                                                    <span class="label label-info">Pending</span>
                                                </div>
                                                <div>19 Jan 2013 12:45PM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-important">In progress</span>
                                                </div>
                                                <div>19 Jan 2013 11:55PM</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div><a href="#">Eric Kim</a> <span class="label label-info">Pending</span></div>
                                                <div>19 Jan 2013 12:45PM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-important">In progress</span>
                                                </div>
                                                <div>19 Jan 2013 11:55PM</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div><a href="#">Eric Kim</a> <span class="label label-info">Pending</span></div>
                                                <div>19 Jan 2013 12:45PM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-important">In progress</span>
                                                </div>
                                                <div>19 Jan 2013 11:55PM</div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row-fluid">
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Eric Kim</a>
                                                    <span class="label label-info">Pending</span>
                                                </div>
                                                <div>19 Jan 2013 12:45PM</div>
                                            </div>
                                        </div>
                                        <div class="span6 user-info">
                                            <img alt="" src="assets/img/avatar.png" />
                                            <div class="details">
                                                <div>
                                                    <a href="#">Lisa Miller</a>
                                                    <span class="label label-important">In progress</span>
                                                </div>
                                                <div>19 Jan 2013 11:55PM</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                        </div>
                    </div>
                    <!--END TABS-->
                </div>
            </div>
            <!-- END PORTLET-->
        </div>
    </div>
    <div class="clearfix"></div>

    <%--    <div id="myModal" class="modal hide fade" tabindex="-1" data-backdrop="static" data-keyboard="false">
        
    </div>--%>

    <div id="ajax-modal" class="modal hide fade" tabindex="-1">

        <div class="modal-body">
            <table class="table table-striped table-bordered table-advance table-hover" id="myTable">
                <thead>
                </thead>
                <tbody>
                </tbody>
                <%--                                <tfoot>
                                        <tr>
                        <th><i class="icon-briefcase"></i> Prefix</th>
                        <th class="hidden-phone"><i class="icon-phone"></i> Delivered</th>
                        <th class="hidden-phone"><i class="icon-phone"></i> Not Delivered</th>
                        <th class="hidden-phone"><i class="icon-phone"></i> Total</th>
                    </tr>
                </tfoot>--%>
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

        var initCharts = function () {
            if (!jQuery.plot) {
                return;
            }

            function showTooltip(title, x, y, contents, xx, yy) {
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
                            return [msg2[k].ChartId, msg2[k].Total];
                        });


                        var plot_statistics = $.plot($("#site_statistics"), [{
                            data: arr,
                            label: "Not Delivered"
                        }, {
                            data: arr2,
                            label: "Delivered"
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











        ////////////////////
        var initCharts2 = function () {
            if (!jQuery.plot) {
                return;
            }

            var data = [];
            var totalPoints = 250;

            // random data generator for plot charts

            function getRandomData() {
                if (data.length > 0) data = data.slice(1);
                // do a random walk
                while (data.length < totalPoints) {
                    var prev = data.length > 0 ? data[data.length - 1] : 50;
                    var y = prev + Math.random() * 10 - 5;
                    if (y < 0) y = 0;
                    if (y > 100) y = 100;
                    data.push(y);
                }
                // zip the generated y values with the x values
                var res = [];
                for (var i = 0; i < data.length; ++i) res.push([i, data[i]])
                return res;
            }

            function showTooltip(title, x, y, contents) {
                $('<div id="tooltip2" class="chart-tooltip"><div class="date">' + title + '<\/div><div class="label label-success">CTR: ' + x / 10 + '%<\/div><div class="label label-important">Imp: ' + x * 12 + '<\/div><\/div>').css({
                    position: 'absolute',
                    display: 'none',
                    top: y - 100,
                    width: 75,
                    left: x - 40,
                    border: '0px solid #ccc',
                    padding: '2px 6px',
                    'background-color': '#fff',
                }).appendTo("body").fadeIn(200);
            }

            function randValue() {
                return (Math.floor(Math.random() * (1 + 50 - 20))) + 10;
            }

            var pageviews = [
                [1, randValue()],
                [2, randValue()],
                [3, 2 + randValue()],
                [4, 3 + randValue()],
                [5, 5 + randValue()],
                [6, 10 + randValue()],
                [7, 15 + randValue()],
                [8, 20 + randValue()],
                [9, 25 + randValue()],
                [10, 30 + randValue()],
                [11, 35 + randValue()],
                [12, 25 + randValue()],
                [13, 15 + randValue()],
                [14, 20 + randValue()],
                [15, 45 + randValue()],
                [16, 50 + randValue()],
                [17, 65 + randValue()],
                [18, 70 + randValue()],
                [19, 85 + randValue()],
                [20, 80 + randValue()],
                [21, 75 + randValue()],
                [22, 80 + randValue()],
                [23, 75 + randValue()],
                [24, 70 + randValue()],
                [25, 65 + randValue()],
                [26, 75 + randValue()],
                [27, 80 + randValue()],
                [28, 85 + randValue()],
                [29, 90 + randValue()],
                [30, 95 + randValue()]
            ];

            var visitors = [
                [1, randValue() - 5],
                [2, randValue() - 5],
                [3, randValue() - 5],
                [4, 6 + randValue()],
                [5, 5 + randValue()],
                [6, 20 + randValue()],
                [7, 25 + randValue()],
                [8, 36 + randValue()],
                [9, 26 + randValue()],
                [10, 38 + randValue()],
                [11, 39 + randValue()],
                [12, 50 + randValue()],
                [13, 51 + randValue()],
                [14, 12 + randValue()],
                [15, 13 + randValue()],
                [16, 14 + randValue()],
                [17, 15 + randValue()],
                [18, 15 + randValue()],
                [19, 16 + randValue()],
                [20, 17 + randValue()],
                [21, 18 + randValue()],
                [22, 19 + randValue()],
                [23, 20 + randValue()],
                [24, 21 + randValue()],
                [25, 14 + randValue()],
                [26, 24 + randValue()],
                [27, 25 + randValue()],
                [28, 26 + randValue()],
                [29, 27 + randValue()],
                [30, 31 + randValue()]
            ];

            if ($('#site_statistics2').size() != 0) {

                $('#site_statistics2_loading').hide();
                $('#site_statistics2_content').show();

                var plot_statistics = $.plot($("#site_statistics2"), [{
                    data: pageviews,
                    label: "Label 1"
                }, {
                    data: visitors,
                    label: "Label 2"
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
                    colors: ["#d12610", "#37b7f3", "#52e136"],
                    xaxis: {
                        ticks: 11,
                        tickDecimals: 0
                    },
                    yaxis: {
                        ticks: 11,
                        tickDecimals: 0
                    }
                });

                var previousPoint = null;
                $("#site_statistics2").bind("plothover", function (event, pos, item) {
                    $("#x").text(pos.x.toFixed(2));
                    $("#y").text(pos.y.toFixed(2));
                    if (item) {
                        if (previousPoint != item.dataIndex) {
                            previousPoint = item.dataIndex;

                            $("#tooltip2").remove();
                            var x = item.datapoint[0].toFixed(2),
                                y = item.datapoint[1].toFixed(2);

                            showTooltip('24 Jan 2013', item.pageX, item.pageY, item.series.label + " of " + x + " = " + y);
                        }
                    } else {
                        $("#tooltip2").remove();
                        previousPoint = null;
                    }
                });
            }               

            if ($('#load_statistics2').size() != 0) {
                //server load
                $('#load_statistics2_loading').hide();
                $('#load_statistics2_content').show();
        
                var updateInterval = 30;
                var plot_statistics = $.plot($("#load_statistics2"), [getRandomData()], {
                    series: {
                        shadowSize: 1
                    },
                    lines: {
                        show: true,
                        lineWidth: 0.2,
                        fill: true,
                        fillColor: {
                            colors: [{
                                opacity: 0.1
                            }, {
                                opacity: 1
                            }
                            ]
                        }
                    },
                    yaxis: {
                        min: 0,
                        max: 100,
                        tickFormatter: function (v) {
                            return v + "%";
                        }
                    },
                    xaxis: {
                        show: false
                    },
                    colors: ["#e14e3d"],
                    grid: {
                        tickColor: "#a8a3a3",
                        borderWidth: 0
                    }
                });
                
                function statisticsUpdate() {
                    plot_statistics.setData([getRandomData()]);
                    plot_statistics.draw();
                    setTimeout(statisticsUpdate, updateInterval);
                
                }
                
                statisticsUpdate();

                $('#load_statistics2').bind("mouseleave", function () {
                    $("#tooltip2").remove();
                });
            }

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

                },

            };

        }();

    </script>

    <script>

        jQuery(document).ready(function () {
            App.init();
            initCharts();
            initCharts2();
            Index.init();
            Index.initJQVMAP(); // init index page's custom scripts
            UIModals();
        });

    </script>
</asp:Content>

