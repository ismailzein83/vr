app.run(['VR_Notification_VRAlertRuleTypeService', 'VR_Notification_VRAlertRuleService', 'VR_Notification_AlertLevelService', function (VR_Notification_VRAlertRuleTypeService,VR_Notification_VRAlertRuleService, VR_Notification_AlertLevelService) {
    VR_Notification_VRAlertRuleService.registerObjectTrackingDrillDownToAlertRule();
    VR_Notification_AlertLevelService.registerObjectTrackingDrillDownToVRAlertLevel();
    VR_Notification_VRAlertRuleTypeService.registerObjectTrackingDrillDownToVRAlertRuleType();
}]);