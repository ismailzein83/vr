DataSourceLogManagementController.$inject = ['$scope', 'DataSourceLogAPIService', 'DataSourceAPIService', 'UtilsService', 'VRNotificationService'];

function DataSourceLogManagementController($scope, DataSourceLogsAPIService, DataSourceAPIService, UtilsService, VRNotificationService) {

    var gridApi;
    var filtersAreNotReady = true;

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

        loadSeverities();

        DataSourceAPIService.GetDataSources()
            .then(function (response) {
                $scope.dataSources = response;
                
                if (response.length > 0) // select the first data source
                    $scope.selectedDataSource = $scope.dataSources[0];

                filtersAreNotReady = false;
                return retrieveData();
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingForm = false;
            });
    }

    function retrieveData() {
        if (gridApi == undefined || filtersAreNotReady) return;

        var query = {
            DataSourceId: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.Id : null,
            Severities: getMappedSelectedSeverities(),
            From: ($scope.selectedFromDateTime != undefined) ? $scope.selectedFromDateTime : null,
            To: ($scope.selectedToDateTime != undefined) ? $scope.selectedToDateTime : null
        };

        return gridApi.retrieveData(query);
    }

    function loadSeverities() {
        $scope.severities = UtilsService.getLogEntryType();
    }

    function getMappedSelectedSeverities() {

        if ($scope.selectedSeverities.length == 0) {
            var logEntryType = UtilsService.getLogEntryType();
            logEntryType.splice(3, 1); // remove Verbose

            $scope.selectedSeverities = logEntryType; // select Error, Warning and Information only
        }

        var mappedSelectedSeverities = [];

        for (var i = 0; i < $scope.selectedSeverities.length; i++) {
            mappedSelectedSeverities.push($scope.selectedSeverities[i].value);
        }

        return mappedSelectedSeverities;
    }
}

appControllers.controller('Integration_DataSourceLogManagementController', DataSourceLogManagementController);