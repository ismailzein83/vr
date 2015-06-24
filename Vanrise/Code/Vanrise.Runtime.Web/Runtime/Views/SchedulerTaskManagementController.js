SchedulerTaskManagementController.$inject = ['$scope', 'SchedulerTaskAPIService', 'VRModalService'];


function SchedulerTaskManagementController($scope, SchedulerTaskAPIService, VRModalService) {
    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.schedulerTasks = [];

        defineMenuActions();

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.AddNewTask = addTask;
    }

    function load() {
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return SchedulerTaskAPIService.GetFilteredTasks(pageInfo.fromRow, pageInfo.toRow, name).then(function (response) {
            angular.forEach(response, function (item) {
                $scope.schedulerTasks.push(item);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
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
                mainGridAPI.itemAdded(task);
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
                mainGridAPI.itemUpdated(task);
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
    }

    function deleteTask(taskObj) {
        //TODO: implement Delete functionality
    }
}
appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);