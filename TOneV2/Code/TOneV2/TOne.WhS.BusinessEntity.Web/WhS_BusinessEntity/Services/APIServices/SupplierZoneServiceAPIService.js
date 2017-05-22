(function (appControllers) {

    "use strict";
    supplierZoneServiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function supplierZoneServiceAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SupplierZoneService";

        function GetFilteredSupplierZoneServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierZoneServices"), input);
        }
        function UpdateSupplierZoneService(serviceObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSupplierZoneService"), serviceObject);
        }
        return ({
            GetFilteredSupplierZoneServices: GetFilteredSupplierZoneServices,
            UpdateSupplierZoneService: UpdateSupplierZoneService
        });
    }

    appControllers.service("WhS_BE_SupplierZoneServiceAPIService", supplierZoneServiceAPIService);
})(appControllers);