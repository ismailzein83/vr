SchedulerTaskManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService'];

function SchedulerTaskManagementController($scope, VR_Runtime_SchedulerTaskService) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.name = undefined;
        $scope.schedulerTasks = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.AddNewTask = addTask;
        $scope.EditNew = editNew;
    }

    function load() {
    }

    function retrieveData() {
        var name = ($scope.name != undefined && $scope.name != '') ? $scope.name : null;

        return gridAPI.loadGrid(name);
    }

    function addTask() {
        var onTaskAdded = function (addedItem) {
            gridAPI.onTaskAdded(addedItem);
        };

        VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
    }
    function editNew() {
        var onTaskAdded = function (addedItem) {
            //gridAPI.onTaskAdded(addedItem);
        };

        VR_Runtime_SchedulerTaskService.editTaskNew(37,onTaskAdded);
    }

}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);