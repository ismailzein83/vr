SchedulerTaskManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService', 'SchedulerTaskAPIService'];

function SchedulerTaskManagementController($scope, VR_Runtime_SchedulerTaskService, SchedulerTaskAPIService) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.name = undefined;
        $scope.schedulerTasks = [];
        $scope.gridMenuActions = [];

        var filter = {};

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
        filter = ($scope.name != undefined && $scope.name != '') ? $scope.name : null;
    }

    function addTask() {
        var onTaskAdded = function (addedItem) {
            gridAPI.onTaskAdded(addedItem);
        };

        VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);