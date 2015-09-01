QueueItemHeaderGridController.$inject = ['$scope', 'DataSourceImportedBatchAPIService', 'DataSourceService', 'VRNotificationService'];

function QueueItemHeaderGridController($scope, DataSourceImportedBatchAPIService, DataSourceService, VRNotificationService) {

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

                    angular.forEach(response.Data, function (item) {
                        item.ExecutionStatusDescription = DataSourceService.getExecutionStatusDescription(item.Status);
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.getStatusColor = function (dataItem, colDef) {
            return DataSourceService.getExecutionStatusColor(dataItem.Status);
        }
    };

    function load() {

    };

    function retrieveData() {

        var itemIds = $scope.dataItem.QueueItemIds.split(',').map(Number)
        return gridApi.retrieveData(itemIds);
    }
}

appControllers.controller('Integration_QueueItemHeaderGridController', QueueItemHeaderGridController);