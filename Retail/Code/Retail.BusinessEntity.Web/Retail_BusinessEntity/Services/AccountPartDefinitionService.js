(function (appControllers) {

    'use stict';

    AccountPartDefinitionService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountPartDefinitionService(VRModalService, VRNotificationService) {
        function addAccountPartDefinition(onAccountPartDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartDefinitionAdded = onAccountPartDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountPartDefinition/AccountPartDefinitionEditor.html', null, settings);
        };

        function editAccountPartDefinition(accountPartDefinitionId, onAccountPartDefinitionUpdated) {
            var parameters = {
                accountPartDefinitionId: accountPartDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartDefinitionUpdated = onAccountPartDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountPartDefinition/AccountPartDefinitionEditor.html', parameters, settings);
        };
        return {
            addAccountPartDefinition: addAccountPartDefinition,
            editAccountPartDefinition: editAccountPartDefinition,
        };
    }

    appControllers.service('Retail_BE_AccountPartDefinitionService', AccountPartDefinitionService);

})(appControllers);