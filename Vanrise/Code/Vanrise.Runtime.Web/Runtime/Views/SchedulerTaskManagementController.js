SchedulerTaskManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService', 'SchedulerTaskAPIService'];

function SchedulerTaskManagementController($scope, VR_Runtime_SchedulerTaskService, SchedulerTaskAPIService) {

    var gridAPI;

    defineScope();
    load();
    var filter = {};

    function defineScope() {

        $scope.name = undefined;
        $scope.schedulerTasks = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            gridAPI.loadGrid(filter);
        };

        $scope.searchClicked = function () {
            setFilterObject();
            return gridAPI.loadGrid(filter);
        };

        $scope.AddNewTask = addTask;

        $scope.hasAddSchedulerTaskPermission = function () {
            return SchedulerTaskAPIService.HasAddSchedulerTaskPermission();
        };
    }

    function load() {
    }

    function setFilterObject() {
        filter =
            {
                NameFilter: ($scope.name != undefined && $scope.name != '') ? $scope.name : null
            };
    }

    function addTask() {
        var onTaskAdded = function (addedItem) {
            var addedItemObj = { Entity: addedItem };
            gridAPI.onTaskAdded(addedItemObj);
        };

        VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);