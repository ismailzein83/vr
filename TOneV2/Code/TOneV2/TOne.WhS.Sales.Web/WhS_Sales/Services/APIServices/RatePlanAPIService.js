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

        function GetZoneLetters(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneLetters"), input);
        }

        function GetZoneItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneItems"), input);
        }

        function GetZoneItem(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneItem"), input);
        }

        function GetDefaultItem(ownerType, ownerId, effectiveOn) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetDefaultItem"), {
                ownerType: ownerType,
                ownerId: ownerId,
                effectiveOn: effectiveOn
            });
        }

        function GetCostCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetCostCalculationMethodTemplates"));
        }

        function GetRateCalculationMethodTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetRateCalculationMethodTemplates"));
        }

        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "SaveChanges"), input);
        }

        function TryApplyCalculatedRates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "TryApplyCalculatedRates"), input);
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
        
        function GetDraftCurrencyId(ownerType, ownerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetDraftCurrencyId"), {
                ownerType: ownerType,
                ownerId: ownerId
            });
        }
        
        function DeleteChangedRates(ownerType, ownerId, newCurrencyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "DeleteChangedRates"), {
                ownerType: ownerType,
                ownerId: ownerId,
                newCurrencyId: newCurrencyId
            });
        }

        function GetCustomerDefaultInheritedService(customerId, effectiveOn) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetCustomerDefaultInheritedService"), {
                customerId: customerId,
                effectiveOn: effectiveOn
            });
        }

        function GetZoneInheritedService(ownerType, ownerId, zoneId, effectiveOn) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetZoneInheritedService"), {
                ownerType: ownerType,
                ownerId: ownerId,
                zoneId: zoneId,
                effectiveOn: effectiveOn
            });
        }

        return {
            ValidateCustomer: ValidateCustomer,
            GetZoneLetters: GetZoneLetters,
            GetDefaultItem: GetDefaultItem,
            GetZoneItems: GetZoneItems,
            GetZoneItem: GetZoneItem,
            GetCostCalculationMethodTemplates: GetCostCalculationMethodTemplates,
            GetRateCalculationMethodTemplates: GetRateCalculationMethodTemplates,
            SaveChanges: SaveChanges,
            TryApplyCalculatedRates: TryApplyCalculatedRates,
            ApplyCalculatedRates: ApplyCalculatedRates,
            CheckIfDraftExists: CheckIfDraftExists,
            DeleteDraft: DeleteDraft,
            GetRatePlanSettingsData: GetRatePlanSettingsData,
            GetDraftCurrencyId: GetDraftCurrencyId,
            DeleteChangedRates: DeleteChangedRates,
            GetCustomerDefaultInheritedService: GetCustomerDefaultInheritedService,
            GetZoneInheritedService: GetZoneInheritedService
        };

    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);