DataSourceManagementController.$inject = ['$scope', 'DataSourceAPIService', 'DataSourceService', "UtilsService", 'VRModalService', 'VRNotificationService'];

function DataSourceManagementController($scope, DataSourceAPIService, DataSourceService, UtilsService, VRModalService, VRNotificationService) {
    var gridApi;

    defineScope();
    load();

    function defineScope() {
        $scope.gridMenuActions = [];
        $scope.dataSources = [];

        // filter vars
        $scope.name = undefined;
        $scope.adapterTypes = [];
        $scope.selectedAdapterTypes = [];
        $scope.statuses = [
            { value: true, description: "Enabled" },
            { value: false, description: "Disabled" }
        ];
        $scope.selectedStatuses = [];

        defineMenuActions();

        $scope.searchClicked = function () {
            return retrieveData();
        }

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
        $scope.isLoadingFilters = true;

        return DataSourceAPIService.GetDataSourceAdapterTypes()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.adapterTypes.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            AdapterTypeIDs: UtilsService.getPropValuesFromArray($scope.selectedAdapterTypes, "AdapterTypeId"),
            IsEnabled: ($scope.selectedStatuses.length == 1) ? $scope.selectedStatuses[0].value : null
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
            console.log(dataSource);
            gridApi.itemUpdated(dataSource);
        }

        DataSourceService.editDataSource(dataSourceObj, onDataSourceUpdated);
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