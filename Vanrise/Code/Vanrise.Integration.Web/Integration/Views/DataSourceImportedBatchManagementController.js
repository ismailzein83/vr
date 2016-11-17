DataSourceImportedBatchManagementController.$inject = ['$scope', 'VR_Integration_MappingResultEnum', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

function DataSourceImportedBatchManagementController($scope, VR_Integration_MappingResultEnum, UtilsService, VRNotificationService, VRUIUtilsService) {

    var gridApi;
    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.mappingResults = [];
        $scope.selectedMappingResults = [];
        $scope.importedBatches = [];

        $scope.showGrid = false;

        $scope.gridReady = function (api) {
            gridApi = api;
        };

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return gridApi.loadGrid(getGridQuery());
        };

        $scope.mappingResults = UtilsService.getArrayEnum(VR_Integration_MappingResultEnum);

        $scope.onDataSourceSelectorReady = function (api) {
            dataSourceDirectiveAPI = api;
            dataSourceReadyPromiseDeferred.resolve();
        };
    }

    function load() {
        $scope.isLoading= true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadDatasourceSelector]).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isLoading = false;
        }).finally(function () {
            $scope.isLoading = false;
        });
    }

    function getGridQuery() {
        var query = {
            DataSourceId: dataSourceDirectiveAPI.getSelectedIds(),
            BatchName: ($scope.batchName != undefined && $scope.batchName != "") ? $scope.batchName : null,
            MappingResults: getMappedMappingResults(),
            From: $scope.selectedFromDateTime,
            To: $scope.selectedToDateTime
        };
        return query;

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

    function getMappedMappingResults() {

        if ($scope.selectedMappingResults.length == 0) {
            // select all
            $scope.selectedMappingResults = UtilsService.getArrayEnum(VR_Integration_MappingResultEnum);
        }

        var mappedMappingResults = [];

        for (var i = 0; i < $scope.selectedMappingResults.length; i++) {
            mappedMappingResults.push($scope.selectedMappingResults[i].value);
        }

        return mappedMappingResults;
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
