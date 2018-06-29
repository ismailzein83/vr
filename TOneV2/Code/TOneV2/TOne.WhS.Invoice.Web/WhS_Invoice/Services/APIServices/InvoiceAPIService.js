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
        function GetOriginalInvoiceDataRuntime(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetOriginalInvoiceDataRuntime"), {
                invoiceId: invoiceId
            });
        }
        function GetInvoiceDetails(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetails'), {
                invoiceId: invoiceId,
            });
        }
        return ({
            CompareInvoices: CompareInvoices,
            UpdateOriginalInvoiceData: UpdateOriginalInvoiceData,
            GetOriginalInvoiceDataRuntime: GetOriginalInvoiceDataRuntime,
            GetInvoiceDetails: GetInvoiceDetails
        });
    }

    appControllers.service("WhS_Invoice_InvoiceAPIService", invoiceAPIService);
})(appControllers);