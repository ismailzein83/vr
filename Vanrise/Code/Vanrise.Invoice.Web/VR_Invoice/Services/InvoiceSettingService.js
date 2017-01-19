"use strict";
app.service('VR_Invoice_InvoiceSettingService', ['VRModalService',
    function (VRModalService) {

        function addInvoiceSetting(onInvoiceSettingAdded, invoiceTypeId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingAdded = onInvoiceSettingAdded;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/InvoiceSettingEditor.html', parameters, settings);
        }
        function editInvoiceSetting(onInvoiceSettingUpdated,invoiceSettingId, invoiceTypeId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceSettingUpdated = onInvoiceSettingUpdated;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId,
                invoiceSettingId: invoiceSettingId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/InvoiceSettingEditor.html', parameters, settings);
        }


        return ({
            addInvoiceSetting: addInvoiceSetting,
            editInvoiceSetting: editInvoiceSetting,
        });
    }]);
