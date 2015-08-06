DataSourceImportedBatchManagementController.$inject = ['$scope', 'DataSourceAPIService', 'UtilsService', 'VRNotificationService'];

function DataSourceImportedBatchManagementController($scope, DataSourceAPIService, UtilsService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {
        $scope.dataSources = [];
        $scope.selectedDataSources = [];
        $scope.mappingResults = [];
        $scope.selectedMappingResults = [];
        $scope.selectedFromDateTime = undefined;
        $scope.selectedToDateTime = undefined;
    }

    function load() {
        $scope.isLoadingForm = true;

        UtilsService.waitMultipleAsyncOperations([loadDataSources, loadMappingResults])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingForm = false;
            });
    }

    function loadDataSources() {
        return DataSourceAPIService.GetDataSources()
            .then(function (response) {
                $scope.dataSources = response;
            });
    }

    function loadMappingResults() {
        // to be removed
        $scope.mappingResults.push({ value: 1, description: 'Valid' });
        $scope.mappingResults.push({ value: 2, description: 'Invalid' });
        $scope.mappingResults.push({ value: 3, description: 'Empty' });
    }
}

appControllers.controller('Integration_DataSourceImportedBatchManagementController', DataSourceImportedBatchManagementController);
