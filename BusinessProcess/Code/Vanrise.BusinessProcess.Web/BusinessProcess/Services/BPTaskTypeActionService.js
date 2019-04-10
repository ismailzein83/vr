
app.service('BusinessProcess_TaskTypeActionService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'BusinessProcess_BPTaskAPIService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, BusinessProcess_BPTaskAPIService) {

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
                    return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function () {
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
