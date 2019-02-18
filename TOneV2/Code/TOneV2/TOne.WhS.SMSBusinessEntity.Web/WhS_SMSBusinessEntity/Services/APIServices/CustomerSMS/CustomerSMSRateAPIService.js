(function (appControllers) {

    "use strict";

    customerSMSRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig", "SecurityService"];

    function customerSMSRateAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig, SecurityService) {

        var controllerName = "CustomerSMSRate";

        function GetFilteredCustomerSMSRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerSMSRate"), input);
        }

        function HasSearchRatesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['GetFilteredCustomerSMSRate']));
        }

        function HasAddDraftPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['AddCustomerSMSRateDraft']));
        }

        return {
            GetFilteredCustomerSMSRate: GetFilteredCustomerSMSRate,
            HasSearchRatesPermission: HasSearchRatesPermission,
            HasAddDraftPermission: HasAddDraftPermission
        };
    }

    appControllers.service("WhS_SMSBusinessEntity_CustomerSMSRateAPIService", customerSMSRateAPIService);

})(appControllers);