DataSourceImportedBatchManagementController.$inject = ['$scope', 'DataSourceImportedBatchAPIService', 'DataSourceAPIService', 'Integration_MappingResultEnum', 'Integration_ExecutionStatusEnum', 'Integration_ExecutionStatusColorEnum', 'UtilsService', 'VRNotificationService'];

function DataSourceImportedBatchManagementController($scope, DataSourceImportedBatchAPIService, DataSourceAPIService, Integration_MappingResultEnum, Integration_ExecutionStatusEnum, Integration_ExecutionStatusColorEnum, UtilsService, VRNotificationService) {

    var gridApi;
    var filtersAreNotReady = true;

    defineScope();
    load();

    function defineScope() {
        $scope.dataSources = [];
        $scope.selectedDataSource = [];
        $scope.mappingResults = [];
        $scope.selectedMappingResult = undefined;
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
                        item.MappingResultDescription = getMappingResultDescription(item.MappingResult);
                        item.ExecutionStatusDescription = getExecutionStatusDescription(item.ExecutionStatus);
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
            return getExecutionStatusColor(dataItem.ExecutionStatus);
        }
    }

    function load() {
        $scope.isLoadingForm = true;

        loadMappingResults();

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
        if (gridApi == undefined) return;
        if (filtersAreNotReady) return;

        var query = {
            DataSourceId: $scope.selectedDataSource.ID,
            BatchName: ($scope.batchName != undefined) ? $scope.batchName : null,
            MappingResult: ($scope.selectedMappingResult != undefined) ? $scope.selectedMappingResult.value : Integration_MappingResultEnum.Valid.value,
            From: $scope.selectedFromDateTime,
            To: $scope.selectedToDateTime
        };

        return gridApi.retrieveData(query);
    }

    function loadMappingResults() {
        for (var prop in Integration_MappingResultEnum) {
            $scope.mappingResults.push(Integration_MappingResultEnum[prop]);
        }
    }

    function getMappingResultDescription(mappingResultValue) {
        
        var enumObj = UtilsService.getEnum(Integration_MappingResultEnum, 'value', mappingResultValue);
        if (enumObj) return enumObj.description;

        return undefined;
    }

    function getExecutionStatusDescription(executionStatusValue) {

        var enumObj = UtilsService.getEnum(Integration_ExecutionStatusEnum, 'value', executionStatusValue);
        if (enumObj) return enumObj.description;
        
        return undefined;
    }

    function getExecutionStatusColor(executionStatusValue) {

        if (executionStatusValue === Integration_ExecutionStatusEnum.New.value) return Integration_ExecutionStatusColorEnum.New.color;
        if (executionStatusValue === Integration_ExecutionStatusEnum.Processing.value) return Integration_ExecutionStatusColorEnum.Processing.color;
        if (executionStatusValue === Integration_ExecutionStatusEnum.Failed.value) return Integration_ExecutionStatusColorEnum.Failed.color;
        if (executionStatusValue === Integration_ExecutionStatusEnum.Processed.value) return Integration_ExecutionStatusColorEnum.Processed.color;
        
        return Integration_ExecutionStatusColorEnum.New.color;
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
