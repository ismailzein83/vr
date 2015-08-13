SchedulerTaskManagementController.$inject = ['$scope', 'SchedulerTaskAPIService', 'VRModalService', 'VRNotificationService'];

function SchedulerTaskManagementController($scope, SchedulerTaskAPIService, VRModalService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {

        $scope.name = undefined;
        $scope.schedulerTasks = [];
        $scope.gridMenuActions = [];

        defineMenuActions();

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SchedulerTaskAPIService.GetFilteredTasks(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.AddNewTask = addTask;
    }

    function load() {
    }

    function retrieveData() {
        var query = {
            Name: ($scope.name != undefined) ? $scope.name : null
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editTask
            },
            {
                name: "Delete",
                clicked: deleteTask
            }
        ];
    }

    function addTask() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Task";
            modalScope.onTaskAdded = function (task) {
                gridApi.itemAdded(task);
                return retrieveData();
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', null, settings);
    }

    function editTask(taskObj) {
        var modalSettings = {
        };
        var parameters = {
            taskId: taskObj.TaskId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Task: " + taskObj.Name;
            modalScope.onTaskUpdated = function (task) {
                gridApi.itemUpdated(task);
                return retrieveData();
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
    }

    function deleteTask(taskObj) {
        //TODO: implement Delete functionality
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);