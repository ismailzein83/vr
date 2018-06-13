app.run(['WhS_Routing_RouteRuleService', 'WhS_Routing_RouteOptionRuleService', 'WhS_Routing_CustomerRoutesService',
	function (WhS_Routing_RouteRuleService, WhS_Routing_RouteOptionRuleService, WhS_Routing_CustomerRoutesService) {
		WhS_Routing_RouteRuleService.registerObjectTrackingDrillDownToRouteRules();
		WhS_Routing_RouteOptionRuleService.registerObjectTrackingDrillDownToRouteOptionRules();
		WhS_Routing_RouteRuleService.registerHistoryViewAction();
		WhS_Routing_RouteOptionRuleService.registerHistoryViewAction();
		WhS_Routing_CustomerRoutesService.registerOpenCustomerRoutes();
	}]);