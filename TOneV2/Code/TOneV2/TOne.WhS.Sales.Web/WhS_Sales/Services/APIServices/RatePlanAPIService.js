(function (appControllers) {

    "use strict";

    ratePlanAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function ratePlanAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        return ({
            ValidateCustomer: ValidateCustomer,
            GetZoneLetters: GetZoneLetters,
            GetDefaultItem: GetDefaultItem,
            GetZoneItems: GetZoneItems,
            GetZoneItem: GetZoneItem,
            GetCostCalculationMethodTemplates: GetCostCalculationMethodTemplates,
            GetRateCalculationMethodTemplates: GetRateCalculationMethodTemplates,
            GetChangesSummary: GetChangesSummary,
            GetFilteredZoneRateChanges: GetFilteredZoneRateChanges,
            GetFilteredZoneRoutingProductChanges: GetFilteredZoneRoutingProductChanges,
            SavePriceList: SavePriceList,
            SaveChanges: SaveChanges,
            ApplyCalculatedRates: ApplyCalculatedRates
        });

        function ValidateCustomer(customerId, effectiveOn) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "ValidateCustomer"), {
                customerId: customerId,
                effectiveOn: effectiveOn
            });
        }

        function GetZoneLetters(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneLetters"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetZoneItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneItems"), input);
        }

        function GetZoneItem(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetZoneItem"), input);
        }

        function GetDefaultItem(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetDefaultItem"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetCostCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetCostCalculationMethodTemplates"));
        }

        function GetRateCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetRateCalculationMethodTemplates"));
        }

        function GetChangesSummary(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetChangesSummary"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetFilteredZoneRateChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetFilteredZoneRateChanges"), input);
        }

        function GetFilteredZoneRoutingProductChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetFilteredZoneRoutingProductChanges"), input);
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

        function ApplyCalculatedRates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "ApplyCalculatedRates"), input);
        }
    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);
