(function (appControllers) {

    'use stict';

    AccountManagerAssignmentService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountManagerAssignmentService(VRModalService, VRNotificationService) {

        function addAccountManagerAssignments(subViewDefinitionEntity, onAccountManagerAssignmentAdded) {
            var parameters = {
                accountManagerAssignementDefinitionId: subViewDefinitionEntity.accountManagerSubViewDefinition.Settings.AccountManagerAssignementDefinitionId,
                accountManagerDefinitionId: subViewDefinitionEntity.accountManagerDefinitionId,
                accountManagerId: subViewDefinitionEntity.accountManagerId,
            };
           
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentAdded = onAccountManagerAssignmentAdded;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditorTemplate.html', parameters, settings);
        }
        function editAccountManagerAssignment(accountManagerAssignmentId, onAccountManagerAssignmentUpdated, subViewDefinitionEntity) {
            var modalSettings = {
            };
            var parameters = {
                accountManagerAssignementDefinitionId: subViewDefinitionEntity.accountManagerSubViewDefinition.Settings.AccountManagerAssignementDefinitionId,
                accountManagerDefinitionId: subViewDefinitionEntity.accountManagerDefinitionId,
                accountManagerId: subViewDefinitionEntity.accountManagerId,
                accountManagerAssignmentId: accountManagerAssignmentId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentUpdated = onAccountManagerAssignmentUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditorTemplate.html', parameters, modalSettings);
        }

        return {
            addAccountManagerAssignments: addAccountManagerAssignments,
            editAccountManagerAssignment: editAccountManagerAssignment
        };
    }

    appControllers.service('Retail_BE_AccountManagerAssignmentService', AccountManagerAssignmentService);

})(appControllers);