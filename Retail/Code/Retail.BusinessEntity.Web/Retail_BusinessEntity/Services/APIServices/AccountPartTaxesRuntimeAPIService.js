
(function (appControllers) {

    "use strict";

    AccountPartTaxesRuntimeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function AccountPartTaxesRuntimeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "AccountPartTaxesRuntime";

        function GetInvoiceTypesTaxesRuntime(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetInvoiceTypesTaxesRuntime'), input);
        }

        return ({
            GetInvoiceTypesTaxesRuntime: GetInvoiceTypesTaxesRuntime,
        });
    }

    appControllers.service('Retail_BE_AccountPartTaxesRuntimeAPIService', AccountPartTaxesRuntimeAPIService);

})(appControllers);