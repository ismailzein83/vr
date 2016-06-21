(function (appControllers) {

    'use stict';

    AccountTypeService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountTypeService(VRModalService, VRNotificationService)
    {
        function addAccountType(onAccountTypeAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountTypeAdded = onAccountTypeAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountType/AccountTypeEditor.html', null, settings);
        };

        function editAccountType(accountTypeId, onAccountTypeUpdated)
        {
            var parameters = {
                accountTypeId: accountTypeId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountTypeUpdated = onAccountTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountType/AccountTypeEditor.html', parameters, settings);
        };

        return {
            addAccountType: addAccountType,
            editAccountType: editAccountType,

        };
    }

    appControllers.service('Retail_BE_AccountTypeService', AccountTypeService);

})(appControllers);