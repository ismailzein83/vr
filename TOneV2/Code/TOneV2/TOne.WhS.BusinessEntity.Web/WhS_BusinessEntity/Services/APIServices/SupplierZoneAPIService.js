(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SupplierZone";

        function GetFilteredSupplierZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierZones"), input);
        }

        function GetSupplierZoneInfo(searchValue, supplierId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZoneInfo"), {
                serializedFilter: serializedFilter,
                supplierId :supplierId,
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

        function GetDistinctSupplierIdsBySupplierZoneIds(supplierZoneIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetDistinctSupplierIdsBySupplierZoneIds"), supplierZoneIds);
        }

        function GetSupplierZonesInfo(supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZonesInfo"), {
                SupplierId: supplierId
            });
        }
        function GetSupplierZoneByCode(supplierId, codeNumber)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierZoneByCode"), {
                supplierId: supplierId,
                codeNumber: codeNumber
            });
        }

        return ({
            GetFilteredSupplierZones: GetFilteredSupplierZones,
            GetSupplierZoneInfo: GetSupplierZoneInfo,
            GetSupplierZoneInfoByIds: GetSupplierZoneInfoByIds,
            GetSupplierZoneGroupTemplates: GetSupplierZoneGroupTemplates,
            GetDistinctSupplierIdsBySupplierZoneIds: GetDistinctSupplierIdsBySupplierZoneIds,
            GetSupplierZonesInfo: GetSupplierZonesInfo,
            GetSupplierZoneByCode: GetSupplierZoneByCode
        });
    }

    appControllers.service("WhS_BE_SupplierZoneAPIService", supplierZoneAPIService);
})(appControllers);