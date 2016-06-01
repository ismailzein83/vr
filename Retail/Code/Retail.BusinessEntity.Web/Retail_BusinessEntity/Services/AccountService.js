(function (appControllers) {

    'use stict';

    AccountService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountService(VRModalService, VRNotificationService)
    {
        function addAccount(parentAccountId, onAccountAdded)
        {
            var parameters = {
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        function editAccount(accountId, parentAccountId, onAccountUpdated)
        {
            var parameters = {
                accountId: accountId,
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountUpdated = onAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        return {
            addAccount: addAccount,
            editAccount: editAccount
        };
    }

    appControllers.service('Retail_BE_AccountService', AccountService);

})(appControllers);