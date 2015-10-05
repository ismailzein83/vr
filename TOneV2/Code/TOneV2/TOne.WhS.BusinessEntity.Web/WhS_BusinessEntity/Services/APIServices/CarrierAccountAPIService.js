(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetCarrierAccounts(getCustomers,getSuppliers) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccounts"), {
                getCustomers: getCustomers,
                getSuppliers: getSuppliers
            });

        }

        function GetSupplierGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetSupplierGroupTemplates"));
        }

        return ({
            GetCarrierAccounts: GetCarrierAccounts,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates
        });
    }

    appControllers.service('WhS_BE_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);