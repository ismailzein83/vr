﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Controls_Header" %>
<!-- BEGIN HEADER -->   
<div class="header navbar navbar-inverse navbar-fixed-top">
	<!-- BEGIN TOP NAVIGATION BAR -->
	<div class="navbar-inner">
		<div class="container-fluid">
			<!-- BEGIN LOGO -->
			<a class="brand" href="default.aspx" style="color:white">&nbsp;&nbsp;&nbsp;<i class="icon-home"></i>&nbsp;Vanrise&nbsp;-&nbsp;CLI Tester
			<%--<img src="assets/img/logo.png" alt="logo" />--%>
			</a>
			<!-- END LOGO -->
			<!-- BEGIN RESPONSIVE MENU TOGGLER -->
			<a href="javascript:;" class="btn-navbar collapsed" data-toggle="collapse" data-target=".nav-collapse">
			<img src="assets/img/menu-toggler.png" alt="" />
			</a>          
			<!-- END RESPONSIVE MENU TOGGLER -->            
			<!-- BEGIN TOP NAVIGATION MENU -->              
			<ul class="nav pull-right">
				<!-- BEGIN NOTIFICATION DROPDOWN -->   
		<%--		<li class="dropdown" id="header_notification_bar">
					<a href="#" class="dropdown-toggle" data-toggle="dropdown" data-hover="dropdown" data-close-others="true">
					<i class="icon-warning-sign"></i>
					<span class="badge">6</span>
					</a>
					<ul class="dropdown-menu extended notification">
						<li>
							<p>You have 14 new notifications</p>
						</li>
						<li>
							<ul class="dropdown-menu-list scroller" style="height:250px">
								<li>
									<a href="#">
									<span class="label label-success"><i class="icon-plus"></i></span>
									New user registered. 
									<span class="time">Just now</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-important"><i class="icon-bolt"></i></span>
									Server #12 overloaded. 
									<span class="time">15 mins</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-warning"><i class="icon-bell"></i></span>
									Server #2 not responding.
									<span class="time">22 mins</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-info"><i class="icon-bullhorn"></i></span>
									Application error.
									<span class="time">40 mins</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-important"><i class="icon-bolt"></i></span>
									Database overloaded 68%. 
									<span class="time">2 hrs</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-important"><i class="icon-bolt"></i></span>
									2 user IP blocked.
									<span class="time">5 hrs</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-warning"><i class="icon-bell"></i></span>
									Storage Server #4 not responding.
									<span class="time">45 mins</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-info"><i class="icon-bullhorn"></i></span>
									System Error.
									<span class="time">55 mins</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="label label-important"><i class="icon-bolt"></i></span>
									Database overloaded 68%. 
									<span class="time">2 hrs</span>
									</a>
								</li>
							</ul>
						</li>
						<li class="external">
							<a href="#">See all notifications <i class="m-icon-swapright"></i></a>
						</li>
					</ul>
				</li>--%>
				<!-- END NOTIFICATION DROPDOWN -->
				<!-- BEGIN INBOX DROPDOWN -->
			<%--	<li class="dropdown" id="header_inbox_bar">
					<a href="#" class="dropdown-toggle" data-toggle="dropdown" data-hover="dropdown" data-close-others="true">
					<i class="icon-envelope"></i>
					<span class="badge">5</span>
					</a>
					<ul class="dropdown-menu extended inbox">
						<li>
							<p>You have 12 new messages</p>
						</li>
						<li>
							<ul class="dropdown-menu-list scroller" style="height:250px">
								<li>
									<a href="inbox.html?a=view">
									<span class="photo"><img src="./assets/img/avatar2.jpg" alt="" /></span>
									<span class="subject">
									<span class="from">Lisa Wong</span>
									<span class="time">Just Now</span>
									</span>
									<span class="message">
									Vivamus sed auctor nibh congue nibh. auctor nibh
									auctor nibh...
									</span>  
									</a>
								</li>
								<li>
									<a href="inbox.html?a=view">
									<span class="photo"><img src="./assets/img/avatar3.jpg" alt="" /></span>
									<span class="subject">
									<span class="from">Richard Doe</span>
									<span class="time">16 mins</span>
									</span>
									<span class="message">
									Vivamus sed congue nibh auctor nibh congue nibh. auctor nibh
									auctor nibh...
									</span>  
									</a>
								</li>
								<li>
									<a href="inbox.html?a=view">
									<span class="photo"><img src="./assets/img/avatar1.jpg" alt="" /></span>
									<span class="subject">
									<span class="from">Bob Nilson</span>
									<span class="time">2 hrs</span>
									</span>
									<span class="message">
									Vivamus sed nibh auctor nibh congue nibh. auctor nibh
									auctor nibh...
									</span>  
									</a>
								</li>
								<li>
									<a href="inbox.html?a=view">
									<span class="photo"><img src="./assets/img/avatar2.jpg" alt="" /></span>
									<span class="subject">
									<span class="from">Lisa Wong</span>
									<span class="time">40 mins</span>
									</span>
									<span class="message">
									Vivamus sed auctor 40% nibh congue nibh...
									</span>  
									</a>
								</li>
								<li>
									<a href="inbox.html?a=view">
									<span class="photo"><img src="./assets/img/avatar3.jpg" alt="" /></span>
									<span class="subject">
									<span class="from">Richard Doe</span>
									<span class="time">46 mins</span>
									</span>
									<span class="message">
									Vivamus sed congue nibh auctor nibh congue nibh. auctor nibh
									auctor nibh...
									</span>  
									</a>
								</li>
							</ul>
						</li>
						<li class="external">
							<a href="inbox.html">See all messages <i class="m-icon-swapright"></i></a>
						</li>
					</ul>
				</li>--%>
				<!-- END INBOX DROPDOWN -->
				<!-- BEGIN TODO DROPDOWN -->
