
app.service('VR_Invoice_PartnerInvoiceSettingService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService','VR_Invoice_PartnerInvoiceSettingAPIService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, VR_Invoice_PartnerInvoiceSettingAPIService) {

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
        function deletePartnerInvoiceSetting(scope, partnerInvoiceSettingId, onPartnerInvoiceSettingDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                  
                    return VR_Invoice_PartnerInvoiceSettingAPIService.DeletePartnerInvoiceSetting(partnerInvoiceSettingId).then(function (deletionResponse) {
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
            deletePartnerInvoiceSetting: deletePartnerInvoiceSetting
        });
    }]);
