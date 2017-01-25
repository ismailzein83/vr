
app.service('VR_Invoice_PartnerInvoiceSettingService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {

        function addPartnerInvoiceSetting(onPartnerInvoiceSettingAdded, invoiceSettingId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPartnerInvoiceSettingAdded = onPartnerInvoiceSettingAdded;
            };
            var parameters = {
                invoiceSettingId: invoiceSettingId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/PartnerInvoiceSettingEditor.html', parameters, settings);
        }
        function editPartnerInvoiceSetting(onPartnerInvoiceSettingUpdated,partnerInvoiceSettingId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPartnerInvoiceSettingUpdated = onPartnerInvoiceSettingUpdated;
            };
            var parameters = {
                partnerInvoiceSettingId: partnerInvoiceSettingId,
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/PartnerInvoiceSettingEditor.html', parameters, settings);
        }

        return ({
            addPartnerInvoiceSetting: addPartnerInvoiceSetting,
            editPartnerInvoiceSetting: editPartnerInvoiceSetting,
        });
    }]);
