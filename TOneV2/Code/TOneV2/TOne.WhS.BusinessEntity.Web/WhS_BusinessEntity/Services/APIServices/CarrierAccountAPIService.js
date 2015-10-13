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
        function GetCarrierAccountsInfo(getCustomers, getSuppliers) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccountsInfo"), {
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
        function AddCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "AddCarrierAccount"), carrierAccountObject);
        }
        function UpdateCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CarrierAccount", "UpdateCarrierAccount"), carrierAccountObject);
        }
        return ({
            GetCarrierAccountsInfo: GetCarrierAccountsInfo,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates,
            GetCustomerGroupTemplates: GetCustomerGroupTemplates,
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount,
            AddCarrierAccount: AddCarrierAccount,
            UpdateCarrierAccount: UpdateCarrierAccount
        });
    }

    appControllers.service('WhS_BE_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);