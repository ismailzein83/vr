(function (appControllers) {

    'use stict';

    AccountSynchronizerHandlerService.$inject = ['VRModalService'];

    function AccountSynchronizerHandlerService(VRModalService) {

        function addAccountSynchronizerHandler(accountBEDefinitionId, onAccountSynchronizerHandlerAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountSynchronizerHandlerAdded = onAccountSynchronizerHandlerAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountSynchronizerHandler/Templates/AccountSynchronizerInsertHandlerEditor.html', parameters, settings);
        };
        function editAccountSynchronizerHandler(accountSynchronizerInsertHandler, accountBEDefinitionId, onAccountSynchronizerHandlerUpdated) {

            var parameters = {
                accountSynchronizerInsertHandler: accountSynchronizerInsertHandler,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountSynchronizerHandlerUpdated = onAccountSynchronizerHandlerUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/AccountSynchronizerHandler/Templates/AccountSynchronizerInsertHandlerEditor.html', parameters, settings);
        }

        return {
            addAccountSynchronizerHandler: addAccountSynchronizerHandler,
            editAccountSynchronizerHandler: editAccountSynchronizerHandler
        };
    }

    appControllers.service('Retail_BE_AccountSynchronizerHandlerService', AccountSynchronizerHandlerService);

})(appControllers);