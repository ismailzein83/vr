DataSourceManagementController.$inject = ['$scope', 'DataSourceAPIService', 'DataSourceService', 'VRModalService', 'VRNotificationService'];

function DataSourceManagementController($scope, DataSourceAPIService, DataSourceService, VRModalService, VRNotificationService) {
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
                gridApi.itemAdded(dataSource);
            };
        };
        VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceEditor.html', null, settings);
    }

    function editDataSource(dataSourceObj) {
        
        var onDataSourceUpdated = function (dataSource) {
            gridApi.itemUpdated(dataSource);
        }

        DataSourceService.openDataSourceEditor(dataSourceObj.DataSourceId, dataSourceObj.TaskId, onDataSourceUpdated);
    }

    function deleteDataSource(dataSourceObj) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return DataSourceAPIService.DeleteDataSource(dataSourceObj.DataSourceId, dataSourceObj.TaskId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Data Source", deletionResponse);
                            return retrieveData(); // refresh the grid
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);