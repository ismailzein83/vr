
app.service('VR_Invoice_InvoiceFileNameSettingsService', ['VRModalService',
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNamePart/Templates/FileNamePartEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNamePart/Templates/FileNamePartEditor.html', parameters, settings);
        }

        return ({
            addFileNamePart: addFileNamePart,
            editFileNamePart: editFileNamePart

        });
    }]);
