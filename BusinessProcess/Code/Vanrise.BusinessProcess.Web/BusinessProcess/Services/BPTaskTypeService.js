(function (appControllers) {

    "use strict";
    BusinessProcess_BPTaskTypeService.$inject = [ 'VRModalService','BusinessProcess_BPTaskTypeAPIService'];
    function BusinessProcess_BPTaskTypeService(VRModalService, BusinessProcess_BPTaskTypeAPIService) {

        function addBPTaskType(onTaskTypeAdded) {
            var modalParameters = {
                businessEntityDefinitionId: "d33fd65a-721f-4ae1-9d41-628be9425796"
            };

            var modalSettings = {
                size:"medium"
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEAdded = onTaskTypeAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', modalParameters, modalSettings);
        }

        function addTaskTypeAction(onBPTaskTypeActionAdded) {
            var modalParameters = {};

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBPTaskTypeActionAdded = onBPTaskTypeActionAdded;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPGenericTaskType/Templates/BPGenericTaskTypeActionEditorTemplate.html', modalParameters, modalSettings);
        }

        function editTaskTypeAction(taskTypeActionEntity, onBPTaskTypeActionUpdated) {
            var modalParameters = {
                taskTypeActionEntity: taskTypeActionEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBPTaskTypeActionUpdated = onBPTaskTypeActionUpdated;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPGenericTaskType/Templates/BPGenericTaskTypeActionEditorTemplate.html', modalParameters, modalSettings);
        }

        return {
            addBPTaskType: addBPTaskType,
            addTaskTypeAction: addTaskTypeAction,
            editTaskTypeAction: editTaskTypeAction
        };
    }
    appControllers.service('BusinessProcess_BPTaskTypeService', BusinessProcess_BPTaskTypeService);

})(appControllers);