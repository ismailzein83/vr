(function (appControllers) {

    'use strict';

    BankDetailService.$inject = ['VRModalService'];

    function BankDetailService(VRModalService) {
        return {
            addBankDetail: addBankDetail,
            editBankDetail: editBankDetail
        };

        function addBankDetail(onBankDetailsAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBankDetailsAdded = onBankDetailsAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/BankDetails/BankDetailsEditor.html', modalParameters, modalSettings);
        }

        function editBankDetail(bankDetailEntity, onBankDetailsUpdated) {
            var modalParameters = {
                bankDetailEntity: bankDetailEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBankDetailsUpdated = onBankDetailsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/BankDetails/BankDetailsEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_BankDetailService', BankDetailService);

})(appControllers);