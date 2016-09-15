(function (appControllers) {

    "use strict";
    supplierZoneServiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function supplierZoneServiceAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SupplierZoneService";

        function GetFilteredSupplierZoneServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierZoneServices"), input);
        }

        return ({
            GetFilteredSupplierZoneServices: GetFilteredSupplierZoneServices
        });
    }

    appControllers.service("WhS_BE_SupplierZoneServiceAPIService", supplierZoneServiceAPIService);
})(appControllers);