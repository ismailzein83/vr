"use strict";
app.service('VR_Invoice_InvoiceGenerationActionService', ['VRModalService',
    function (VRModalService) {

        function addEmailAttachment(onEmailAttachmentAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmailAttachmentAdded = onEmailAttachmentAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/Templates/InvoiceEmailAttachmentEditor.html', parameters, settings);
        }
        function editEmailAttachment(emailAttachmentEntity,onEmailAttachmentUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmailAttachmentUpdated = onEmailAttachmentUpdated;
            };
            var parameters = {
                emailAttachmentEntity: emailAttachmentEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/Templates/InvoiceEmailAttachmentEditor.html', parameters, settings);
        }

        function addEmailAttachmentSet(onEmailAttachmentSetAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmailAttachmentSetAdded = onEmailAttachmentSetAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/EmailActionAttachmentSetEditor.html', parameters, settings);
        }
        function editEmailAttachmentSet(emailAttachmentSetEntity, onEmailAttachmentSetUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmailAttachmentSetUpdated = onEmailAttachmentSetUpdated;
            };
            var parameters = {
                emailAttachmentSetEntity: emailAttachmentSetEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/EmailActionAttachmentSetEditor.html', parameters, settings);
        }

        function addInvoiceToFileActionSet(onInvoiceToFileActionSetAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceToFileActionSetAdded = onInvoiceToFileActionSetAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/SaveInvoiceToFileActionSetEditor.html', parameters, settings);
        }
        function editInvoiceToFileActionSet(invoiceToFileActionSetEntity, onInvoiceToFileActionSetUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceToFileActionSetUpdated = onInvoiceToFileActionSetUpdated;
            };
            var parameters = {
                invoiceToFileActionSetEntity: invoiceToFileActionSetEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/SaveInvoiceToFileActionSetEditor.html', parameters, settings);
        }

        return ({
            addEmailAttachment: addEmailAttachment,
            editEmailAttachment: editEmailAttachment,
            addEmailAttachmentSet: addEmailAttachmentSet,
            editEmailAttachmentSet: editEmailAttachmentSet,
            editInvoiceToFileActionSet: editInvoiceToFileActionSet,
            addInvoiceToFileActionSet: addInvoiceToFileActionSet
        });
    }]);
