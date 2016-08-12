(function (appControllers) {

    "use strict";
    invoiceTypeService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetInvoiceType(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceType"), {
                invoiceTypeId: invoiceTypeId
            });
        }

        return ({
            GetInvoiceType: GetInvoiceType,
        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeService', invoiceTypeService);

})(appControllers);