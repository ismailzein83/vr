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
        function SetInvoicePaid(invoiceId, isInvoicePaid) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'SetInvoicePaid'), {
                invoiceId: invoiceId,
                isInvoicePaid: isInvoicePaid
            });
        }
        function GetInvoiceDetail(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetail'), {
                invoiceId: invoiceId,
            });
        }
        return ({
            GetInvoice: GetInvoice,
            GenerateInvoice: GenerateInvoice,
            GetFilteredInvoices: GetFilteredInvoices,
            SetInvoicePaid: SetInvoicePaid,
            GetInvoiceDetail: GetInvoiceDetail
        });
    }

    appControllers.service('VR_Invoice_InvoiceAPIService', invoiceAPIService);

})(appControllers);