(function (appControllers) {

    "use strict";
    supplierZoneServiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function supplierZoneServiceAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "SupplierZoneService";

        function GetFilteredSupplierZoneServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierZoneServices"), input);
        }
        function UpdateSupplierZoneService(serviceObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSupplierZoneService"), serviceObject);
        }
        function HasUpdateSupplierZoneServicePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSupplierZoneService']));
        }

        return ({
            GetFilteredSupplierZoneServices: GetFilteredSupplierZoneServices,
            UpdateSupplierZoneService: UpdateSupplierZoneService,
            HasUpdateSupplierZoneServicePermission: HasUpdateSupplierZoneServicePermission
        });
    }

    appControllers.service("WhS_BE_SupplierZoneServiceAPIService", supplierZoneServiceAPIService);
})(appControllers);