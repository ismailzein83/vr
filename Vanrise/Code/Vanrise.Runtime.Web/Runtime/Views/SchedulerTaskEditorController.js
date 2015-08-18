SchedulerTaskEditorController.$inject = ['$scope', 'SchedulerTaskAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function SchedulerTaskEditorController($scope, SchedulerTaskAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    var taskId;
    var actionTypeId;
    var additionalParameter;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        taskId = undefined;
        actionTypeId = undefined;
        additionalParameter = undefined;

        if (parameters != undefined && parameters != null)
        {
            taskId = parameters.taskId;
            actionTypeId = parameters.actionTypeId;
            additionalParameter = parameters.additionalParameter;
        }
        

        if (taskId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        
        $scope.SaveTask = function () {
            if (editMode) {
                return updateTask();
            }
            else {
                return insertTask();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.triggerTypes = [];
        $scope.schedulerTaskTrigger = {};

        $scope.actionTypes = [];
        $scope.schedulerTaskAction = {};
    }

    function load() {
        
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadTriggers, loadActions]).then(function () {
            

            if (editMode) {
                getTask();
            }
            else {
                
                $scope.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, 1, "TriggerTypeId");
                if (actionTypeId != undefined) {
                    $scope.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, actionTypeId, "ActionTypeId");
                }
                else {
                    $scope.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, 1, "ActionTypeId");
                }

                if (additionalParameter != undefined) {
                    $scope.schedulerTaskAction.additionalParameter = additionalParameter;
                }


                $scope.isGettingData = false;

            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getTask() {
        return SchedulerTaskAPIService.GetTask(taskId)
           .then(function (response) {
               fillScopeFromTaskObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
    }

    function loadTriggers()
    {
        return SchedulerTaskAPIService.GetSchedulerTaskTriggerTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.triggerTypes.push(item);
            });
        });
    }

    function loadActions() {
        return SchedulerTaskAPIService.GetSchedulerTaskActionTypes().then(function (response) {
            angular.forEach(response, function (item) {
                if (!item.Info.SystemType)
                {
                    $scope.actionTypes.push(item);
                }
            });
        });
    }

    function buildTaskObjFromScope() {
        var taskObject = {
            TaskId: (taskId != null) ? taskId : 0,
            Name: $scope.name,
            IsEnabled: $scope.isEnabled,
            TaskType: 1,
            TriggerTypeId: $scope.selectedTriggerType.TriggerTypeId,
            ActionTypeId: $scope.selectedActionType.ActionTypeId,
            TaskSettings: { TaskTriggerArgument: $scope.schedulerTaskTrigger.getData(), TaskActionArgument: $scope.schedulerTaskAction.getData()}
        };
        return taskObject;
    }

    function fillScopeFromTaskObj(taskObject) {
        $scope.name = taskObject.Name;
        $scope.isEnabled = taskObject.IsEnabled;

        $scope.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, taskObject.TriggerTypeId, "TriggerTypeId");
        $scope.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, taskObject.ActionTypeId, "ActionTypeId");

        $scope.schedulerTaskTrigger.data = taskObject.TaskSettings.TaskTriggerArgument;
        if ($scope.schedulerTaskTrigger.loadTemplateData != undefined)
            $scope.schedulerTaskTrigger.loadTemplateData();

        $scope.schedulerTaskAction.data = taskObject.TaskSettings.TaskActionArgument;
        if ($scope.schedulerTaskAction.loadTemplateData != undefined)
            $scope.schedulerTaskAction.loadTemplateData();
    }

    function insertTask() {
        $scope.issaving = true;
        var taskObject = buildTaskObjFromScope();
        return SchedulerTaskAPIService.AddTask(taskObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Schedule Task", response)) {
                if ($scope.onTaskAdded != undefined)
                    $scope.onTaskAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateTask() {
        var taskObject = buildTaskObjFromScope();
        SchedulerTaskAPIService.UpdateTask(taskObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Schedule Task", response)) {
                if ($scope.onTaskUpdated != undefined)
                    $scope.onTaskUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('Runtime_SchedulerTaskEditorController', SchedulerTaskEditorController);
