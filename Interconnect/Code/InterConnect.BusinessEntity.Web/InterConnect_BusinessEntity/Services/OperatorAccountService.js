(function (appControllers) {

    'use stict';

    OperatorAccountService.$inject = ['VRModalService'];

    function OperatorAccountService(VRModalService) {
        return ({
            addOperatorAccount: addOperatorAccount,
            editOperatorAccount: editOperatorAccount
        });

        function addOperatorAccount(onOperatorAccountAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onOperatorAccountAdded = onOperatorAccountAdded;
            };

            VRModalService.showModal('/Client/Modules/InterConnect_BusinessEntity/Views/OperatorAccount/OperatorAccountEditor.html', null, settings);
        }

        function editOperatorAccount(OperatorAccountObj, onOperatorAccountUpdated) {
            var modalSettings = {
            };

            var parameters = {
                OperatorAccountId: OperatorAccountObj.OperatorAccountId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOperatorAccountUpdated = onOperatorAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/InterConnect_BusinessEntity/Views/OperatorAccount/OperatorAccountEditor.html', parameters, modalSettings);
        }
    }

    appControllers.service('InterConnect_BE_OperatorAccountService', OperatorAccountService);

})(appControllers);
