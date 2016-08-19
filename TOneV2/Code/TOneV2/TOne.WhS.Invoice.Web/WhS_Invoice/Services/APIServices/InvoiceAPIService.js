(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Invoice_ModuleConfig", "VRModalService"];

    function invoiceAPIService(BaseAPIService, UtilsService, WhS_Invoice_ModuleConfig, VRModalService) {

        var controllerName = "WhSInvoice";

        function GetInvoiceCarriers(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceCarriers"), filter);
        }

        return ({
            GetInvoiceCarriers: GetInvoiceCarriers,
        });
    }

    appControllers.service("WhS_Invoice_InvoiceAPIService", invoiceAPIService);
})(appControllers);