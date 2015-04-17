'use strict'
var RoutingAPIService = function (BaseAPIService) {

    return ({
        getAllRouteRule:getAllRouteRule,
        getRouteRuleDetails: getRouteRuleDetails,
        saveRouteRule: saveRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules,
    });
    function getAllRouteRule(page, pageSize) {      
        return BaseAPIService.get("/api/routing/GetAllRouteRule",
            {
                pageNumber: page,
                pageSize: pageSize
            });
    }
    function getRouteRuleDetails(RouteRuleId) {
        return BaseAPIService.get("/api/routing/GetRouteRuleDetails",
            {
                RouteRuleId: RouteRuleId
            });
    }
    function GetFilteredRouteRules(filter) {
        return BaseAPIService.post("/api/routing/GetFilteredRouteRules", filter);
    }
    function saveRouteRule(routeRule) {

        return BaseAPIService.post("/api/routing/SaveRouteRule", routeRule);
    }
    
}
RoutingService.$inject = ['BaseAPIService'];
appControllers.service('RoutingAPIService', RoutingAPIService);
