(function (appControllers) {

    "use strict";
    invoiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetRemoteInvoiceTypeInfo(connectionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetRemoteInvoiceTypeInfo"), {
                connectionId: connectionId,
                filter: filter
            });
        }
        function GetRemoteInvoiceFieldsInfo(connectionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetRemoteInvoiceFieldsInfo"), {
                connectionId: connectionId,
                filter: filter
            });
        }
        function GetRemoteInvoiceTypeCustomFieldsInfo(connectionId, invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetRemoteInvoiceTypeCustomFieldsInfo"), {
                connectionId: connectionId,
                invoiceTypeId: invoiceTypeId
            });
        }
        return ({
            GetRemoteInvoiceTypeInfo: GetRemoteInvoiceTypeInfo,
            GetRemoteInvoiceFieldsInfo: GetRemoteInvoiceFieldsInfo,
            GetRemoteInvoiceTypeCustomFieldsInfo: GetRemoteInvoiceTypeCustomFieldsInfo
        });
    }

    appControllers.service('PartnerPortal_Invoice_InvoiceTypeAPIService', invoiceTypeAPIService);

})(appControllers);