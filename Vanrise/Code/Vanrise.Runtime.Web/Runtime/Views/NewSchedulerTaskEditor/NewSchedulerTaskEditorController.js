newSchedulerTaskEditorController.$inject = ['$scope', 'SchedulerTaskAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

function newSchedulerTaskEditorController($scope, SchedulerTaskAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

    var editMode;
    var taskId;
    var actionTypeId;
    var additionalParameter;
    var taskObject;
    var taskTriggerDirectiveAPI;
    var taskTriggerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var taskActionDirectiveAPI;
    var taskActionDirectiveReadyPromiseDeferred; //= UtilsService.createPromiseDeferred();
    

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
        $scope.scopeModel = {};
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

        $scope.startEffDate = new Date();
        $scope.endEffDate = undefined;

        $scope.onTaskTriggerDirectiveReady = function (api) {
            taskTriggerDirectiveAPI = api;
            taskTriggerDirectiveReadyPromiseDeferred.resolve();

        }
        $scope.onTaskActionDirectiveReady = function (api) {
            taskActionDirectiveAPI = api;
            var setLoader = function (value) {
                $scope.isLoadingAction = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, taskActionDirectiveAPI, undefined, setLoader, taskActionDirectiveReadyPromiseDeferred);

        }

        $scope.onTaskActionSelctionChanged = function () {
            var payload;
            if (taskObject && taskObject.TaskSettings != undefined && taskActionDirectiveAPI != undefined) {
                payload = {
                    data: taskObject.TaskSettings.TaskActionArgument
                };
                taskActionDirectiveAPI.load(payload);
            }
           
        }
        
    }

    function load() {
        $scope.isLoading = true;

        if (editMode) {
            getTask().then(function () {
                loadAllControls()
                    .finally(function () {
                    });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }
        else {
            $scope.title = UtilsService.buildTitleForAddEditor("Task");
            loadAllControls();
        }
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadTriggers, loadActions,fillScopeFromTaskObj])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }


    function getTask() {
        return SchedulerTaskAPIService.GetTask(taskId)
           .then(function (response) {
               taskObject = response;
               fillScopeFromTaskObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
    }

    function loadTriggers() {
        var promises = [];
        var triggersPayload;
        if (taskObject != undefined && taskObject.TaskSettings != null) {
            triggersPayload = {
                data: taskObject.TaskSettings.TaskTriggerArgument
            };
        }
        var taskTriggerTypesLoad = SchedulerTaskAPIService.GetSchedulerTaskTriggerTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.triggerTypes.push(item);
            });
            if (taskObject.TriggerTypeId)
              $scope.scopeModel.selectedTriggerType = UtilsService.getItemByVal($scope.triggerTypes, taskObject.TriggerTypeId, "TriggerTypeId");
        });
        promises.push(taskTriggerTypesLoad);
        if (triggersPayload != undefined) {
            var loadTaskTriggerPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadTaskTriggerPromiseDeferred.promise);
            taskTriggerDirectiveReadyPromiseDeferred.promise.then(function () {
                taskTriggerDirectiveReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(taskTriggerDirectiveAPI, triggersPayload, loadTaskTriggerPromiseDeferred);
            });

        }
       
        return UtilsService.waitMultiplePromises(promises);
    }

    function loadActions() {
        var promises = [];
        var actionPayload;

        if (taskObject != undefined && taskObject.TaskSettings != null)
        {
            actionPayload = {
                data: taskObject.TaskSettings.TaskActionArgument
            };
        }            

        var loadActionTypesPromise = SchedulerTaskAPIService.GetSchedulerTaskActionTypes().then(function (response) {
            angular.forEach(response, function (item) {
                if (!item.Info.SystemType)
                {
                    $scope.actionTypes.push(item);
                }
            });
            if (taskObject != undefined && taskObject.ActionTypeId)
               $scope.scopeModel.selectedActionType = UtilsService.getItemByVal($scope.actionTypes, taskObject.ActionTypeId, "ActionTypeId");
        });

        promises.push(loadActionTypesPromise);

        if (actionPayload != undefined) {
            taskActionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var loadTaskActionPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadTaskActionPromiseDeferred.promise);

            taskActionDirectiveReadyPromiseDeferred.promise.then(function () {
                taskActionDirectiveReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(taskActionDirectiveAPI, actionPayload, loadTaskActionPromiseDeferred);
            });
        }

        return UtilsService.waitMultiplePromises(promises);
    }

   

    function buildTaskObjFromScope() {
        var taskObject = {
            TaskId: (taskId != null) ? taskId : 0,
            Name: $scope.scopeModel.name,
            IsEnabled: $scope.scopeModel.isEnabled,
            TaskType: 1,
            TriggerTypeId: $scope.scopeModel.selectedTriggerType.TriggerTypeId,
            ActionTypeId: $scope.scopeModel.selectedActionType.ActionTypeId,
            TaskSettings:
                {
                    TaskTriggerArgument: taskTriggerDirectiveAPI.getData(),
                    TaskActionArgument: taskActionDirectiveAPI.getData(),
                    StartEffDate:$scope.scopeModel.startEffDate,
                    EndEffDate: $scope.scopeModel.endEffDate
                }
        };
        return taskObject;
    }

    function fillScopeFromTaskObj() {
        $scope.title = UtilsService.buildTitleForUpdateEditor(taskObject.Name, "Task");
        $scope.scopeModel.name = taskObject.Name;
        $scope.scopeModel.isEnabled = taskObject.IsEnabled;
        
        //$scope.schedulerTaskTrigger.data = taskObject.TaskSettings.TaskTriggerArgument;
        //if ($scope.schedulerTaskTrigger.loadTemplateData != undefined)
        //    $scope.schedulerTaskTrigger.loadTemplateData();

        //$scope.schedulerTaskAction.data = taskObject.TaskSettings.TaskActionArgument;
        //if ($scope.schedulerTaskAction.loadTemplateData != undefined)
        //    $scope.schedulerTaskAction.loadTemplateData();

        $scope.scopeModel.startEffDate = taskObject.TaskSettings.StartEffDate;
        $scope.scopeModel.endEffDate = taskObject.TaskSettings.EndEffDate;
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
        console.log(taskObject)
        //SchedulerTaskAPIService.UpdateTask(taskObject)
        //.then(function (response) {
        //    if (VRNotificationService.notifyOnItemUpdated("Schedule Task", response)) {
        //        if ($scope.onTaskUpdated != undefined)
        //            $scope.onTaskUpdated(response.UpdatedObject);
        //        $scope.modalContext.closeModal();
        //    }
        //}).catch(function (error) {
        //    VRNotificationService.notifyException(error, $scope);
        //});
    }
}
appControllers.controller('Runtime_NewSchedulerTaskEditorController', newSchedulerTaskEditorController);
