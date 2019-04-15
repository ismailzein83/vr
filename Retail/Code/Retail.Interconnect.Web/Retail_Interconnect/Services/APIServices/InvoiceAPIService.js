(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_Interconnect_ModuleConfig"];

    function invoiceAPIService(BaseAPIService, UtilsService, Retail_Interconnect_ModuleConfig) {

        var controllerName = "InvoiceController";

        function UpdateOriginalInvoiceData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "UpdateOriginalInvoiceData"), input);
        }
        function GetOriginalInvoiceDataRuntime(invoiceId, invoiceCarrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "GetOriginalInvoiceDataRuntime"), {
                invoiceId: invoiceId,
                invoiceCarrierType: invoiceCarrierType
            });
        }
    
        function GetInvoiceDetails(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetails'), {
                invoiceId: invoiceId
            });
        }

        function DoesInvoiceReportExist(isCustomer) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, 'DoesInvoiceReportExist'), {
                isCustomer: isCustomer
            });
        }
        return ({
            UpdateOriginalInvoiceData: UpdateOriginalInvoiceData,
            GetOriginalInvoiceDataRuntime: GetOriginalInvoiceDataRuntime,
            GetInvoiceDetails: GetInvoiceDetails,
            DoesInvoiceReportExist: DoesInvoiceReportExist
        });
    }

    appControllers.service("Retail_Interconnect_InvoiceAPIService", invoiceAPIService);
})(appControllers);