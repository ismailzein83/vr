﻿(function (appControllers) {

    "use strict";
    invoiceItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceItemAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceItem';

        function GetFilteredInvoiceItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoiceItems"), input);
        }

        return ({
            GetFilteredInvoiceItems: GetFilteredInvoiceItems
        });
    }

    appControllers.service('VR_Invoice_InvoiceItemAPIService', invoiceItemAPIService);

})(appControllers);