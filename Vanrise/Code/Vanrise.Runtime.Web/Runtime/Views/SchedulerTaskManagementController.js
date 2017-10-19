SchedulerTaskManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService', 'SchedulerTaskAPIService', 'VRNavigationService', 'VRNotificationService', 'UtilsService'];

function SchedulerTaskManagementController($scope, VR_Runtime_SchedulerTaskService, SchedulerTaskAPIService, VRNavigationService, VRNotificationService, UtilsService) {

    var gridAPI;

    defineScope();
    
    load();
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
        $scope.showDisableAll = false;
        $scope.showEnableAll = false;
        $scope.onGridReady = function (api) {
            gridAPI = api;
            loadParameters();
            gridAPI.isMyTasksSelected(isMyTasks);
            gridAPI.load(getGridQuery()).then(function () {
                getTaskManagmentInfo();
            });
        };

        $scope.searchClicked = function () {
            return gridAPI.load(getGridQuery());
        };

        $scope.AddNewTask = addTask;
        $scope.DisableAllTasks = disableAllTasks;
        $scope.EnableAllTasks = enableAllTasks;

        $scope.hasAddSchedulerTaskPermission = function () {
            return SchedulerTaskAPIService.HasAddSchedulerTaskPermission();
        };

        $scope.hasConfigureAllTaskPermission = function () {
            return SchedulerTaskAPIService.DoesUserHaveConfigureAllTaskAccess();
        };
    }

    function load() {

    }
   
    function getTaskManagmentInfo() {
        SchedulerTaskAPIService.GetTaskManagmentInfo().then(function (response) {
            $scope.showDisableAll = response.ShowDisableAll;
            $scope.showEnableAll = response.ShowEnableAll;
        });
    }

    function getGridQuery() {
        var query = {
            NameFilter: ($scope.name != undefined && $scope.name != '') ? $scope.name : null
        };
        var payload = {
            context: getContext(),
            query: query
        };
        return payload;
    }
    function addTask() {
        var onTaskAdded = function (addedItem) {
            var addedItemObj = addedItem;
            gridAPI.onTaskAdded(addedItemObj);
            getTaskManagmentInfo();
        };

        VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
    }


    function disableAllTasks() {

        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return SchedulerTaskAPIService.DisableAllTasks().then(function (response) {
                    if (response) {
                        $scope.showDisableAll = false;
                        $scope.showEnableAll = true;
                        gridAPI.updateDataItemsStatuts(false);
                       $scope.searchClicked();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, null);
                });
            }
        });
    }

    function enableAllTasks() {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return SchedulerTaskAPIService.EnableAllTasks().then(function (response) {
                    if (response) {
                        $scope.showDisableAll = true;
                        $scope.showEnableAll = false;
                        gridAPI.updateDataItemsStatuts(true);
                        $scope.searchClicked();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, null);
                });
            }
        });
    }

    function getContext() {
       return  {
            getTaskManagmentInfo: function () {
                getTaskManagmentInfo();
            }
        };
    }
}

appControllers.controller('Runtime_SchedulerTaskManagementController', SchedulerTaskManagementController);