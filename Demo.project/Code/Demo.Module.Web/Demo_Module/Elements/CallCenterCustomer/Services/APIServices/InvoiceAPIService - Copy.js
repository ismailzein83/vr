(function (appControllers) {
    "use strict";
    invoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function invoiceAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Invoice";

        function GetInvoices() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetInvoices"));
        }

        return {
            GetInvoices: GetInvoices
        };
    };
    appControllers.service("Demo_Module_InvoiceAPIService", invoiceAPIService);

})(appControllers);