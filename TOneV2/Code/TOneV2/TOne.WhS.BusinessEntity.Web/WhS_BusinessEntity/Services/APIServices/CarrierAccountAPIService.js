(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCarrierAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetFilteredCarrierAccounts"), input);
        }
        
        function GetCarrierAccount(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccount"), {
                carrierAccountId: carrierAccountId
            });

        }
        function GetCarrierAccounts(getCustomers,getSuppliers) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccounts"), {
                getCustomers: getCustomers,
                getSuppliers: getSuppliers
            });

        }

        function GetSupplierGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetSupplierGroupTemplates"));
        }

        function GetCustomerGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCustomerGroupTemplates"));
        }

        return ({
            GetCarrierAccounts: GetCarrierAccounts,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates,
            GetCustomerGroupTemplates: GetCustomerGroupTemplates,
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount
        });
    }

    appControllers.service('WhS_BE_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);