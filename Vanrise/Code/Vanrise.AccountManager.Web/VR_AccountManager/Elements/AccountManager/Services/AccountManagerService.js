(function (appControllers) {

    'use strict';

    AccountManagerService.$inject = ['VRModalService'];

    function AccountManagerService(VRModalService)
    {

        return ({
            addNewAssignmentDefinition: addNewAssignmentDefinition,
            editAssignmentDefinition: editAssignmentDefinition,
            addNewAccountManager: addNewAccountManager,
            editAccountmanager: editAccountmanager
        });
        function addNewAssignmentDefinition(onAssignmentDefinitionAdded)
       {
            var settings = {
            };

            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionAdded = onAssignmentDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }
        function addNewAccountManager(onAccountManagerAdded, accountDefinitionId) {
            var settings = {
            };

            var parameters = {
                AccountManagerDefinitionId: accountDefinitionId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerAdded = onAccountManagerAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAccountmanager(accountManagerObject, onAccountManagerUpdated) {
            var settings = {
            };
            var parameters = {
                UserID: accountManagerObject.UserId,
                UserName: accountManagerObject.UserName,
                AccountManagerDefinitionId: accountManagerObject.AccountManagerDefinitionId,
                AccountManagerId:accountManagerObject.AccountManagerId

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountManagerUpdated = onAccountManagerUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AccountManagerEditor.html', parameters, settings);
        }
        function editAssignmentDefinition(assignmentDefinitionEntity, onAssignmentDefinitionUpdated) {
            var parameters = {
                assignmentDefinitionEntity: assignmentDefinitionEntity
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionUpdated = onAssignmentDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountManager/Elements/AccountManager/Views/AssignmentDefinitionEditor.html', parameters, settings);
        }

    }

    appControllers.service('VR_AccountManager_AccountManagerService', AccountManagerService);

})(appControllers);
