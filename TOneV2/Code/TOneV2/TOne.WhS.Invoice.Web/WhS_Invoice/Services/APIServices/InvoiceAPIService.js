(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function invoiceAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "WhSInvoice";

        function CompareInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "CompareInvoices"), input);
        }
        function UpdateOriginalInvoiceData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "UpdateOriginalInvoiceData"), input);
        }
        return ({
            CompareInvoices: CompareInvoices,
            UpdateOriginalInvoiceData: UpdateOriginalInvoiceData
        });
    }

    appControllers.service("WhS_Invoice_InvoiceAPIService", invoiceAPIService);
})(appControllers);