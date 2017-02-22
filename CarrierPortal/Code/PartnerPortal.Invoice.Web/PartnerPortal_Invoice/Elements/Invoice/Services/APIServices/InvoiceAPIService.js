(function (appControllers) {

    'use strict';

    InvoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function InvoiceAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {
        var controllerName = 'Invoice';

        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoices"), input);
        };

        function GetInvoiceQueryInterceptorTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceQueryInterceptorTemplates"));
        };

        return {
            GetFilteredInvoices: GetFilteredInvoices,
            GetInvoiceQueryInterceptorTemplates: GetInvoiceQueryInterceptorTemplates
        };
    }

    appControllers.service('PartnerPortal_Invoice_InvoiceAPIService', InvoiceAPIService);

})(appControllers);