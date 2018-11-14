
(function (appControllers) {

    "use strict";
    RouteTableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig','SecurityService'];

    function RouteTableAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "RouteTable";


        function GetFilteredRouteTables(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredRouteTables'), input);
        }
        function AddRouteTable(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddRouteTable'), RouteTableItem);
        }
        function HasAddRouteTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddRouteTable']));
        }

        function UpdateRouteTable(RouteTableItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateRouteTable'), RouteTableItem);
        }
        function HasUpdateRouteTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateRouteTable']));
        }
        function DeleteRouteTable(routeTableId, RouteTableViewType) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "DeleteRouteTable"),
                {
                    routeTableId: routeTableId,
                    RouteTableViewType: RouteTableViewType
                });
        }
        function HasDeleteRouteTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['DeleteRouteTable']));
        }
        function GetRouteTableById(routeTableId, RouteTableViewType) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetRouteTableById"),
                {
                    routeTableId: routeTableId,
                    RouteTableViewType: RouteTableViewType
                });
        }
        function GetCarrierAccountIds(routeTableId, routeTableViewType) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetCarrierAccountIds"),
                {
                    routeTableId: routeTableId,
                    routeTableViewType: routeTableViewType
                });
        }


        return ({
            GetFilteredRouteTables: GetFilteredRouteTables,
            AddRouteTable: AddRouteTable,
            HasAddRouteTablePermission: HasAddRouteTablePermission,
            UpdateRouteTable: UpdateRouteTable,
            HasUpdateRouteTablePermission: HasUpdateRouteTablePermission,
            DeleteRouteTable: DeleteRouteTable,
            HasDeleteRouteTablePermission: HasDeleteRouteTablePermission,
            GetRouteTableById: GetRouteTableById,
            GetCarrierAccountIds: GetCarrierAccountIds
        });
    }

    appControllers.service('NP_IVSwitch_RouteTableAPIService', RouteTableAPIService);

})(appControllers);