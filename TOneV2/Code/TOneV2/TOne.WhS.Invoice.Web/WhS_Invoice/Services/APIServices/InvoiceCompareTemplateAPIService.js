(function (appControllers) {

    "use strict";
    InvoiceCompareTemplateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function InvoiceCompareTemplateAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "InvoiceCompareTemplateController";

        function SaveInvoiceCompareTemplate(invoiceComparisonTemplate) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "SaveInvoiceCompareTemplate"), invoiceComparisonTemplate);
        }
        function GetInvoiceCompareTemplate(invoiceTypeId, partnerId, invoiceCarrierType) {
         
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceCompareTemplate"), {
                invoiceTypeId: invoiceTypeId,
                partnerId: partnerId,
                invoiceCarrierType:invoiceCarrierType
            });
        }
     
        return ({
            SaveInvoiceCompareTemplate: SaveInvoiceCompareTemplate,
            GetInvoiceCompareTemplate: GetInvoiceCompareTemplate
        });
    }

    appControllers.service("WhS_Invoice_InvoiceCompareTemplateAPIService", InvoiceCompareTemplateAPIService);
})(appControllers);