(function (appControllers) {

    "use strict";

    supplierSMSPriceListAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig", "SecurityService"];

    function supplierSMSPriceListAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig, SecurityService) {

        var controllerName = "SupplierSMSPriceList";

        function GetFilteredSupplierSMSPriceLists(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierSMSPriceLists"), input);
        }

        function HasSearchPriceListsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['GetFilteredSupplierPriceList']));
        }

        return {
            GetFilteredSupplierSMSPriceLists: GetFilteredSupplierSMSPriceLists,
            HasSearchPriceListsPermission: HasSearchPriceListsPermission
        };
    }

    appControllers.service("WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService", supplierSMSPriceListAPIService);

})(appControllers);