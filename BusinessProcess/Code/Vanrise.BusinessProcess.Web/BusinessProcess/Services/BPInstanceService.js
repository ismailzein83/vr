(function (appControllers) {

    "use strict";
    BusinessProcess_BPInstanceService.$inject = ['LabelColorsEnum', 'BPInstanceStatusEnum', 'VRModalService'];
    function BusinessProcess_BPInstanceService(LabelColorsEnum, BPInstanceStatusEnum, VRModalService) {
        function getStatusColor(status) {

            if (status === BPInstanceStatusEnum.New.value) return LabelColorsEnum.New.color;
            if (status === BPInstanceStatusEnum.Postponed.value) return LabelColorsEnum.WarningLevel1.color;
            if (status === BPInstanceStatusEnum.Running.value) return LabelColorsEnum.Processing.color;
            if (status === BPInstanceStatusEnum.ProcessFailed.value) return LabelColorsEnum.Warning.color;
            if (status === BPInstanceStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === BPInstanceStatusEnum.Aborted.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Suspended.value) return LabelColorsEnum.Error.color;
            if (status === BPInstanceStatusEnum.Terminated.value) return LabelColorsEnum.Error.color;

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

        return ({
            getStatusColor: getStatusColor,
            openProcessTracking: openProcessTracking,
            startNewInstance: startNewInstance
        });
    }
    appControllers.service('BusinessProcess_BPInstanceService', BusinessProcess_BPInstanceService);

})(appControllers);