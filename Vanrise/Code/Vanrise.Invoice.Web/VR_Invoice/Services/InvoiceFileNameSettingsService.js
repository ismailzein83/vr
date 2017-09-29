
app.service('VR_Invoice_InvoiceFileSettingsService', ['VRModalService',
    function (VRModalService) {

        function addFileNamePart(onFileNamePartAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileNamePartAdded = onFileNamePartAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FileNamePartEditor.html', parameters, settings);
        }

        function editFileNamePart(fileNamePartEntity, onFileNamePartUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileNamePartUpdated = onFileNamePartUpdated;
            };
            var parameters = {
                fileNamePartEntity: fileNamePartEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FileNamePartEditor.html', parameters, settings);
        }


        function addFileAttachment(onFileAttachmentAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileAttachmentAdded = onFileAttachmentAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FileAttachmentEditor.html', parameters, settings);
        }

        function editFileAttachment(fileAttachmentEntity, onFileAttachmentUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileAttachmentUpdated = onFileAttachmentUpdated;
            };
            var parameters = {
                fileAttachmentEntity: fileAttachmentEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/Templates/FileAttachmentEditor.html', parameters, settings);
        }


        return ({
            addFileNamePart: addFileNamePart,
            editFileNamePart: editFileNamePart,
            addFileAttachment:addFileAttachment,
            editFileAttachment: editFileAttachment

        });
    }]);
