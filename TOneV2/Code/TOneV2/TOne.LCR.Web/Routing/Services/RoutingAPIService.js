
var RoutingAPIService = function (BaseAPIService) {
    'use strict';
    return ({
        GetRoutes: GetRoutes
    });

    function GetRoutes(input) {
        return BaseAPIService.post("/api/Routing/GetRoutes", input);
    }
}
RoutingAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingAPIService', RoutingAPIService);
