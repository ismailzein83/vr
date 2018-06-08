
(function (appControllers) {

    "use strict";

    AccountPartTaxesRuntimeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function AccountPartTaxesRuntimeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "AccountPartTaxesRuntime";

        function GetInvoiceTypesTaxesRuntime(invoiceTypesIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetInvoiceTypesTaxesRuntime'), invoiceTypesIds);
        }

        return ({
            GetInvoiceTypesTaxesRuntime: GetInvoiceTypesTaxesRuntime,
        });
    }

    appControllers.service('Retail_BE_AccountPartTaxesRuntimeAPIService', AccountPartTaxesRuntimeAPIService);

})(appControllers);