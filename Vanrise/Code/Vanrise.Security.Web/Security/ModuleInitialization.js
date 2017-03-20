app.run(['VR_Sec_UserService', 'VR_Sec_GroupService', function (VR_Sec_UserService, VR_Sec_GroupService) {
    VR_Sec_UserService.registerObjectTrackingDrillDownToUser();
    VR_Sec_GroupService.registerObjectTrackingDrillDownToGroup();
}]);