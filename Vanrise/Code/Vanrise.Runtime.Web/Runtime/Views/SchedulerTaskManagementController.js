SchedulerTaskManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService', 'SchedulerTaskAPIService', 'VRNavigationService'];

function SchedulerTaskManagementController($scope, VR_Runtime_SchedulerTaskService, SchedulerTaskAPIService, VRNavigationService) {

    var gridAPI;

    defineScope();
    
    load();
    var filter = {};

    var isMyTasks = false;
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            isMyTasks = true;
        }
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.schedulerTasks = [];
        $scope.gridMenuActions = [];

        $scope.onGridReady = function (api) {
            gridAPI = api;
            loadParameters();
            gridAPI.isMyTasksSelected(isMyTasks);
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
            var addedItemObj = addedItem;
            gridAPI.onTaskAdded(addedItemObj);
        };

        VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);