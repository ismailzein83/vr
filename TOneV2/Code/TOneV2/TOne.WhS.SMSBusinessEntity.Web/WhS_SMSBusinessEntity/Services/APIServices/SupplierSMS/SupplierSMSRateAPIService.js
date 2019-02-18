(function (appControllers) {

    "use strict";

    supplierSMSRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig"];

    function supplierSMSRateAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig) {

        var controllerName = "SupplierSMSRate";

        function GetFilteredSupplierSMSRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierSMSRate"), input);
        }

        function HasSearchRatesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['GetFilteredSupplierSMSRate']));
        }

        function HasAddDraftPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['AddSupplierSMSRateDraft']));
        }

        return {
            GetFilteredSupplierSMSRate: GetFilteredSupplierSMSRate,
            HasSearchRatesPermission: HasSearchRatesPermission,
            HasAddDraftPermission: HasAddDraftPermission
        };

    }

    appControllers.service("WhS_SMSBusinessEntity_SupplierSMSRateAPIService", supplierSMSRateAPIService);

})(appControllers);