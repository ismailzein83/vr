DataSourceImportedBatchManagementController.$inject = ['$scope', 'DataSourceImportedBatchAPIService', 'DataSourceAPIService', 'Integration_MappingResultTypeEnum', 'UtilsService', 'VRNotificationService'];

function DataSourceImportedBatchManagementController($scope, DataSourceImportedBatchAPIService, DataSourceAPIService, Integration_MappingResultTypeEnum, UtilsService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {
        $scope.dataSources = [];
        $scope.selectedDataSource = [];
        $scope.mappingResults = [];
        $scope.selectedMappingResult = undefined;
        $scope.batchNames = [];
        $scope.selectedBatchName = undefined;
        $scope.selectedFromDateTime = undefined;
        $scope.selectedToDateTime = undefined;
        $scope.importedBatches = [];

        $scope.gridReady = function (api) {
            gridApi = api;

            // get all of the data sources to filter by the first data source
            $scope.isLoadingForm = true;

            DataSourceAPIService.GetDataSources()
                .then(function (response) {
                    $scope.dataSources = response;

                    if (response.length > 0) // select the first data source
                        $scope.selectedDataSource = $scope.dataSources[0];

                    return retrieveData();
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function () {
                    $scope.isLoadingForm = false;
                });
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DataSourceImportedBatchAPIService.GetFilteredDataSourceImportedBatches(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.searchClicked = function () {
            return retrieveData();
        }
    }

    function load() {
        $scope.isLoadingForm = true;

        UtilsService.waitMultipleAsyncOperations([loadBatchNames, loadMappingResults])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingForm = false;
            });
    }

    function retrieveData() {
        var query = {
            DataSourceId: $scope.selectedDataSource.ID,
            BatchName: ($scope.selectedBatchName != undefined) ? $scope.selectedBatchName.BatchName : null,
            MappingResult: ($scope.selectedMappingResult != undefined) ? $scope.selectedMappingResult.value : Integration_MappingResultTypeEnum.Valid.value,
            From: $scope.selectedFromDateTime,
            To: $scope.selectedToDateTime
        };

        return gridApi.retrieveData(query);
    }

    function loadBatchNames() {
        return DataSourceImportedBatchAPIService.GetBatchNames()
            .then(function (response) {
                $scope.batchNames = response;
            });
    }

    function loadMappingResults() {
        for (property in Integration_MappingResultTypeEnum) {
            var object = {
                value: Integration_MappingResultTypeEnum[property].value,
                description: Integration_MappingResultTypeEnum[property].description
            }

            $scope.mappingResults.push(object);
        }
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
