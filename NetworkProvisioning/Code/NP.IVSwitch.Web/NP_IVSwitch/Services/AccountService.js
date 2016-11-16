
(function (appControllers) {

    "use strict";

    AccountService.$inject = ['VRModalService'];

    function AccountService(NPModalService) {

        function addAccount(onAccountAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Account/AccountEditor.html', null, settings);
        };
        function editAccount(AccountId, onAccountUpdated) {
            var settings = {};

            var parameters = {
                AccountId: AccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountUpdated = onAccountUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Account/AccountEditor.html', parameters, settings);
        }


        return {
            addAccount: addAccount,
            editAccount: editAccount
        };
    }

    appControllers.service('NP_IVSwitch_AccountService', AccountService);

})(appControllers);