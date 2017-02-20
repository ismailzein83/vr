(function (appControllers) {

    'use strict';

    InvoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_CustomerAccess_ModuleConfig', 'SecurityService'];

    function InvoiceAPIService(BaseAPIService, UtilsService, PartnerPortal_CustomerAccess_ModuleConfig, SecurityService) {
        var controllerName = 'Invoice';

        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetFilteredInvoices"), input);
        };

        function GetInvoiceContextHandlerTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_CustomerAccess_ModuleConfig.moduleName, controllerName, "GetInvoiceContextHandlerTemplates"));
        };

        return {
            GetFilteredInvoices: GetFilteredInvoices,
            GetInvoiceContextHandlerTemplates: GetInvoiceContextHandlerTemplates
        };
    }

    appControllers.service('PartnerPortal_CustomerAccess_InvoiceAPIService', InvoiceAPIService);

})(appControllers);