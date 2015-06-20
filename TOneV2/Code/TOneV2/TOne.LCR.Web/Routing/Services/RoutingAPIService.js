'use strict'
var RoutingAPIService = function (BaseAPIService) {

    return ({
        GetRoutes: GetRoutes
    });

    function GetRoutes(input) {
        return BaseAPIService.post("/api/Routing/GetRoutes", input);
    }

}
RoutingAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingAPIService', RoutingAPIService);
