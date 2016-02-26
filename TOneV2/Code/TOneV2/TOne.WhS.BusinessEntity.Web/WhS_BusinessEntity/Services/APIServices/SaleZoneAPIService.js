﻿(function (appControllers) {

    "use strict";
    saleZonesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function saleZonesAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        function GetFilteredSaleZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetFilteredSaleZones"), input);
        }

        function GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZonesInfo"), {
                nameFilter: nameFilter,
                sellingNumberPlanId: sellingNumberPlanId,
                serializedFilter: serializedFilter
            });
        }

        function GetSaleZonesInfoByIds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZonesInfoByIds"), input);
        }

        function GetSaleZone(saleZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZone"), { saleZoneId: saleZoneId });
        }
        function GetSellingNumberPlanIdBySaleZoneId(saleZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSellingNumberPlanIdBySaleZoneId"), { saleZoneId: saleZoneId });
        }
        function GetSaleZoneGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZoneGroupTemplates"));
        }

        function GetSaleZonesByName(customerId, saleZoneNameFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZonesByName"), {
                customerId: customerId,
                saleZoneNameFilter: saleZoneNameFilter
            });
        }
        function GetSaleZoneInfoByCountryId(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleZone", "GetSaleZonesInfoByCountryId"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }
        return ({
            GetFilteredSaleZones: GetFilteredSaleZones,
            GetSaleZonesInfo: GetSaleZonesInfo,
            GetSellingNumberPlanIdBySaleZoneId:GetSellingNumberPlanIdBySaleZoneId,
            GetSaleZonesInfoByIds: GetSaleZonesInfoByIds,
            GetSaleZoneGroupTemplates: GetSaleZoneGroupTemplates,
            GetSaleZonesByName: GetSaleZonesByName,
            GetSaleZone: GetSaleZone,
            GetSaleZoneInfoByCountryId: GetSaleZoneInfoByCountryId
        });
    }

    appControllers.service('WhS_BE_SaleZoneAPIService', saleZonesAPIService);

})(appControllers);