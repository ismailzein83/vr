(function (appControllers) {

    "use strict";
    ComparisonInvoiceDetailAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function ComparisonInvoiceDetailAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "WhSInvoice";

        
        function GetComparisonInvoiceDetail(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, 'GetComparisonInvoiceDetail'), {
                invoiceId: invoiceId,
            });
        }

        return ({
            GetComparisonInvoiceDetail: GetComparisonInvoiceDetail
        });
    }

    appControllers.service("WhS_Invoice_ComparisonInvoiceDetailAPIService", ComparisonInvoiceDetailAPIService);
})(appControllers);