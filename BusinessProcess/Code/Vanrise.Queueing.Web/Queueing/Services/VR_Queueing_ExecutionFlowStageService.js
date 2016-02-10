(function (appControllers) {

    'use strict';

    ExecutionFlowStageService.$inject = ['VRModalService', 'VRNotificationService'];

    function ExecutionFlowStageService(VRModalService, VRNotificationService) {
        return ({
            addExecutionFlowStage: addExecutionFlowStage,
            editExecutionFlowStage: editExecutionFlowStage,
            deleteExecutionFlowStage: deleteExecutionFlowStage
        });

        function addExecutionFlowStage(onExecutionFlowStageAdded, existingFields) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowStageAdded = onExecutionFlowStageAdded;
            };
            var modalParameters = {
                ExistingFields: existingFields
            };
            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowStageEditor.html', modalParameters, modalSettings);
        }

        function editExecutionFlowStage(executionFlowStage, onExecutionFlowStageUpdated, existingFields) {
            var modalParameters = {
                ExecutionFlowStage: executionFlowStage,
                ExistingFields: existingFields
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowStageUpdated = onExecutionFlowStageUpdated;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowStageEditor.html', modalParameters, modalSettings);
        }

        function deleteExecutionFlowStage($scope, executionFlowStageObj, onExecutionFlowStageDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onExecutionFlowStageDeleted(executionFlowStageObj);
                    }
                });
        }

    };

    appControllers.service('VR_Queueing_ExecutionFlowStageService', ExecutionFlowStageService);

})(appControllers);
