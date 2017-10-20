(function (appControllers) {

    'use strict';

    AccountManagerService.$inject = ['VRModalService'];

    function AccountManagerService(VRModalService)
    {

        return ({
            addNewAssignmentDefinition: addNewAssignmentDefinition,
            editAssignmentDefinition: editAssignmentDefinition
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
