
app.service('BusinessProcess_TaskTypeActionService', ['VRNotificationService', 'BusinessProcess_BPTaskAPIService', 'WhS_BP_ExecuteBPTaskResultEnum',
    function (VRNotificationService, BusinessProcess_BPTaskAPIService, WhS_BP_ExecuteBPTaskResultEnum) {

        var actionTypes = [];

        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function registerExecuteAction() {
            var actionType = {
                ActionTypeName: "Execute",
                actionMethod: function (payload) {
                    var input = {
                        $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                        TaskId: payload.taskId,
                        TaskData: {
                            $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData, Vanrise.BusinessProcess.MainExtensions",
                            FieldValues: payload.fieldValues
                        },
                        Notes: payload.notes,
                        Decision: payload.decision
                    };
                    return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                        if (response != undefined && response.Result == WhS_BP_ExecuteBPTaskResultEnum.Failed.value) {
                            VRNotificationService.showError(response.OutputMessage);
                        }
                        else
                            payload.context.closeModal();
                    });
                }
            };
            registerActionType(actionType);
        }

        return ({
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            registerExecuteAction: registerExecuteAction
        });
    }]);
