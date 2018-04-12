(function (appControllers) {

    "use strict";
    includedInvoicesInSettlementAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function includedInvoicesInSettlementAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "InvoiceSettlement";
        
        function TryLoadInvoicesAndGetAmountByCurrency(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "TryLoadInvoicesAndGetAmountByCurrency"), input);
        }
        function LoadInvoicesDetails(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "LoadInvoicesDetails"), input);
        }
        function EvaluatePartnerCustomPayload(partnerId, invoiceTypeId, fromDate, toDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "EvaluatePartnerCustomPayload"), {
                partnerId: partnerId,
                invoiceTypeId: invoiceTypeId,
                fromDate: fromDate,
                toDate: toDate,
            });
        }
        return ({
            TryLoadInvoicesAndGetAmountByCurrency: TryLoadInvoicesAndGetAmountByCurrency,
            LoadInvoicesDetails: LoadInvoicesDetails,
            EvaluatePartnerCustomPayload: EvaluatePartnerCustomPayload
        });
    }

    appControllers.service("WhS_Invoice_IncludedInvoicesInSettlementAPIService", includedInvoicesInSettlementAPIService);
})(appControllers);