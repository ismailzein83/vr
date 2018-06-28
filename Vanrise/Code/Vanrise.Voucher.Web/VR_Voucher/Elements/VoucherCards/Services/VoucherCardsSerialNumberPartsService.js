
app.service('VR_Voucher_VoucherCardsSerialNumberPartsService', ['VRModalService',
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
            VRModalService.showModal('/Client/Modules/VR_Voucher/Elements/VoucherCards/Directives/Templates/SerialNumberPartEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Voucher/Elements/VoucherCards/Directives/Templates/SerialNumberPartEditor.html', parameters, settings);
        }

        return ({
            addSerialNumberPart: addSerialNumberPart,
            editSerialNumberPart: editSerialNumberPart

        });
    }]);
