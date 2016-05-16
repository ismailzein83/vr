(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SupplierZone";

        function GetFilteredSupplierZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierZones"), input);
        }

        function GetSupplierZoneInfo(serializedFilter, searchValue) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZoneInfo"), {
                serializedFilter: serializedFilter,
                searchValue: searchValue
            });

        }

        function GetSupplierZoneInfoByIds(serializedObj) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZoneInfoByIds"),
                {
                    serializedObj: serializedObj
                });
        }
        function GetSupplierZoneGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZoneGroupTemplates"));
        }
        return ({
            GetFilteredSupplierZones: GetFilteredSupplierZones,
            GetSupplierZoneInfo: GetSupplierZoneInfo,
            GetSupplierZoneInfoByIds: GetSupplierZoneInfoByIds,
            GetSupplierZoneGroupTemplates: GetSupplierZoneGroupTemplates
        });
    }

    appControllers.service("WhS_BE_SupplierZoneAPIService", supplierZoneAPIService);
})(appControllers);