﻿(function (appControllers) {

    'use strict';

    InvoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function InvoiceAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {
        var controllerName = 'Invoice';

        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoices"), input);
        };

        return {
            GetFilteredInvoices: GetFilteredInvoices,
        };
    }

    appControllers.service('PartnerPortal_Invoice_InvoiceAPIService', InvoiceAPIService);

})(appControllers);