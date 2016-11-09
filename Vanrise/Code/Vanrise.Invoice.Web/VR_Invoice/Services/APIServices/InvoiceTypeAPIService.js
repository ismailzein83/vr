﻿(function (appControllers) {

    "use strict";
    invoiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetInvoiceType(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceType"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetInvoiceTypeRuntime(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypeRuntime"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        
        function GetFilteredInvoiceTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoiceTypes"), input);
        }

        function GetInvoiceGeneratorConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGeneratorConfigs"));
        }
        function GetInvoiceGridActionSettingsConfigs() {
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
        function AddInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "AddInvoiceType"), invoiceType);
        }
        function UpdateInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "UpdateInvoiceType"), invoiceType);
        }
        function CovertToGridColumnAttribute(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "CovertToGridColumnAttribute"), input);
        }
        function GetInvoiceTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypesInfo"), { filter: filter });
        }

        function GetInvoicePartnerSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoicePartnerSettingsConfigs"));
        }
        function GetItemsFilterConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetItemsFilterConfigs"));
        }
        function GetInvoiceFilterConditionConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceFilterConditionConfigs"));

        }
        return ({
            GetInvoiceType: GetInvoiceType,
            GetFilteredInvoiceTypes: GetFilteredInvoiceTypes,
            GetInvoiceGeneratorConfigs: GetInvoiceGeneratorConfigs,
            GetInvoiceGridActionSettingsConfigs: GetInvoiceGridActionSettingsConfigs,
            GetRDLCDataSourceSettingsConfigs: GetRDLCDataSourceSettingsConfigs,
            GetRDLCParameterSettingsConfigs: GetRDLCParameterSettingsConfigs,
            AddInvoiceType: AddInvoiceType,
            UpdateInvoiceType: UpdateInvoiceType,
            GetInvoiceTypeRuntime: GetInvoiceTypeRuntime,
            GetInvoiceUISubSectionSettingsConfigs: GetInvoiceUISubSectionSettingsConfigs,
            CovertToGridColumnAttribute: CovertToGridColumnAttribute,
            GetInvoiceTypesInfo: GetInvoiceTypesInfo,
            GetInvoicePartnerSettingsConfigs: GetInvoicePartnerSettingsConfigs,
            GetItemsFilterConfigs: GetItemsFilterConfigs,
            GetInvoiceFilterConditionConfigs: GetInvoiceFilterConditionConfigs
        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeAPIService', invoiceTypeAPIService);

})(appControllers);