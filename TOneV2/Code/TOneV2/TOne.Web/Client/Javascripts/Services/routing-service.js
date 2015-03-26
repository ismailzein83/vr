'use strict'
var RoutingService = function (HttpService, MainService) {

    return ({
        getAllRouteRule:getAllRouteRule,
        getRouteRuleDetails: getRouteRuleDetails,
        saveRouteRule: saveRouteRule
    });


    function getAllRouteRule(page, pageSize) {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/GetAllRouteRule";
        return HttpService.get(getRoutingURL,
            {
                pageNumber: page,
                pageSize: pageSize
            });
    }
    function getRouteRuleDetails(RouteRuleId) {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/GetRouteRuleDetails";
        return HttpService.get(getRoutingURL,
            {
                RouteRuleId: RouteRuleId
            });
    }
    function saveRouteRule(routeRule) {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/SaveRouteRule";
        return HttpService.post(getRoutingURL, routeRule);
    }
    
}
RoutingService.$inject = ['HttpService', 'MainService'];
appControllers.service('RoutingService', RoutingService);
