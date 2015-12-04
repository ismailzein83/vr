(function (appControllers) {

    "use strict";

    ratePlanAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function ratePlanAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        return ({
            GetZoneLetters: GetZoneLetters,
            GetDefaultItem: GetDefaultItem,
            GetZoneItems: GetZoneItems,
            GetZoneItem: GetZoneItem,
            GetCostCalculationMethodTemplates: GetCostCalculationMethodTemplates,
            SavePriceList: SavePriceList,
            SaveChanges: SaveChanges
        });

        function GetZoneLetters(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneLetters"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetZoneItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneItems"), input);
        }

        function GetZoneItem(ownerType, ownerId, routingDatabaseId, policyConfigId, numberOfOptions, zoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneItem"), {
                ownerType: ownerType,
                ownerId: ownerId,
                routingDatabaseId: routingDatabaseId,
                policyConfigId: policyConfigId,
                numberOfOptions: numberOfOptions,
                zoneId: zoneId
            });
        }

        function GetDefaultItem(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetDefaultItem"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function SavePriceList(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "SavePriceList"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "SaveChanges"), input);
        }

        function GetCostCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetCostCalculationMethodTemplates"));
        }
    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);
