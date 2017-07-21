app.run(['NP_IVSwitch_EndPointService', 'NP_IVSwitch_FirewallService', 'NP_IVSwitch_TranslationRuleService', 'NP_IVSwitch_CodecProfileService', 'NP_IVSwitch_RouteService', 'NP_IVSwitch_EndPointService',
function (NP_IVSwitch_EndPointService, NP_IVSwitch_FirewallService, NP_IVSwitch_TranslationRuleService, NP_IVSwitch_CodecProfileService, NP_IVSwitch_RouteService, NP_IVSwitch_EndPointService) {
    NP_IVSwitch_EndPointService.registerDrillDownToCarrierAccount();
    NP_IVSwitch_FirewallService.registerObjectTrackingDrillDownToFirewall();
    NP_IVSwitch_FirewallService.registerHistoryViewAction();
    NP_IVSwitch_TranslationRuleService.registerHistoryViewAction();
    NP_IVSwitch_TranslationRuleService.registerObjectTrackingDrillDownToTranslationRule();
    NP_IVSwitch_CodecProfileService.registerHistoryViewAction();
    NP_IVSwitch_CodecProfileService.registerObjectTrackingDrillDownToCodecProfile();
    NP_IVSwitch_RouteService.registerObjectTrackingDrillDownToRoute();
    NP_IVSwitch_RouteService.registerHistoryViewAction();
    NP_IVSwitch_EndPointService.registerObjectTrackingDrillDownToEndPoint();
    NP_IVSwitch_EndPointService.registerHistoryViewAction();
}]);

app.run(['NP_IVSwitch_RouteService',
function (NP_IVSwitch_RouteService) {
    NP_IVSwitch_RouteService.registerDrillDownToCarrierAccount();
}]);

