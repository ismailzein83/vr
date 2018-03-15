(function (appControllers) {

    'use strict';

    aNumberSaleCodeService.$inject = ['VRModalService', 'VRNotificationService'];

    function aNumberSaleCodeService(VRModalService, VRNotificationService) {

        function addANumberSaleCodes(aNumberGroupId, onANumberSaleCodesAdded) {
            var settings = {
            };

            var parameters = {
                aNumberGroupId: aNumberGroupId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onANumberSaleCodesAdded = onANumberSaleCodesAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSaleCodeEditor.html', parameters, settings);
        }

        return ({
            addANumberSaleCodes: addANumberSaleCodes
        });
    }

    appControllers.service('WhS_BE_ANumberSaleCodeService', aNumberSaleCodeService);

})(appControllers);
