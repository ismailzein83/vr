(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSupplierZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetFilteredSupplierZones"), input);
        }

        function GetSupplierZoneInfo(serializedFilter, searchValue) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetSupplierZoneInfo"), {
                serializedFilter: serializedFilter,
                searchValue: searchValue
            });

        }
        function GetSupplierZoneInfoByIds(serializedObj) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetSupplierZoneInfoByIds"), { serializedObj: serializedObj });
        
        }


        return ({
            GetFilteredSupplierZones:GetFilteredSupplierZones,
            GetSupplierZoneInfo: GetSupplierZoneInfo,
            GetSupplierZoneInfoByIds: GetSupplierZoneInfoByIds
        });
    }

    appControllers.service('WhS_BE_SupplierZoneAPIService', supplierZoneAPIService);
})(appControllers);