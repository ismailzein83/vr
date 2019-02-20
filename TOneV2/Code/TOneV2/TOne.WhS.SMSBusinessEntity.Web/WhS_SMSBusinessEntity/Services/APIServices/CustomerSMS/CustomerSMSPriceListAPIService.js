(function (appControllers) {

    "use strict";

    customerSMSPriceListAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig", "SecurityService"];

    function customerSMSPriceListAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig, SecurityService) {

        var controllerName = "CustomerSMSPriceList";

        function GetFilteredCustomerSMSPriceLists(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerSMSPriceLists"), input);
        }

        function HasSearchPriceListsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['AddCustomerSMSRateDraft']));
        }

        return {
            GetFilteredCustomerSMSPriceLists: GetFilteredCustomerSMSPriceLists,
            HasSearchPriceListsPermission: HasSearchPriceListsPermission
        };
    }

    appControllers.service("WhS_SMSBusinessEntity_CustomerSMSPriceListAPIService", customerSMSPriceListAPIService);

})(appControllers);