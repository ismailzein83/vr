(function (appControllers) {

    'use stict';

    AccountServiceService.$inject = ['VRModalService'];

    function AccountServiceService(VRModalService) {
        return ({
            addAccountService: addAccountService,
            editAccountService: editAccountService,
        });

        function addAccountService(onAccountServiceAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onAccountServiceAdded = onAccountServiceAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountService/AccountServiceEditor.html', null, settings);
        }

        function editAccountService(accountServiceId, onAccountServiceUpdated) {
            var modalSettings = {
            };

            var parameters = {
                accountServiceId: accountServiceId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountServiceUpdated = onAccountServiceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountService/AccountServiceEditor.html', parameters, modalSettings);
        }

    }

    appControllers.service('Retail_BE_AccountServiceService', AccountServiceService);

})(appControllers);
