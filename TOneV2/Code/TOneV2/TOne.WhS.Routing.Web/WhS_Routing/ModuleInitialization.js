app.run(['WhS_Routing_RouteRuleService','WhS_Routing_RouteOptionRuleService',
function (WhS_Routing_RouteRuleService, WhS_Routing_RouteOptionRuleService) {
    WhS_Routing_RouteRuleService.registerObjectTrackingDrillDownToRouteRules();
    WhS_Routing_RouteOptionRuleService.registerObjectTrackingDrillDownToRouteOptionRules();
}]);