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
        if (gridApi == undefined || filtersAreNotReady) return;

        var query = {
            DataSourceId: $scope.selectedDataSource.DataSourceId,
            BatchName: ($scope.batchName != undefined) ? $scope.batchName : null,
            MappingResults: getMappedMappingResults(),
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

    function getMappedMappingResults() {

        if ($scope.selectedMappingResults.length == 0) {
            // select all
            $scope.selectedMappingResults = getArrayEnum(Integration_MappingResultEnum);
        }

        var mappedMappingResults = [];

        for (var i = 0; i < $scope.selectedMappingResults.length; i++) {
            mappedMappingResults.push($scope.selectedMappingResults[i].value);
        }

        return mappedMappingResults;
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

    function getArrayEnum(enumObj) {
        var array = [];

        for (var item in enumObj) {
            if (enumObj.hasOwnProperty(item)) {
                array.push(enumObj[item]);
            }
        }

        return array;
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
