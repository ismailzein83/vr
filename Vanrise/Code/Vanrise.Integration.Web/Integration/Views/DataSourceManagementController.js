DataSourceManagementController.$inject = ['$scope', 'DataSourceAPIService', 'VRModalService'];


function DataSourceManagementController($scope, DataSourceAPIService, VRModalService) {
    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.dataSources = [];

        defineMenuActions();

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.AddNewDataSource = addNewDataSource;
    }

    function load() {
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return DataSourceAPIService.GetDataSources().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.dataSources.push(item);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editDataSource
        },
        {
            name: "Delete",
            clicked: deleteDataSource
        }
        ];
    }

    function addNewDataSource() {
        //var settings = {};

        //settings.onScopeReady = function (modalScope) {
        //    modalScope.title = "Add Task";
        //    modalScope.onTaskAdded = function (task) {
        //        mainGridAPI.itemAdded(task);
        //    };
        //};
        //VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', null, settings);
    }

    function editDataSource(dataSourceObj) {
        //var modalSettings = {
        //};
        //var parameters = {
        //    taskId: taskObj.TaskId
        //};

        //modalSettings.onScopeReady = function (modalScope) {
        //    modalScope.title = "Edit Task: " + taskObj.Name;
        //    modalScope.onTaskUpdated = function (task) {
        //        mainGridAPI.itemUpdated(task);
        //    };
        //};
        //VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, modalSettings);
    }

    function deleteDataSource(dataSourceObj) {
        //TODO: implement Delete functionality
    }
}
appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);