SchedulerTaskEditorController.$inject = ['$scope', 'SchedulerTaskAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function SchedulerTaskEditorController($scope, SchedulerTaskAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.taskId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.taskId = parameters.taskId;

        if ($scope.taskId != undefined)
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
        UtilsService.waitMultipleAsyncOperations([loadTriggers, loadActions]).finally(function () {

            if (editMode) {
                getTask();
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });

        
    }

    function getTask() {
        return SchedulerTaskAPIService.GetTask($scope.taskId)
           .then(function (response) {
               fillScopeFromTaskObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
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
                $scope.actionTypes.push(item);
            });
        });
    }

    function buildTaskObjFromScope() {

        var taskObject = {
            TaskId: ($scope.taskId != null) ? $scope.taskId : 0,
            Name: $scope.name,
            IsEnabled: $scope.isEnabled,
            TriggerTypeId: $scope.selectedTriggerType.TriggerTypeId,
            TaskTrigger: $scope.schedulerTaskTrigger.getData(),
            ActionTypeId: $scope.selectedActionType.ActionTypeId,
            TaskAction: $scope.schedulerTaskAction.getData()
        };
        return taskObject;
    }

    function fillScopeFromTaskObj(taskObject) {
        $scope.name = taskObject.Name;
        $scope.isEnabled = taskObject.IsEnabled;
        $scope.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, taskObject.TriggerTypeId, "TriggerTypeId");
        $scope.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, taskObject.ActionTypeId, "ActionTypeId");
        $scope.schedulerTaskTrigger.data = taskObject.TaskTrigger;
        $scope.schedulerTaskAction.data = taskObject.TaskAction;
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
