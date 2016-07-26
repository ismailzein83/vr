<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="Controls_Menu" %>
<!-- BEGIN SIDEBAR -->
<div class="page-sidebar nav-collapse collapse">
    <!-- BEGIN SIDEBAR MENU -->
    <ul class="page-sidebar-menu">
        <li>
            <!-- BEGIN SIDEBAR TOGGLER BUTTON -->
            <div class="sidebar-toggler hidden-phone">
            </div>
            <!-- BEGIN SIDEBAR TOGGLER BUTTON -->
        </li>
        <li class="start"><a href="default.aspx"><i class="icon-home"></i><span class="title">
            Dashboard</span></a></li>
        <li><a href="javascript:;"><i class="icon-table"></i><span class="title">Management</span>
            <span class="arrow "></span></a>
            <ul class="sub-menu">
                <li><a href="ManageSchedules.aspx">Schedules</a> </li>
                <li><a href="CDR.aspx">CDR</a> </li>
                <li><a href="ManageTestNumberGroups.aspx">Groups</a> </li>
            </ul>
        </li>
    </ul>
    <!-- END SIDEBAR MENU -->
</div>
<!-- END SIDEBAR -->
