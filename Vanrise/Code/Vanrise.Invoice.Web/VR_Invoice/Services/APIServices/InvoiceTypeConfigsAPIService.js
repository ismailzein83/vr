(function (appControllers) {

    "use strict";
    invoiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceTypeConfigs';
        function GetInvoiceActionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGridActionSettingsConfigs"));
        }
        function GetRDLCDataSourceSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetRDLCDataSourceSettingsConfigs"));
        }
        function GetRDLCParameterSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetRDLCParameterSettingsConfigs"));
        }
        function GetInvoiceUISubSectionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceUISubSectionSettingsConfigs"));
        }
        function GetItemsFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetItemsFilterConfigs"));
        }
        function GetInvoiceFilterConditionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceFilterConditionConfigs"));

        }
        function GetInvoiceExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceExtendedSettingsConfigs"));
        }

        return ({
            GetInvoiceActionSettingsConfigs: GetInvoiceActionSettingsConfigs,
            GetRDLCDataSourceSettingsConfigs: GetRDLCDataSourceSettingsConfigs,
            GetRDLCParameterSettingsConfigs: GetRDLCParameterSettingsConfigs,
            GetInvoiceUISubSectionSettingsConfigs: GetInvoiceUISubSectionSettingsConfigs,
            GetItemsFilterConfigs: GetItemsFilterConfigs,
            GetInvoiceFilterConditionConfigs: GetInvoiceFilterConditionConfigs,
            GetInvoiceExtendedSettingsConfigs: GetInvoiceExtendedSettingsConfigs
        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeConfigsAPIService', invoiceTypeAPIService);

})(appControllers);