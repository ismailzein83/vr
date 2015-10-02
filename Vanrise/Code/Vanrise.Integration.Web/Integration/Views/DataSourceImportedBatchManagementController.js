DataSourceImportedBatchManagementController.$inject = ['$scope', 'DataSourceImportedBatchAPIService', 'DataSourceAPIService', 'Integration_MappingResultEnum', 'UtilsService', 'DataSourceService', 'VRNotificationService'];

function DataSourceImportedBatchManagementController($scope, DataSourceImportedBatchAPIService, DataSourceAPIService, Integration_MappingResultEnum, UtilsService, DataSourceService, VRNotificationService) {

    var gridApi;
    var filtersAreNotReady = true;

    defineScope();
    load();

    function defineScope() {
        $scope.dataSources = [];
        $scope.selectedDataSource = [];
        $scope.mappingResults = [];
        $scope.selectedMappingResults = [];
        $scope.batchName = undefined;
        $scope.selectedFromDateTime = undefined;
        $scope.selectedToDateTime = undefined;
        $scope.importedBatches = [];

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DataSourceImportedBatchAPIService.GetFilteredDataSourceImportedBatches(dataRetrievalInput)
                .then(function (response) {
                    angular.forEach(response.Data, function (item) {
                        item.MappingResultDescription = DataSourceService.getMappingResultDescription(item.MappingResult);
                        item.ExecutionStatusDescription = DataSourceService.getExecutionStatusDescription(item.ExecutionStatus);
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

        $scope.getStatusColor = function (dataItem, colDef) {
            return DataSourceService.getExecutionStatusColor(dataItem.ExecutionStatus);
        }
    }

    function load() {
        $scope.isLoadingForm = true;

        $scope.mappingResults = UtilsService.getArrayEnum(Integration_MappingResultEnum);

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
            DataSourceId: $scope.selectedDataSource.DataSourceId,
            BatchName: ($scope.batchName != undefined && $scope.batchName != "") ? $scope.batchName : null,
            MappingResults: getMappedMappingResults(),
            From: $scope.selectedFromDateTime,
            To: $scope.selectedToDateTime
        };

        console.log(query);

        return gridApi.retrieveData(query);
    }

    function getMappedMappingResults() {

        if ($scope.selectedMappingResults.length == 0) {
            // select all
            $scope.selectedMappingResults = UtilsService.getArrayEnum(Integration_MappingResultEnum);
        }

        var mappedMappingResults = [];

        for (var i = 0; i < $scope.selectedMappingResults.length; i++) {
            mappedMappingResults.push($scope.selectedMappingResults[i].value);
        }

        return mappedMappingResults;
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
