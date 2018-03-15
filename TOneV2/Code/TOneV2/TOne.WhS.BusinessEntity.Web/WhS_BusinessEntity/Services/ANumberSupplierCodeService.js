(function (appControllers) {

    'use strict';

    aNumberSupplierCodeService.$inject = ['VRModalService', 'VRNotificationService'];

    function aNumberSupplierCodeService(VRModalService, VRNotificationService) {

        function addANumberSupplierCodes(aNumberGroupId, onANumberSupplierCodesAdded) {
            var settings = {
            };

            var parameters = {
                aNumberGroupId: aNumberGroupId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onANumberSupplierCodesAdded = onANumberSupplierCodesAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSupplierCodeEditor.html', parameters, settings);
        }

        return ({
            addANumberSupplierCodes: addANumberSupplierCodes
        });
    }

    appControllers.service('WhS_BE_ANumberSupplierCodeService', aNumberSupplierCodeService);

})(appControllers);
