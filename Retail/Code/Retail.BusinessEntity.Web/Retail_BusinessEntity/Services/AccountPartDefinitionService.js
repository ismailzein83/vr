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
        
        function addAccountPartGenericEditorItemDefinition(parameters, onAccountPartGenericEditorItemDefinitionAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartGenericEditorItemDefinitionAdded = onAccountPartGenericEditorItemDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeGenericEditorPartDefinitionItemTemplate.html', parameters, settings);
        }

        function editAccountPartGenericEditorItemDefinition(parameters, onAccountPartGenericEditorItemDefinitionUpdated) {
            
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountPartGenericEditorItemDefinitionUpdated = onAccountPartGenericEditorItemDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeGenericEditorPartDefinitionItemTemplate.html', parameters, settings);
        }

        return {
            addAccountPartDefinition: addAccountPartDefinition,
            editAccountPartDefinition: editAccountPartDefinition,
            addAccountPartGenericEditorItemDefinition: addAccountPartGenericEditorItemDefinition,
            editAccountPartGenericEditorItemDefinition: editAccountPartGenericEditorItemDefinition
        };
    }

    appControllers.service('Retail_BE_AccountPartDefinitionService', AccountPartDefinitionService);

})(appControllers);