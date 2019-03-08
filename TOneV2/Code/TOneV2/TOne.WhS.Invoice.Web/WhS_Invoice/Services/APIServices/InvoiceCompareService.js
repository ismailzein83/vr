(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function invoiceAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "WhSInvoiceCompare";

        function CompareVoiceInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "CompareVoiceInvoices"), input);
        }

        function CompareSMSInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "CompareSMSInvoices"), input);
        }

        return ({
            CompareVoiceInvoices: CompareVoiceInvoices,
            CompareSMSInvoices: CompareSMSInvoices
        });
    }

    appControllers.service("WhS_Invoice_InvoiceCompareService", invoiceAPIService);
})(appControllers);