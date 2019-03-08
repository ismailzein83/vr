//(function (appControllers) {

//    "use strict";
//    InvoiceCompareTemplateAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_Interconnect_ModuleConfig"];

//    function InvoiceCompareTemplateAPIService(BaseAPIService, UtilsService, Retail_Interconnect_ModuleConfig) {

//        var controllerName = "InterconnectInvoiceCompareTemplateController";

//        function SaveInvoiceCompareTemplate(invoiceComparisonTemplate) {
//            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "SaveInvoiceCompareTemplate"), invoiceComparisonTemplate);
//        }
//        function GetInvoiceCompareTemplate(invoiceTypeId, partnerId, invoiceCarrierType) {

//            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "GetInvoiceCompareTemplate"), {
//                invoiceTypeId: invoiceTypeId,
//                partnerId: partnerId,
//                invoiceCarrierType: invoiceCarrierType
//            });
//        }

//        return ({
//            SaveInvoiceCompareTemplate: SaveInvoiceCompareTemplate,
//            GetInvoiceCompareTemplate: GetInvoiceCompareTemplate
//        });
//    }

//    appControllers.service("Retail_Interconnect_InvoiceCompareTemplateAPIService", InvoiceCompareTemplateAPIService);
//})(appControllers);