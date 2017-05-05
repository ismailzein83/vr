(function (appControllers) {

    'use strict';

    InvoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function InvoiceAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {
        var controllerName = 'Invoice';

        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoices"), input);
        };
        function GetRemoteLastInvoice(connectionId, invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetRemoteLastInvoice"), {
                connectionId: connectionId,
                invoiceTypeId: invoiceTypeId
            });
        };

        return {
            GetFilteredInvoices: GetFilteredInvoices,
            GetRemoteLastInvoice: GetRemoteLastInvoice
        };
    }

    appControllers.service('PartnerPortal_Invoice_InvoiceAPIService', InvoiceAPIService);

})(appControllers);