'use strict'
var RoutingService = function (HttpService, MainService) {

    return ({
        getAllRouteRule:getAllRouteRule,
        getRouteRuleDetails: getRouteRuleDetails,
        saveRouteRule: saveRouteRule
    });


    function getAllRouteRule() {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/GetAllRouteRule";
        return HttpService.get(getRoutingURL);
    }
    function getRouteRuleDetails(RouteRuleId) {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/GetRouteRuleDetails";
        return HttpService.get(getRoutingURL, RouteRuleId);
    }
    function saveRouteRule(routeRule) {

        var getRoutingURL = MainService.getBaseURL() + "/api/routing/SaveRouteRule";
        return HttpService.get(getRoutingURL, routeRule);
    }
}
RoutingService.$inject = ['HttpService', 'MainService'];
app.service('RoutingService', RoutingService);
