﻿(function (appControllers) {

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
        function GetInvoiceGridFilterConditionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGridFilterConditionConfigs"));

        }
        function GetInvoiceExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceExtendedSettingsConfigs"));
        }
        function GetInvoiceGeneratorFilterConditionConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGeneratorFilterConditionConfigs"));
        }
        function GetBillingPeriodTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetBillingPeriodTemplateConfigs"));
        }
        function GetStartDateCalculationMethodConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetStartDateCalculationMethodConfigs"));
        }
        function GetInvoiceSettingPartsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceSettingPartsConfigs"));
        }
        function GetInvoiceBulkActionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceBulkActionSettingsConfigs"));
        }
        function GetItemSetNameStorageRuleConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetItemSetNameStorageRuleConfigs"));
        }
        function GetInvoiceUIGridColumnFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceUIGridColumnFilterConfigs"));
        }

        function GetInvoiceSubSectionFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceSubSectionFilterConfigs"));
        }
        return ({
            GetInvoiceActionSettingsConfigs: GetInvoiceActionSettingsConfigs,
            GetRDLCDataSourceSettingsConfigs: GetRDLCDataSourceSettingsConfigs,
            GetRDLCParameterSettingsConfigs: GetRDLCParameterSettingsConfigs,
            GetInvoiceUISubSectionSettingsConfigs: GetInvoiceUISubSectionSettingsConfigs,
            GetItemsFilterConfigs: GetItemsFilterConfigs,
            GetInvoiceGridFilterConditionConfigs: GetInvoiceGridFilterConditionConfigs,
            GetInvoiceExtendedSettingsConfigs: GetInvoiceExtendedSettingsConfigs,
            GetInvoiceGeneratorFilterConditionConfigs: GetInvoiceGeneratorFilterConditionConfigs,
            GetBillingPeriodTemplateConfigs: GetBillingPeriodTemplateConfigs,
            GetStartDateCalculationMethodConfigs: GetStartDateCalculationMethodConfigs,
            GetInvoiceSettingPartsConfigs: GetInvoiceSettingPartsConfigs,
            GetInvoiceBulkActionSettingsConfigs: GetInvoiceBulkActionSettingsConfigs,
            GetItemSetNameStorageRuleConfigs: GetItemSetNameStorageRuleConfigs,
            GetInvoiceUIGridColumnFilterConfigs: GetInvoiceUIGridColumnFilterConfigs,
            GetInvoiceSubSectionFilterConfigs: GetInvoiceSubSectionFilterConfigs

        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeConfigsAPIService', invoiceTypeAPIService);

})(appControllers);