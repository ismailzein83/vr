(function (appControllers) {

    'use stict';

    AccountServiceService.$inject = ['VRModalService'];

    function AccountServiceService(VRModalService) {
        return ({
            addAccountService: addAccountService,
            editAccountService: editAccountService,
        });

        function addAccountService(accountBEDefinitionId, accountId, onAccountServiceAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };

            var settings = {
            };
            
            settings.onScopeReady = function (modalScope) {

                modalScope.onAccountServiceAdded = onAccountServiceAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountService/AccountServiceEditor.html', parameters, settings);
        }

        function editAccountService(accountServiceId, accountBEDefinitionId, onAccountServiceUpdated) {

            var parameters = {
                accountServiceId: accountServiceId,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountServiceUpdated = onAccountServiceUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountService/AccountServiceEditor.html', parameters, modalSettings);
        }

    }

    appControllers.service('Retail_BE_AccountServiceService', AccountServiceService);

})(appControllers);
