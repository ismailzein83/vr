(function (appControllers) {

    "use strict";
    invoiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetInvoiceType(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceType"), {
                invoiceTypeId: invoiceTypeId
            });
        }

        function GetInvoiceTypeExtendedSettings(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypeExtendedSettings"), {
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
        function AddInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "AddInvoiceType"), invoiceType);
        }
        function HasAddInvoiceTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Invoice_ModuleConfig.moduleName, controllerName, ['AddInvoiceType']));
        }
        function UpdateInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "UpdateInvoiceType"), invoiceType);
        }
        function HasUpdateInvoiceTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Invoice_ModuleConfig.moduleName, controllerName, ['UpdateInvoiceType']));
        }
        function ConvertToGridColumnAttribute(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "ConvertToGridColumnAttribute"), input);
        }
        function GetInvoiceTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypesInfo"), { filter: filter });
        }
        function GetGeneratorInvoiceTypeRuntime(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetGeneratorInvoiceTypeRuntime"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetInvoiceGeneratorActions(generateInvoiceInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGeneratorActions"), generateInvoiceInput);
        }
        function GetInvoicePartnerSelector(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoicePartnerSelector"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetInvoiceAction(invoiceTypeId, invoiceActionId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceAction"), {
                invoiceTypeId: invoiceTypeId,
                invoiceActionId: invoiceActionId
            });
        }

        function GetMenualInvoiceBulkActionsDefinitions(invoiceTypeId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetMenualInvoiceBulkActionsDefinitions"), {
                invoiceTypeId: invoiceTypeId,
            });
        }
        function GetPartnerName(invoiceTypeId,partnerId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetPartnerName"), {
                invoiceTypeId: invoiceTypeId,
                partnerId: partnerId
            });
        }
        function GetPartnerInvoiceSettingFilterFQTN(invoiceTypeId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetPartnerInvoiceSettingFilterFQTN"), {
                invoiceTypeId: invoiceTypeId,
            });
        }
        return ({
            GetInvoiceType: GetInvoiceType,
            GetFilteredInvoiceTypes: GetFilteredInvoiceTypes,
            AddInvoiceType: AddInvoiceType,
            HasAddInvoiceTypePermission: HasAddInvoiceTypePermission,
            UpdateInvoiceType: UpdateInvoiceType,
            HasUpdateInvoiceTypePermission: HasUpdateInvoiceTypePermission,
            GetInvoiceTypeRuntime: GetInvoiceTypeRuntime,
            ConvertToGridColumnAttribute: ConvertToGridColumnAttribute,
            GetInvoiceTypesInfo: GetInvoiceTypesInfo,
            GetGeneratorInvoiceTypeRuntime: GetGeneratorInvoiceTypeRuntime,
            GetInvoiceGeneratorActions: GetInvoiceGeneratorActions,
            GetInvoicePartnerSelector: GetInvoicePartnerSelector,
            GetInvoiceAction: GetInvoiceAction,
            GetInvoiceTypeExtendedSettings: GetInvoiceTypeExtendedSettings,
            GetMenualInvoiceBulkActionsDefinitions: GetMenualInvoiceBulkActionsDefinitions,
            GetPartnerName: GetPartnerName,
            GetPartnerInvoiceSettingFilterFQTN: GetPartnerInvoiceSettingFilterFQTN
        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeAPIService', invoiceTypeAPIService);

})(appControllers);