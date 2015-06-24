SchedulerTaskEditorController.$inject = ['$scope', 'SchedulerTaskAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function SchedulerTaskEditorController($scope, SchedulerTaskAPIService, VRModalService, VRNotificationService, VRNavigationService) {

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

        //$scope.optionsUsers = {
        //    selectedvalues: [],
        //    datasource: []
        //};
    }

    function load() {
        //UsersAPIService.GetUsers().then(function (response) {
        //    $scope.optionsUsers.datasource = response;

        //});

        if (editMode) {
            $scope.isGettingData = true;
            getTask().finally(function () {
                $scope.isGettingData = false;
            })
        }
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



    function buildTaskObjFromScope() {
        var taskObject = {
            taskId: ($scope.taskId != null) ? $scope.taskId : 0,
            name: $scope.name,
            isEnabled: $scope.isEnabled
        };
        return taskObject;
    }

    function fillScopeFromTaskObj(taskObject) {
        $scope.name = taskObject.Name;
        $scope.isEnabled = taskObject.IsEnabled;
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
