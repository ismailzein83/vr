
app.service('VR_Invoice_PartnerInvoiceSettingService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService','VR_Invoice_PartnerInvoiceSettingAPIService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, VR_Invoice_PartnerInvoiceSettingAPIService) {

        function addPartnerInvoiceSetting(onPartnerInvoiceSettingAdded, invoiceTypeId,invoiceSettingId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPartnerInvoiceSettingAdded = onPartnerInvoiceSettingAdded;
            };
            var parameters = {
                invoiceSettingId: invoiceSettingId,
                invoiceTypeId: invoiceTypeId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/PartnerInvoiceSettingEditor.html', parameters, settings);
        }
        function editPartnerInvoiceSetting(onPartnerInvoiceSettingUpdated, invoiceTypeId,partnerInvoiceSettingId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPartnerInvoiceSettingUpdated = onPartnerInvoiceSettingUpdated;
            };
            var parameters = {
                partnerInvoiceSettingId: partnerInvoiceSettingId,
                invoiceTypeId: invoiceTypeId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/PartnerInvoiceSettingEditor.html', parameters, settings);
        }
        function deletePartnerInvoiceSetting(scope, partnerInvoiceSettingId, invoiceSettingId, onPartnerInvoiceSettingDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                  
                    return VR_Invoice_PartnerInvoiceSettingAPIService.DeletePartnerInvoiceSetting(partnerInvoiceSettingId, invoiceSettingId).then(function (deletionResponse) {
                        var deleted = VRNotificationService.notifyOnItemDeleted("Partner Invoice Setting", deletionResponse);
                        if (deleted && onPartnerInvoiceSettingDeleted != undefined) {
                            onPartnerInvoiceSettingDeleted();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
        return ({
            addPartnerInvoiceSetting: addPartnerInvoiceSetting,
            editPartnerInvoiceSetting: editPartnerInvoiceSetting,
            deletePartnerInvoiceSetting: deletePartnerInvoiceSetting,

        });
    }]);
