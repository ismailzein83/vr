DataSourceManagementController.$inject = ['$scope', 'DataSourceAPIService', 'VRModalService', 'VRNotificationService'];

function DataSourceManagementController($scope, DataSourceAPIService, VRModalService, VRNotificationService) {
    var gridApi;

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.dataSources = [];

        defineMenuActions();

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DataSourceAPIService.GetFilteredDataSources(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.AddNewDataSource = addNewDataSource;
    }

    function load() {
    }

    function retrieveData() {
        var query = {
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
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
        var message = 'Do you want to delete ' + dataSourceObj.Name + '?';

        VRNotificationService.showConfirmation(message)
            .then(function (response) {
                if (response == true) {

                    return DataSourceAPIService.DeleteDataSource(dataSourceObj.DataSourceId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Data Source", deletionResponse);
                            $scope.dataSources = []; // clear the grid
                            return getData(); // refresh the grid
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);