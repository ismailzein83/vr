
app.service('VR_Invoice_InvoiceSerialNumberSettingsService', ['VRModalService',
    function (VRModalService) {

        function addSerialNumberPart(onSerialNumberPartAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSerialNumberPartAdded = onSerialNumberPartAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/Templates/SerialNumberPartEditor.html', parameters, settings);
        }

        function editSerialNumberPart(serialNumberPartEntity, onSerialNumberPartUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSerialNumberPartUpdated = onSerialNumberPartUpdated;
            };
            var parameters = {
                serialNumberPartEntity: serialNumberPartEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/Templates/SerialNumberPartEditor.html', parameters, settings);
        }

        return ({
            addSerialNumberPart: addSerialNumberPart,
            editSerialNumberPart: editSerialNumberPart

        });
    }]);
