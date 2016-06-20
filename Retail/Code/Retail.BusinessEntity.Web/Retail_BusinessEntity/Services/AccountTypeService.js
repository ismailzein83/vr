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

        function addAccountPartDefinition(onAccountPartDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartDefinitionAdded = onAccountPartDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountType/AccountPartDefinitionEditor.html', null, settings);
        };

        function editAccountPartDefinition(accountPartDefinitionObj, onAccountPartDefinitionUpdated) {
            var parameters = {
                accountPartDefinitionEntity: accountPartDefinitionObj
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartDefinitionUpdated = onAccountPartDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountType/AccountPartDefinitionEditor.html', parameters, settings);
        };


        return {
            addAccountType: addAccountType,
            editAccountType: editAccountType,
            editAccountPartDefinition: editAccountPartDefinition,
            addAccountPartDefinition: addAccountPartDefinition
        };
    }

    appControllers.service('Retail_BE_AccountTypeService', AccountTypeService);

})(appControllers);