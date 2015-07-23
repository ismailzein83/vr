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
        $scope.isGettingData = true;

        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return DataSourceAPIService.GetDataSources().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.dataSources.push(item);
            });
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
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
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Data Source";
            modalScope.onDataSourceAdded = function (dataSource) {
                mainGridAPI.itemAdded(dataSource);
            };
        };
        VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceEditor.html', null, settings);
    }

    function editDataSource(dataSourceObj) {
        var modalSettings = {
        };
        var parameters = {
            dataSourceId: dataSourceObj.DataSourceId,
            taskId: dataSourceObj.TaskId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Data Source";
            modalScope.onDataSourceUpdated = function (dataSource) {
                mainGridAPI.itemUpdated(dataSource);
            };
        };
        VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceEditor.html', parameters, modalSettings);
    }

    function deleteDataSource(dataSourceObj) {
        //TODO: implement Delete functionality
    }
}
appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);