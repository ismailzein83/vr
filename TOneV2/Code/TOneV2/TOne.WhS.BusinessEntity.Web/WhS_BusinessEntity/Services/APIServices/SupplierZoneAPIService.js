﻿(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

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
            GetSupplierZoneInfo: GetSupplierZoneInfo,
            GetSupplierZoneInfoByIds: GetSupplierZoneInfoByIds
        });
    }

    appControllers.service('WhS_BE_SupplierZoneAPIService', supplierZoneAPIService);
})(appControllers);