<%--				<li class="dropdown" id="header_task_bar">
					<a href="#" class="dropdown-toggle" data-toggle="dropdown" data-hover="dropdown" data-close-others="true">
					<i class="icon-tasks"></i>
					<span class="badge">5</span>
					</a>
					<ul class="dropdown-menu extended tasks">
						<li>
							<p>You have 12 pending tasks</p>
						</li>
						<li>
							<ul class="dropdown-menu-list scroller" style="height:250px">
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">New release v1.2</span>
									<span class="percent">30%</span>
									</span>
									<span class="progress progress-success ">
									<span style="width: 30%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">Application deployment</span>
									<span class="percent">65%</span>
									</span>
									<span class="progress progress-danger progress-striped active">
									<span style="width: 65%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">Mobile app release</span>
									<span class="percent">98%</span>
									</span>
									<span class="progress progress-success">
									<span style="width: 98%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">Database migration</span>
									<span class="percent">10%</span>
									</span>
									<span class="progress progress-warning progress-striped">
									<span style="width: 10%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">Web server upgrade</span>
									<span class="percent">58%</span>
									</span>
									<span class="progress progress-info">
									<span style="width: 58%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">Mobile development</span>
									<span class="percent">85%</span>
									</span>
									<span class="progress progress-success">
									<span style="width: 85%;" class="bar"></span>
									</span>
									</a>
								</li>
								<li>
									<a href="#">
									<span class="task">
									<span class="desc">New UI release</span>
									<span class="percent">18%</span>
									</span>
									<span class="progress progress-important">
									<span style="width: 18%;" class="bar"></span>
									</span>
									</a>
								</li>
							</ul>
						</li>
						<li class="external">
							<a href="#">See all tasks <i class="m-icon-swapright"></i></a>
						</li>
					</ul>
				</li>--%>
				<!-- END TODO DROPDOWN -->               
				<!-- BEGIN USER LOGIN DROPDOWN -->
				<li class="dropdown user">
					<a href="#" class="dropdown-toggle" data-toggle="dropdown" data-hover="dropdown" data-close-others="true">
					<%--<img alt="" src="assets/img/avatar1_small.jpg" />--%>
					<span class="username"><span id="spanUser" runat="server"></span></span>
					<i class="icon-angle-down"></i>
					</a>
					<ul class="dropdown-menu">
						<li><a href='<%# CallGeneratorLibrary.Utilities.Config.SiteUrl %>UserProfile.aspx' target="_parent"><i class="icon-user"></i> My Profile</a></li>
<%--						<li><a href="page_calendar.html"><i class="icon-calendar"></i> My Calendar</a></li>
						<li><a href="inbox.html"><i class="icon-envelope"></i> My Inbox <span class="badge badge-important">3</span></a></li>
						<li><a href="#"><i class="icon-tasks"></i> My Tasks <span class="badge badge-success">8</span></a></li>--%>
						<li class="divider"></li>
						<li><a href="javascript:;" id="trigger_fullscreen"><i class="icon-move"></i> Full Screen</a></li>
						<%--<li><a href="extra_lock.html"><i class="icon-lock"></i> Lock Screen</a></li>--%>
						<li><a href='<%# CallGeneratorLibrary.Utilities.Config.SiteUrl %>Logout.aspx' target="_parent"><i class="icon-key"></i> Log Out</a></li>
					</ul>
				</li>
				<!-- END USER LOGIN DROPDOWN -->
				<!-- END USER LOGIN DROPDOWN -->
			</ul>
			<!-- END TOP NAVIGATION MENU --> 
		</div>
	</div>
	<!-- END TOP NAVIGATION BAR -->
</div>
<!-- END HEADER -->