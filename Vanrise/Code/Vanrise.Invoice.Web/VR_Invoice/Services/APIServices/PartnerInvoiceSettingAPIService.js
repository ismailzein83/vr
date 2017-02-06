(function (appControllers) {

    "use strict";
    partnerInvoiceSettingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function partnerInvoiceSettingAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'PartnerInvoiceSetting';

        function GetPartnerInvoiceSetting(partnerInvoiceSettingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetPartnerInvoiceSetting"), {
                partnerInvoiceSettingId: partnerInvoiceSettingId
            });
        }
        function GetFilteredPartnerInvoiceSettings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredPartnerInvoiceSettings"), input);
        }
        function AddPartnerInvoiceSetting(partnerInvoiceSetting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "AddPartnerInvoiceSetting"), partnerInvoiceSetting);
        }
        function UpdatePartnerInvoiceSetting(partnerInvoiceSetting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "UpdatePartnerInvoiceSetting"), partnerInvoiceSetting);
        }
        function DeletePartnerInvoiceSetting(partnerInvoiceSettingId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "DeletePartnerInvoiceSetting"), {
                partnerInvoiceSettingId: partnerInvoiceSettingId
            });
        }
        return ({
            GetPartnerInvoiceSetting: GetPartnerInvoiceSetting,
            GetFilteredPartnerInvoiceSettings: GetFilteredPartnerInvoiceSettings,
            AddPartnerInvoiceSetting: AddPartnerInvoiceSetting,
            UpdatePartnerInvoiceSetting: UpdatePartnerInvoiceSetting,
            DeletePartnerInvoiceSetting: DeletePartnerInvoiceSetting
        });
    }

    appControllers.service('VR_Invoice_PartnerInvoiceSettingAPIService', partnerInvoiceSettingAPIService);

})(appControllers);