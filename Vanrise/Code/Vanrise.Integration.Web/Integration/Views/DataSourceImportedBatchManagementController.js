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
            return retrieveData();
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

        UtilsService.waitMultipleAsyncOperations([loadDataSources, loadBatchNames, loadMappingResults])
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

    function loadDataSources() {
        return DataSourceAPIService.GetDataSources()
            .then(function (response) {
                $scope.dataSources = response;
            });
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
