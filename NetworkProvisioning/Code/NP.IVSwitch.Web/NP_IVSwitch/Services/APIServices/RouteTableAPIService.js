
(function (appControllers) {

    "use strict";
    RouteTableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function RouteTableAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "RouteTable";


        function GetRouteTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTablesInfo"), {
                filter: filter
            });
        }


        return ({
            GetRouteTablesInfo: GetRouteTablesInfo,

        });
    }

    appControllers.service('NP_IVSwitch_RouteTableAPIService', RouteTableAPIService);

})(appControllers);