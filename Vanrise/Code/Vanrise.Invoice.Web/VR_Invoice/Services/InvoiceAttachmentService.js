"use strict";
app.service('VR_Invoice_InvoiceAttachmentService', ['VRModalService',
    function (VRModalService) {

        function addAttachment(onAttachmentAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAttachmentAdded = onAttachmentAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceAttachments/Templates/InvoiceAttachmentEditor.html', parameters, settings);
        }
        function editAttachment(attachmentEntity, onAttachmentUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAttachmentUpdated = onAttachmentUpdated;
            };
            var parameters = {
                attachmentEntity: attachmentEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceAttachments/Templates/InvoiceAttachmentEditor.html', parameters, settings);
        }
       
        return ({
            addAttachment: addAttachment,
            editAttachment: editAttachment,
          
        });
    }]);
