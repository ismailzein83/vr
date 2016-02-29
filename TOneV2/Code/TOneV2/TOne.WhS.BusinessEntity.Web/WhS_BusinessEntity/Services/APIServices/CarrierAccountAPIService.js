(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'CarrierAccount';

        function GetFilteredCarrierAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredCarrierAccounts"), input);
        }
        
        function GetCarrierAccount(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccount"), {
                carrierAccountId: carrierAccountId
            });

        }
        function GetCarrierAccountInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccountInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetSupplierGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierGroupTemplates"));
        }

        function GetCustomerGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerGroupTemplates"));
        }
        function AddCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddCarrierAccount"), carrierAccountObject);
        }
        function UpdateCarrierAccount(carrierAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateCarrierAccount"), carrierAccountObject);
        }
        function GetSuppliersWithZonesGroupsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSuppliersWithZonesGroupsTemplates"));
        }

        function HasUpdateCarrierAccountPermission() {
            return SecurityService.IsAllowedBySystemActionNames(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateCarrierAccount']));
        }

        return ({
            GetCarrierAccountInfo: GetCarrierAccountInfo,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates,
            GetCustomerGroupTemplates: GetCustomerGroupTemplates,
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount,
            AddCarrierAccount: AddCarrierAccount,
            UpdateCarrierAccount: UpdateCarrierAccount,
            GetSuppliersWithZonesGroupsTemplates: GetSuppliersWithZonesGroupsTemplates,
            HasUpdateCarrierAccountPermission: HasUpdateCarrierAccountPermission
        });
    }

    appControllers.service('WhS_BE_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);