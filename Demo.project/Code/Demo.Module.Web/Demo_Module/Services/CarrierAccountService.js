(function (appControllers) {

    'use strict';

    OperatorAccountService.$inject = ['VRModalService'];

    function OperatorAccountService(VRModalService) {
        return ({
            addOperatorAccount: addOperatorAccount,
            editOperatorAccount: editOperatorAccount
        });

        function addOperatorAccount(onOperatorAccountAdded, dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onOperatorAccountAdded = onOperatorAccountAdded;
            };
            var parameters;
            if (dataItem != undefined) {
                parameters = {
                    OperatorProfileId: dataItem.OperatorProfileId,
                };
            }
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorAccount/OperatorAccountEditor.html', parameters, settings);
        }

        function editOperatorAccount(operatorAccountObj, onOperatorAccountUpdated) {
            var modalSettings = {
            };
            var parameters = {
                OperatorAccountId: operatorAccountObj.OperatorAccountId != undefined?operatorAccountObj.OperatorAccountId:operatorAccountObj,
                OperatorProfileId: operatorAccountObj.OperatorProfileId != undefined? operatorAccountObj.OperatorProfileId:undefined,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorAccountUpdated = onOperatorAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Views/OperatorAccount/OperatorAccountEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('Demo_OperatorAccountService', OperatorAccountService);

})(appControllers);
