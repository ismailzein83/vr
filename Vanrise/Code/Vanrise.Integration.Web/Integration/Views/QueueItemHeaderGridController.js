QueueItemHeaderGridController.$inject = ['$scope', 'DataSourceImportedBatchAPIService', 'Integration_ExecutionStatusEnum', 'Integration_ExecutionStatusColorEnum', 'UtilsService', 'VRNotificationService'];

function QueueItemHeaderGridController($scope, DataSourceImportedBatchAPIService, Integration_ExecutionStatusEnum, Integration_ExecutionStatusColorEnum, UtilsService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {

        $scope.queueItemHeaders = [];

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DataSourceImportedBatchAPIService.GetQueueItemHeaders(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);

                    angular.forEach(response.Data, function (item) {
                        item.ExecutionStatusDescription = getExecutionStatusDescription(item.Status);
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.getStatusColor = function (dataItem, colDef) {
            return getExecutionStatusColor(dataItem.Status);
        }
    };

    function load() {

    };

    function retrieveData() {
        var query = {
            itemIds: $scope.dataItem.QueueItemIds.split(',').map(Number)
        };
        
        return gridApi.retrieveData(query);
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

appControllers.controller('Integration_QueueItemHeaderGridController', QueueItemHeaderGridController);