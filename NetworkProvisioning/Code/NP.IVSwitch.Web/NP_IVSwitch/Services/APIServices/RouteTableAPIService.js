
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

        function UpdateRouteTable(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateRouteTable'), RouteTableItem);
        }

        function GetRouteTableById(routeTableId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTableById"),
                {
                    routeTableId: routeTableId
                });
        }

        return ({
            GetFilteredRouteTables: GetFilteredRouteTables,
            AddRouteTable: AddRouteTable,
            GetRouteTableById: GetRouteTableById,
            UpdateRouteTable: UpdateRouteTable
        });
    }

    appControllers.service('NP_IVSwitch_RouteTableAPIService', RouteTableAPIService);

})(appControllers);