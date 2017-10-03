﻿(function (appControllers) {

    "use strict";
    saleZonesAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function saleZonesAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SaleZone";

        function GetFilteredSaleZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSaleZones"), input);
        }

        function GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfo"), {
                nameFilter: nameFilter,
                sellingNumberPlanId: sellingNumberPlanId,
                serializedFilter: serializedFilter
            });
        }

        function GetSaleZonesInfoByIds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfoByIds"), input);
        }

        function GetSaleZone(saleZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZone"), {
                saleZoneId: saleZoneId
            });
        }
        function GetSellingNumberPlanIdBySaleZoneIds(saleZoneIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlanIdBySaleZoneId"), saleZoneIds);
        }
        function GetSaleZoneGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZoneGroupTemplates"));
        }

        function GetSaleZonesByName(customerId, saleZoneNameFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZonesByName"), {
                customerId: customerId,
                saleZoneNameFilter: saleZoneNameFilter
            });
        }
        function GetSaleZoneInfoByCountryId(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfoByCountryId"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }

        function UpdateSaleZoneName(zoneId, zoneName,sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSaleZoneName"), {
                zoneId: zoneId,
                zoneName: zoneName,
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        return ({
            GetFilteredSaleZones: GetFilteredSaleZones,
            GetSaleZonesInfo: GetSaleZonesInfo,
            GetSellingNumberPlanIdBySaleZoneIds: GetSellingNumberPlanIdBySaleZoneIds,
            GetSaleZonesInfoByIds: GetSaleZonesInfoByIds,
            GetSaleZoneGroupTemplates: GetSaleZoneGroupTemplates,
            GetSaleZonesByName: GetSaleZonesByName,
            GetSaleZone: GetSaleZone,
            GetSaleZoneInfoByCountryId: GetSaleZoneInfoByCountryId,
            UpdateSaleZoneName: UpdateSaleZoneName
        });
    }

    appControllers.service("WhS_BE_SaleZoneAPIService", saleZonesAPIService);

})(appControllers);