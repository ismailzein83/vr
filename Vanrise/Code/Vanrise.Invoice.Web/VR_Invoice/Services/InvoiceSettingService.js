"use strict";
app.service('VR_Invoice_InvoiceSettingService', ['VRModalService','UtilsService',
    function (VRModalService, UtilsService) {

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
        function getEntityUniqueName(invoiceTypeId) {
            return "VR_Invoice_InvoiceSetting_" + invoiceTypeId;
        }
        function viewInvoiceSetting(invoiceSettingId,invoiceTypeId) {
            var parameters = {
                invoiceSettingId: invoiceSettingId,
                invoiceTypeId: invoiceTypeId
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/InvoiceSettingEditor.html', parameters, settings);
        }
        return ({
            addInvoiceSetting: addInvoiceSetting,
            editInvoiceSetting: editInvoiceSetting,
            getEntityUniqueName: getEntityUniqueName,
            viewInvoiceSetting: viewInvoiceSetting
        });
    }]);
