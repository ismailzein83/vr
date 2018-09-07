
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

        function GetRouteTableById(routeTableId, RouteTableViewType) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTableById"),
                {
                    routeTableId: routeTableId,
                    RouteTableViewType: RouteTableViewType
                });
        }

        function DeleteRouteTable(routeTableId, RouteTableViewType) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "DeleteRouteTable"),
                {
                    routeTableId: routeTableId,
                    RouteTableViewType: RouteTableViewType
                });
        }

        return ({
            GetFilteredRouteTables: GetFilteredRouteTables,
            AddRouteTable: AddRouteTable,
            GetRouteTableById: GetRouteTableById,
            UpdateRouteTable: UpdateRouteTable,
            DeleteRouteTable: DeleteRouteTable
        });
    }

    appControllers.service('NP_IVSwitch_RouteTableAPIService', RouteTableAPIService);

})(appControllers);