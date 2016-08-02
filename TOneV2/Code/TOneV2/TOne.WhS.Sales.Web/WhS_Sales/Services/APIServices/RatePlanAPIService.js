(function (appControllers) {

    "use strict";

    ratePlanAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function ratePlanAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        var controllerName = "RatePlan";

        function ValidateCustomer(customerId, effectiveOn) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "ValidateCustomer"), {
                customerId: customerId,
                effectiveOn: effectiveOn
            });
        }

        function GetZoneLetters(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneLetters"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetZoneItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneItems"), input);
        }

        function GetZoneItem(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneItem"), input);
        }

        function GetDefaultItem(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetDefaultItem"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetCostCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetCostCalculationMethodTemplates"));
        }

        function GetRateCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetRateCalculationMethodTemplates"));
        }

        function GetChangesSummary(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetChangesSummary"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetFilteredZoneRateChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredZoneRateChanges"), input);
        }

        function GetFilteredZoneRoutingProductChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredZoneRoutingProductChanges"), input);
        }

        function SavePriceList(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "SavePriceList"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "SaveChanges"), input);
        }

        function ApplyCalculatedRates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "ApplyCalculatedRates"), input);
        }

        function CheckIfDraftExists(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "CheckIfDraftExists"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function DeleteDraft(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "DeleteDraft"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }

        function GetRatePlanSettingsData() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetRatePlanSettingsData"));
        }

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
            ApplyCalculatedRates: ApplyCalculatedRates,
            CheckIfDraftExists: CheckIfDraftExists,
            DeleteDraft: DeleteDraft,
            GetRatePlanSettingsData: GetRatePlanSettingsData
        });

    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);
