DataSourceLogManagementController.$inject = ['$scope', 'DataSourceLogAPIService', 'DataSourceAPIService', 'UtilsService', 'VRNotificationService'];

function DataSourceLogManagementController($scope, DataSourceLogsAPIService, DataSourceAPIService, UtilsService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {

        $scope.dataSources = [];
        $scope.selectedDataSource = undefined;
        $scope.severities = [];
        $scope.selectedSeverities = [];
        $scope.selectedFromDateTime = undefined;
        $scope.selectedToDateTime = undefined;
        $scope.logs = [];

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DataSourceLogsAPIService.GetFilteredDataSourceLogs(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {
                        item.SeverityDescription = UtilsService.getLogEntryTypeDescription(item.Severity);
                    });
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.getSeverityColor = function (dataItem, colDef) {
            return UtilsService.getLogEntryTypeColor(dataItem.Severity);
        }
    }

    function load() {
        $scope.isLoadingForm = true;

        UtilsService.waitMultipleAsyncOperations([loadDataSources, loadSeverities])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingForm = false;
            });
    }

    function retrieveData() {
        var query = {
            DataSourceId: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.Id : null,
            Severities: getMappedSelectedSeverities(),
            From: ($scope.selectedFromDateTime != undefined) ? $scope.selectedFromDateTime : null,
            To: ($scope.selectedToDateTime != undefined) ? $scope.selectedToDateTime : null
        };

        return gridApi.retrieveData(query);
    }

    function loadDataSources() {
        return DataSourceAPIService.GetDataSources()
            .then(function (response) {
                $scope.dataSources = response;
            });
    }

    function loadSeverities() {
        $scope.severities = UtilsService.getLogEntryType();
    }

    function getMappedSelectedSeverities() {
        var selectedSeverities = ($scope.selectedSeverities.length > 0) ? $scope.selectedSeverities : UtilsService.getLogEntryType();
        var mappedSelectedSeverities = [];

        for (var i = 0; i < selectedSeverities.length; i++) {
            mappedSelectedSeverities.push(selectedSeverities[i].value);
        }

        return mappedSelectedSeverities;
    }
}

appControllers.controller('Integration_DataSourceLogManagementController', DataSourceLogManagementController);