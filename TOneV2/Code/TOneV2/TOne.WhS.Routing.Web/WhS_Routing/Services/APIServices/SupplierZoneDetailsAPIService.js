(function (appControllers) {

    "use strict";

    supplierZoneDetailsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function supplierZoneDetailsAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "SupplierZoneDetails";

        function GetSupplierZoneDetailsByCode(code) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetSupplierZoneDetailsByCode"), {
                code: code
            });
        };

        return ({
            GetSupplierZoneDetailsByCode: GetSupplierZoneDetailsByCode
        });
    }

    appControllers.service('WhS_Routing_SupplierZoneDetailsAPIService', supplierZoneDetailsAPIService);
})(appControllers);