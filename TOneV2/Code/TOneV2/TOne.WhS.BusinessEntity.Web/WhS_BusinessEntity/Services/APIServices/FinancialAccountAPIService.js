(function (appControllers) {

    'use strict';

    FinancialAccountAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function FinancialAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccount';

        function GetAccountCurrencyName(carrierProfileId, carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAccountCurrencyName"), {
                carrierProfileId: carrierProfileId,
                carrierAccountId: carrierAccountId
            });
        }

        function AddFinancialAccount(financialAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddFinancialAccount"), financialAccountObject);
        }

        function UpdateFinancialAccount(financialAccountToEdit) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateFinancialAccount"), financialAccountToEdit);
        }

        function GetFinancialAccountsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountsInfo"), {
                filter: filter
            });
        }

        function GetFilteredFinancialAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredFinancialAccounts"), input);
        }

        function CanAddFinancialAccountToCarrier(carrierProfileId, carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "CanAddFinancialAccountToCarrier"), {
                carrierProfileId: carrierProfileId,
                carrierAccountId: carrierAccountId
            });
        }
        function GetFinancialAccount(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccount"), {
                financialAccountId: financialAccountId
            });
        }
        function GetFinancialAccountRuntimeEditor(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountRuntimeEditor"), {
                financialAccountId: financialAccountId
            });
        }

        function GetSupplierTimeZoneId(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierTimeZoneId"), {
                financialAccountId: financialAccountId
            });
        }
        function GetFinancialAccountCurrencyId(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountCurrencyId"), {
                financialAccountId: financialAccountId
            });
        }

        function GetCustomerTimeZoneId(financialAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerTimeZoneId"), {
                financialAccountId: financialAccountId
            });
        }

        function HasViewFinancialAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredFinancialAccounts']));
        }

        function HasUpdateFinancialAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateFinancialAccount']));
        }

        function HasAddFinancialAccountPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddFinancialAccount']));
        }

        return ({
            GetFinancialAccountCurrencyId:GetFinancialAccountCurrencyId,
            GetAccountCurrencyName: GetAccountCurrencyName,
            AddFinancialAccount: AddFinancialAccount,
            UpdateFinancialAccount: UpdateFinancialAccount,
            GetFinancialAccountsInfo: GetFinancialAccountsInfo,
            GetFilteredFinancialAccounts: GetFilteredFinancialAccounts,
            CanAddFinancialAccountToCarrier: CanAddFinancialAccountToCarrier,
            GetFinancialAccount: GetFinancialAccount,
            GetFinancialAccountRuntimeEditor: GetFinancialAccountRuntimeEditor,
            GetSupplierTimeZoneId: GetSupplierTimeZoneId,
            GetCustomerTimeZoneId: GetCustomerTimeZoneId,
            HasViewFinancialAccountPermission: HasViewFinancialAccountPermission,
            HasUpdateFinancialAccountPermission: HasUpdateFinancialAccountPermission,
            HasAddFinancialAccountPermission: HasAddFinancialAccountPermission
        });
    }

    appControllers.service("WhS_BE_FinancialAccountAPIService", FinancialAccountAPIService);

})(appControllers);
