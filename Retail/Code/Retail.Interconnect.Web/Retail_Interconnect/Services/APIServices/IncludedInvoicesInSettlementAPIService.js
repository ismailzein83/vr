(function (appControllers) {

    "use strict";
    includedInvoicesInSettlementAPIService.$inject = ["BaseAPIService", "UtilsService", "Retail_Interconnect_ModuleConfig"];

    function includedInvoicesInSettlementAPIService(BaseAPIService, UtilsService, Retail_Interconnect_ModuleConfig) {

        var controllerName = "InvoiceSettlement";
        
        function TryLoadInvoicesAndGetAmountByCurrency(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "TryLoadInvoicesAndGetAmountByCurrency"), input);
        }
        function LoadInvoicesDetails(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "LoadInvoicesDetails"), input);
        }
        function EvaluatePartnerCustomPayload(partnerId, invoiceTypeId, fromDate, toDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Interconnect_ModuleConfig.moduleName, controllerName, "EvaluatePartnerCustomPayload"), {
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

    appControllers.service("Retail_Interconnect_IncludedInvoicesInSettlementAPIService", includedInvoicesInSettlementAPIService);
})(appControllers);