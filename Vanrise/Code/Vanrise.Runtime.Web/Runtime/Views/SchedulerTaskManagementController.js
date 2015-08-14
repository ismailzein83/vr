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
        var name = ($scope.name != undefined && $scope.name != '') ? $scope.name : null;

        return gridApi.retrieveData(name);
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
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
    }

    function deleteTask(taskObj) {
        var message = 'Do you want to delete ' + taskObj.Name + '?';

        VRNotificationService.showConfirmation(message)
            .then(function (response) {
                if (response == true) {

                    return SchedulerTaskAPIService.DeleteTask(taskObj.TaskId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Scheduler Task", deletionResponse);
                            // to be removed
                            return retrieveData();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);