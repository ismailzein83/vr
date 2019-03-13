(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_Interconnect_ModuleConfig"];

    function invoiceAPIService(BaseAPIService, UtilsService, Retail_Interconnect_ModuleConfig) {

        var controllerName = "InterconnectInvoiceCompareController";

        function CompareVoiceInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "CompareVoiceInvoices"), input);
        }

        function CompareSMSInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "CompareSMSInvoices"), input);
        }

        return ({
            CompareVoiceInvoices: CompareVoiceInvoices,
            CompareSMSInvoices: CompareSMSInvoices
        });
    }

    appControllers.service("Retail_Interconnect_InvoiceCompareService", invoiceAPIService);
})(appControllers);