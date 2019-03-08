(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig"];

    function invoiceAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig) {

        var controllerName = "WhSInvoice";

        function UpdateOriginalInvoiceData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "UpdateOriginalInvoiceData"), input);
        }
        function GetOriginalInvoiceDataRuntime(invoiceId,invoiceCarrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetOriginalInvoiceDataRuntime"), {
                invoiceId: invoiceId,
                invoiceCarrierType:invoiceCarrierType
            });
        }
        function GetInvoiceDetails(invoiceId,invoiceCarrierType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetails'), {
                invoiceId: invoiceId,
                invoiceCarrierType:invoiceCarrierType
            });
		}
		function DoesInvoiceReportExist(isCustomer) {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, 'DoesInvoiceReportExist'), {
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

    appControllers.service("WhS_Invoice_InvoiceAPIService", invoiceAPIService);
})(appControllers);