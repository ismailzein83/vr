(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredCarrierAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetFilteredCarrierAccounts"), input);
        }
        
        function GetCarrierAccount(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccount"), {
                carrierAccountId: carrierAccountId
            });

        }
        function GetCarrierAccountInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetCarrierAccountInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetSupplierGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetSupplierGroupTemplates"));
        }

        function GetCustomerGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetCustomerGroupTemplates"));
        }
        function AddCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "AddCarrierAccount"), carrierAccountObject);
        }
        function UpdateCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "UpdateCarrierAccount"), carrierAccountObject);
        }
        function GetSuppliersWithZonesGroupsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CarrierAccount", "GetSuppliersWithZonesGroupsTemplates"));
        }

        return ({
            GetCarrierAccountInfo: GetCarrierAccountInfo,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates,
            GetCustomerGroupTemplates: GetCustomerGroupTemplates,
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount,
            AddCarrierAccount: AddCarrierAccount,
            UpdateCarrierAccount: UpdateCarrierAccount,
            GetSuppliersWithZonesGroupsTemplates: GetSuppliersWithZonesGroupsTemplates
        });
    }

    appControllers.service('Demo_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);