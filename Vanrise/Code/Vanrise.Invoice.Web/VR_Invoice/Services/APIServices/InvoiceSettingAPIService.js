(function (appControllers) {

    "use strict";
    invoiceSettingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceSettingAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceSetting';

        function GetInvoiceSetting(invoiceSettingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceSetting"), {
                invoiceSettingId: invoiceSettingId
            });
        }
        function GetFilteredInvoiceSettings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoiceSettings"), input);
        }
        function AddInvoiceSetting(invoiceSetting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "AddInvoiceSetting"), invoiceSetting);
        }
        function HasAddInvoiceSettingPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Invoice_ModuleConfig.moduleName, controllerName, ['AddInvoiceSetting']));
        }
        function UpdateInvoiceSetting(invoiceSetting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "UpdateInvoiceSetting"), invoiceSetting);
        }
        function HasUpdateInvoiceSettingPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Invoice_ModuleConfig.moduleName, controllerName, ['UpdateInvoiceSetting']));
        }
        function GetInvoiceSettingsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceSettingsInfo"), { filter: filter });
        }
        function SetInvoiceSettingDefault(invoiceSettingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "SetInvoiceSettingDefault"), {
                invoiceSettingId: invoiceSettingId
            });
        }
        return ({
            GetInvoiceSetting: GetInvoiceSetting,
            GetFilteredInvoiceSettings: GetFilteredInvoiceSettings,
            AddInvoiceSetting: AddInvoiceSetting,
            HasAddInvoiceSettingPermission: HasAddInvoiceSettingPermission,
            UpdateInvoiceSetting: UpdateInvoiceSetting,
            HasUpdateInvoiceSettingPermission: HasUpdateInvoiceSettingPermission,
            GetInvoiceSettingsInfo: GetInvoiceSettingsInfo,
            SetInvoiceSettingDefault: SetInvoiceSettingDefault
        });
    }

    appControllers.service('VR_Invoice_InvoiceSettingAPIService', invoiceSettingAPIService);

})(appControllers);