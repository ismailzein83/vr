(function (appControllers) {

    'use stict';

    AccountManagerAssignmentService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountManagerAssignmentService(VRModalService, VRNotificationService) {

        function addAccountManagerAssignments(accountManagerDefinitionId, accountManagerId, accountManagerAssignementDefinitionId, onAccountManagerAssignmentAdded,accountId) {
            var parameters = {
                accountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                accountManagerDefinitionId: accountManagerDefinitionId,
                accountManagerId: accountManagerId,
                accountId:accountId
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentAdded = onAccountManagerAssignmentAdded;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditor.html', parameters, settings);
        }
        function editAccountManagerAssignment(accountManagerAssignmentId, onAccountManagerAssignmentUpdated, accountManagerDefinitionId,  accountManagerAssignementDefinitionId, accountId) {
            var modalSettings = {
            };
            var parameters = {
                accountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                accountManagerDefinitionId: accountManagerDefinitionId,
                accountManagerAssignmentId: accountManagerAssignmentId,
                accountId:accountId
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentUpdated = onAccountManagerAssignmentUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditor.html', parameters, modalSettings);
        }
        function openAccountManagerAssignmentEditor(accountBEDefinitionId, accountId, accountActionDefinition, onItemUpdated) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                accountActionDefinition: accountActionDefinition,
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentUpdated = onItemUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditor.html', parameters, settings);
        }

        return {
            addAccountManagerAssignments: addAccountManagerAssignments,
            editAccountManagerAssignment: editAccountManagerAssignment,
            openAccountManagerAssignmentEditor: openAccountManagerAssignmentEditor
        };
    }

    appControllers.service('Retail_BE_AccountManagerAssignmentService', AccountManagerAssignmentService);

})(appControllers);