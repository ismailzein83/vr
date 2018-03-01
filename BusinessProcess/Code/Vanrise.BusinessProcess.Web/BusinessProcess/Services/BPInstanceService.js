(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService', 'BusinessProcess_BPInstanceAPIService', 'UtilsService'];
    function BusinessProcess_BPInstanceService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService, BusinessProcess_BPInstanceAPIService, UtilsService) {
        function getStatusColor(status) {

            if (status === BPInstanceStatusEnum.New.value) return LabelColorsEnum.New.color;
            if (status === BPInstanceStatusEnum.Postponed.value) return LabelColorsEnum.WarningLevel1.color;
            if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Processing.color;
            if (status === BPInstanceStatusEnum.Waiting.value) return LabelColorsEnum.WarningLevel1.color;
            if (status === BPInstanceStatusEnum.Cancelling.value) return LabelColorsEnum.WarningLevel2.color;
            if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Cancelled.value) return LabelColorsEnum.Error.color;

            return LabelColorsEnum.Info.color;
        };

        function openProcessTracking(processInstanceId,context) {

            //VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPInstance/BPInstanceTrackingModal.html', {
                BPInstanceID: processInstanceId,
                context: context
            }, {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Business Process Progress: ";
                }
            });
        };

        function startNewInstance(bpDefinitionObj, onProcessInputCreated, onProcessInputsCreated) {
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = 'Start New Instance';
                modalScope.onProcessInputCreated = onProcessInputCreated;
                modalScope.onProcessInputsCreated = onProcessInputsCreated;
            };
            var parameters = {
                BPDefinitionID: bpDefinitionObj.Entity.BPDefinitionID
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/NewInstanceEditor/NewInstanceEditor.html', parameters, modalSettings);
        }
        function displayRunningInstancesIfExist(definitionId, entityIds, runningInstanceEditorSettings)
        {
            var displayRunningInstancePromiseDeferred = UtilsService.createPromiseDeferred();
            var hasRunningInstancesInput = {
                definitionId: definitionId,
                entityIds: entityIds
            };
            BusinessProcess_BPInstanceAPIService.HasRunningInstances(hasRunningInstancesInput).then(
                function (response) {
                    var result = { hasRunningProcesses: false };
                    if (response == true) {
                        var parameters = {
                            EntityIds: entityIds,
                            context: {
                                onClose: function () {
                                    BusinessProcess_BPInstanceAPIService.HasRunningInstances(hasRunningInstancesInput).then(
                                        function (response) {
                                            result.hasRunningProcesses = response;
                                            displayRunningInstancePromiseDeferred.resolve(result);
                                        });
                                }
                            },
                            runningInstanceEditorSettings: runningInstanceEditorSettings
                        };
                        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPInstance/BPInstanceRunningProcesses.html', parameters, null);
                    }
                    else {
                        displayRunningInstancePromiseDeferred.resolve(result);
                    }

                });
            return displayRunningInstancePromiseDeferred.promise;
        }
        return ({
            getStatusColor: getStatusColor,
            openProcessTracking: openProcessTracking,
            startNewInstance: startNewInstance,
            displayRunningInstancesIfExist: displayRunningInstancesIfExist
        });
    }
    appControllers.service('BusinessProcess_BPInstanceService', BusinessProcess_BPInstanceService);

})(appControllers);