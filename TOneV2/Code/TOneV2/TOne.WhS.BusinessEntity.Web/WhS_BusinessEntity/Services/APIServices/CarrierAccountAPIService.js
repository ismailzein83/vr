(function (appControllers) {

    "use strict";
    carrierAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function carrierAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'CarrierAccount';

        function GetFilteredCarrierAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredCarrierAccounts"), input);
        }

        function GetCarrierAccountCurrencyId(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccountCurrencyId"), {
                carrierAccountId: carrierAccountId
            });
        }
        function GetCarrierAccountIdsAssignedToSellingProduct(sellingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccountIdsAssignedToSellingProduct"), {
                sellingProductId: sellingProductId
            });
        }
        function GetCustomersBySellingNumberPlanId(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomersBySellingNumberPlanId"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function GetSellingProductId(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingProductId"), {
                carrierAccountId: carrierAccountId
            });
        }
        
        function GetCarrierAccount(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccount"), {
                carrierAccountId: carrierAccountId
            });
        }

        function GetCarrierAccountInfos(serializedCarrierAccountIds) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccountInfos"), {
                serializedCarrierAccountIds: serializedCarrierAccountIds
            });
        }

        function GetCarrierAccountName(carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCarrierAccountName"), {
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
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateCarrierAccount']));
        }
        
        function HasAddCarrierAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddCarrierAccount']));
        }

        function HasViewCarrierAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredCarrierAccounts']));
        }

        function GetCarrierAccountHistoryDetailbyHistoryId(carrierAccountHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetCarrierAccountHistoryDetailbyHistoryId'), {
                carrierAccountHistoryId: carrierAccountHistoryId
            });
        }

        function GetCustomerPricingSettings(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetCustomerPricingSettings'), {
                customerId: customerId
            });
        }

        function GetPassThroughCustomerRateExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetPassThroughCustomerRateExtensionConfigs'));
        }

        return ({
            GetCarrierAccountInfo: GetCarrierAccountInfo,
            GetSupplierGroupTemplates: GetSupplierGroupTemplates,
            GetCustomerGroupTemplates: GetCustomerGroupTemplates,
            GetFilteredCarrierAccounts: GetFilteredCarrierAccounts,
            GetCarrierAccount: GetCarrierAccount,
            GetCarrierAccountInfos: GetCarrierAccountInfos,
            GetCarrierAccountName:GetCarrierAccountName,
            GetCarrierAccountCurrencyId: GetCarrierAccountCurrencyId,
            GetCustomersBySellingNumberPlanId: GetCustomersBySellingNumberPlanId,
            GetSellingProductId:GetSellingProductId,
            AddCarrierAccount: AddCarrierAccount,
            UpdateCarrierAccount: UpdateCarrierAccount,
            GetSuppliersWithZonesGroupsTemplates: GetSuppliersWithZonesGroupsTemplates,
            HasUpdateCarrierAccountPermission: HasUpdateCarrierAccountPermission,
            HasAddCarrierAccountPermission: HasAddCarrierAccountPermission,
            HasViewCarrierAccountPermission: HasViewCarrierAccountPermission,
            GetCarrierAccountHistoryDetailbyHistoryId: GetCarrierAccountHistoryDetailbyHistoryId,
            GetCarrierAccountIdsAssignedToSellingProduct: GetCarrierAccountIdsAssignedToSellingProduct,
            GetCustomerPricingSettings: GetCustomerPricingSettings,
            GetPassThroughCustomerRateExtensionConfigs: GetPassThroughCustomerRateExtensionConfigs
        });
    }

    appControllers.service('WhS_BE_CarrierAccountAPIService', carrierAccountAPIService);

})(appControllers);