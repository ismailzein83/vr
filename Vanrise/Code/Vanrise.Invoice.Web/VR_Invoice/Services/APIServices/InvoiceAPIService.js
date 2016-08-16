(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetInvoice(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoice"), {
                invoiceTypeId: invoiceId
            });
        }
        function GenerateInvoice(createInvoiceInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GenerateInvoice"), createInvoiceInput);
        }
        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetFilteredInvoices'), input);
        }

        return ({
            GetInvoice: GetInvoice,
            GenerateInvoice: GenerateInvoice,
            GetFilteredInvoices: GetFilteredInvoices
        });
    }

    appControllers.service('VR_Invoice_InvoiceAPIService', invoiceAPIService);

})(appControllers);