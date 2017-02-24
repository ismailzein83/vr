(function (appControllers) {

    'use strict';

    InvoiceViewerTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PartnerPortal_Invoice_ModuleConfig', 'SecurityService'];

    function InvoiceViewerTypeAPIService(BaseAPIService, UtilsService, PartnerPortal_Invoice_ModuleConfig, SecurityService) {
        var controllerName = 'InvoiceViewerType';

        function GetInvoiceViewerTypeInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceViewerTypeInfo"), {
                serializedFilter: serializedFilter
            });
        };
        function GetInvoiceViewerType(invoiceViewerTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceViewerType"), {
                invoiceViewerTypeId: invoiceViewerTypeId
            });
        };
        function GetInvoiceViewerTypeRuntime(invoiceViewerTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceViewerTypeRuntime"), {
                invoiceViewerTypeId: invoiceViewerTypeId
            });
        };
        function GetInvoiceQueryInterceptorTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceQueryInterceptorTemplates"));
        };
        function GetInvoiceGridActionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(PartnerPortal_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGridActionSettingsConfigs"));
        };
        return {
            GetInvoiceViewerTypeInfo: GetInvoiceViewerTypeInfo,
            GetInvoiceViewerType: GetInvoiceViewerType,
            GetInvoiceViewerTypeRuntime: GetInvoiceViewerTypeRuntime,
            GetInvoiceQueryInterceptorTemplates: GetInvoiceQueryInterceptorTemplates,
            GetInvoiceGridActionSettingsConfigs:GetInvoiceGridActionSettingsConfigs
        };
    }

    appControllers.service('PartnerPortal_Invoice_InvoiceViewerTypeAPIService', InvoiceViewerTypeAPIService);

})(appControllers);