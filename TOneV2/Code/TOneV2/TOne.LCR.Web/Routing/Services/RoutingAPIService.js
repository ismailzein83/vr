'use strict'
var RoutingAPIService = function (BaseAPIService) {

    return ({
        GetRoutes: GetFilteredRoutes
    });

    function GetFilteredRoutes(filter) {
        return BaseAPIService.post("/api/routing/GetRoutes", filter);
    }

}
RoutingAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingAPIService', RoutingAPIService);
