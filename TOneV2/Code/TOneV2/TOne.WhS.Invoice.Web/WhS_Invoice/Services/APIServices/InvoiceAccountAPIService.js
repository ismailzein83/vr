(function (appControllers) {

    'use strict';

    InvoiceAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Invoice_ModuleConfig', 'SecurityService'];

    function InvoiceAccountAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceAccount';

        function GetFilteredInvoiceAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoiceAccounts"), input);
        }

        function AddInvoiceAccount(invoiceAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "AddInvoiceAccount"), invoiceAccountObject);
        }

        function UpdateInvoiceAccount(invoiceAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "UpdateInvoiceAccount"), invoiceAccountObject);
        }

        function GetInvoiceAccount(invoiceAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceAccount"), {
                invoiceAccountId: invoiceAccountId
            });
        }

        function CheckCarrierAllowAddInvoiceAccounts(carrierProfileId, carrierAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "CheckCarrierAllowAddInvoiceAccounts"), {
                carrierProfileId: carrierProfileId,
                carrierAccountId: carrierAccountId
            });
        }

        function GetInvoiceAccountsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceAccountsInfo"), {
                filter: filter
            });
        }
        function GetInvoiceAccountEditorRuntime(invoiceAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceAccountEditorRuntime"), {
                invoiceAccountId: invoiceAccountId
            });
        }
        return {
            GetFilteredInvoiceAccounts: GetFilteredInvoiceAccounts,
            AddInvoiceAccount: AddInvoiceAccount,
            UpdateInvoiceAccount: UpdateInvoiceAccount,
            CheckCarrierAllowAddInvoiceAccounts: CheckCarrierAllowAddInvoiceAccounts,
            GetInvoiceAccount: GetInvoiceAccount,
            GetInvoiceAccountsInfo: GetInvoiceAccountsInfo,
            GetInvoiceAccountEditorRuntime: GetInvoiceAccountEditorRuntime
        };
    }

    appControllers.service('WhS_Invoice_InvoiceAccountAPIService', InvoiceAccountAPIService);

})(appControllers);