"use strict";
app.service('VR_Invoice_InvoiceEmailActionService', ['VRModalService',
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceActions/InvoiceEmailAttachmentEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceActions/InvoiceEmailAttachmentEditor.html', parameters, settings);
        }

        return ({
            addEmailAttachment: addEmailAttachment,
            editEmailAttachment: editEmailAttachment,
        });
    }]);
