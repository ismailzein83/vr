//(function (appControllers) {

//    "use strict";
//    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_Interconnect_ModuleConfig"];

//    function invoiceAPIService(BaseAPIService, UtilsService, Retail_Interconnect_ModuleConfig) {

//        var controllerName = "InterconnectInvoiceController";

//        function GetInvoiceDetails(invoiceId, invoiceCarrierType) {
//            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetails'), {
//                invoiceId: invoiceId,
//                invoiceCarrierType: invoiceCarrierType
//            });
//        }
//        return ({
//            GetInvoiceDetails: GetInvoiceDetails
//        });
//    }

//    appControllers.service("Retail_Interconnect_InvoiceAPIService", invoiceAPIService);
//})(appControllers);