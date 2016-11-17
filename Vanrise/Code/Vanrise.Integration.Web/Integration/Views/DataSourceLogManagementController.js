DataSourceLogManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

function DataSourceLogManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {

    var gridApi;
    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();


    defineScope();
    load();

    function defineScope() {
        $scope.showGrid = false;
        $scope.dataSources = [];
        $scope.severities = [];
        $scope.selectedSeverities = [];
        var fromDate = new Date();

        fromDate.setHours(0, 0, 0, 0);
        $scope.selectedFromDateTime = fromDate;

        $scope.gridReady = function (api) {
            gridApi = api;
        };

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return gridApi.loadGrid(getQueryGrid());
        };
        $scope.onDataSourceSelectorReady = function (api) {
            dataSourceDirectiveAPI = api;
            dataSourceReadyPromiseDeferred.resolve();
        };

    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSeverities, loadDatasourceSelector]).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isLoading = false;
        }).finally(function () {
            $scope.isLoading = false;
        });
    }

    function loadDatasourceSelector() {
        var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        dataSourceReadyPromiseDeferred.promise
            .then(function () {
                periodReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, undefined, dataSourceLoadPromiseDeferred);
            });
        return dataSourceLoadPromiseDeferred.promise;
    }

    function getQueryGrid() {
        var query = {
            DataSourceId: ($scope.selectedDataSource != undefined) ? $scope.selectedDataSource.DataSourceID : null,
            Severities: getMappedSelectedSeverities(),
            From: ($scope.selectedFromDateTime != undefined) ? $scope.selectedFromDateTime : null,
            To: ($scope.selectedToDateTime != undefined) ? $scope.selectedToDateTime : null
        };
        return query
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