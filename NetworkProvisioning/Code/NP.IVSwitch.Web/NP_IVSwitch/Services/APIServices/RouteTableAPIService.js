
(function (appControllers) {

    "use strict";
    RouteTableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function RouteTableAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

        var controllerName = "RouteTable";


        function GetFilteredRouteTables(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredRouteTables'), input);
        }

        function AddRouteTable(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddRouteTable'), RouteTableItem);
        }


        return ({
            GetFilteredRouteTables: GetFilteredRouteTables,
            AddRouteTable: AddRouteTable
        });
    }

    appControllers.service('NP_IVSwitch_RouteTableAPIService', RouteTableAPIService);

})(appControllers);