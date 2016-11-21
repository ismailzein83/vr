(function (appControllers) {

    "use strict";
    saleZonesAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig"];

    function saleZonesAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig) {

        var controllerName = "SaleZone";

        function GetFilteredSaleZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredSaleZones"), input);
        }

        function GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfo"), {
                nameFilter: nameFilter,
                sellingNumberPlanId: sellingNumberPlanId,
                serializedFilter: serializedFilter
            });
        }

        function GetSaleZonesInfoByIds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfoByIds"), input);
        }

        function GetSaleZone(saleZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZone"), {
                saleZoneId: saleZoneId
            });
        }
        function GetSellingNumberPlanIdBySaleZoneIds(saleZoneIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlanIdBySaleZoneId"), saleZoneIds);
        }
        function GetSaleZoneGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZoneGroupTemplates"));
        }

        function GetSaleZonesByName(customerId, saleZoneNameFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZonesByName"), {
                customerId: customerId,
                saleZoneNameFilter: saleZoneNameFilter
            });
        }
        function GetSaleZoneInfoByCountryId(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSaleZonesInfoByCountryId"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
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
            GetSaleZoneInfoByCountryId: GetSaleZoneInfoByCountryId
        });
    }

    appControllers.service("Vr_NP_SaleZoneAPIService", saleZonesAPIService);

})(appControllers);