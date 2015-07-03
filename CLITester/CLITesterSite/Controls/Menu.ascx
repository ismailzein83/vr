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
        <li>
        </li>
        <li class="start"><a href="default.aspx"><i class="icon-home"></i><span class="title">
            Dashboard</span></a></li>
        <li><a href="javascript:;"><i class="icon-table"></i><span class="title">Management</span>
            <span class="arrow "></span></a>
            <ul class="sub-menu">
                <li runat="server" id="liManual"><a href="ManualTestCall.aspx">Manual Call</a> </li>
                <li runat="server" id="liDirect"><a href="TestCall.aspx">Direct Call</a> </li>
                <li runat="server" id="liCarriers"><a href="ManageCarriers.aspx">Carriers</a> </li>
                <li runat="server" id="liSchedule"><a href="ManageSchedules.aspx">Schedules</a> </li>
                <li runat="server" id="liUsers"><a href="ManageUsers.aspx">Users</a> </li>
                <li><a href="ManageTestOperators.aspx">History Test Calls</a> </li>
                <li runat="server" id="liSettings"><a href="ManageSettings.aspx">Settings</a> </li>
                <li runat="server" id="liContracts"><a href="ManageContracts.aspx">Contracts</a> </li>
                <li runat="server" id="liBalances"><a href="ManageBalances.aspx">Balances</a> </li>
                <li runat="server" id="licalls" ><a href="ManageTestCalls.aspx">Data Calls</a> </li>
                <%--<li runat="server" id="liLstOnline" ><a href="ListOnlineOperators.aspx">List Online Operators</a> </li>--%>
                <li runat="server" id="liPhoneNumbers" ><a href="ManagePhoneNumbers.aspx">Phone Numbers</a> </li>
                <li runat="server" id="liOperators" ><a href="ManageOperators.aspx">Operators</a> </li>
                <li runat="server" id="liRequestCalls" ><a href="ManageRequestCalls.aspx">Request Calls</a> </li>
            </ul>
        </li>
    </ul>
    <!-- END SIDEBAR MENU -->
</div>
<!-- END SIDEBAR -->
