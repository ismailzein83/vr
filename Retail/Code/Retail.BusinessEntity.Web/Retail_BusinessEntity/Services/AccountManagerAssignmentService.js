(function (appControllers) {

    'use stict';

    AccountManagerAssignmentService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountManagerAssignmentService(VRModalService, VRNotificationService) {

        function addAccountManagerAssignments(accountManagerDefinitionId, accountManagerId, accountManagerAssignementDefinitionId, onAccountManagerAssignmentAdded) {
            var parameters = {
                accountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                accountManagerDefinitionId: accountManagerDefinitionId,
                accountManagerId: accountManagerId,
            };
           
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentAdded = onAccountManagerAssignmentAdded;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditor.html', parameters, settings);
        }
        function editAccountManagerAssignment(accountManagerAssignmentId, onAccountManagerAssignmentUpdated, accountManagerDefinitionId, accountManagerId, accountManagerAssignementDefinitionId) {
            var modalSettings = {
            };
            var parameters = {
                accountManagerAssignementDefinitionId: accountManagerAssignementDefinitionId,
                accountManagerDefinitionId: accountManagerDefinitionId,
                accountManagerId: accountManagerId,
                accountManagerAssignmentId: accountManagerAssignmentId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAssignmentUpdated = onAccountManagerAssignmentUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountManager/AccountManagerAssignmentEditor.html', parameters, modalSettings);
        }

        return {
            addAccountManagerAssignments: addAccountManagerAssignments,
            editAccountManagerAssignment: editAccountManagerAssignment
        };
    }

    appControllers.service('Retail_BE_AccountManagerAssignmentService', AccountManagerAssignmentService);

})(appControllers);