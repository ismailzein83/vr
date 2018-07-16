app.run(['VR_Sec_OrgChartService', 'VR_Sec_UserService', 'VR_Sec_ViewService', 'VR_Sec_GroupService', 'VR_Sec_SecurityProviderStatusService', function (VR_Sec_OrgChartService, VR_Sec_UserService, VR_Sec_ViewService, VR_Sec_GroupService, VR_Sec_SecurityProviderStatusService) {
    VR_Sec_UserService.registerObjectTrackingDrillDownToUser();
    VR_Sec_GroupService.registerObjectTrackingDrillDownToGroup();
    VR_Sec_ViewService.registerObjectTrackingDrillDownToView();
    VR_Sec_OrgChartService.registerObjectTrackingDrillDownToOrgChart();
    VR_Sec_UserService.registerHistoryViewAction();
    VR_Sec_GroupService.registerHistoryViewAction();
    VR_Sec_SecurityProviderStatusService.changeSecurityProviderStatus();
}]);