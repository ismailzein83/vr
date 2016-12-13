DataSourceLogManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

function DataSourceLogManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {

    var searchApi;
    var searchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();
    function defineScope() {      
        $scope.onSearchReady = function (api) {
            searchApi = api;
            searchReadyPromiseDeferred.resolve();

        };
    }
    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSearchDirective]).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isLoading = false;
        }).finally(function () {
            $scope.isLoading = false;
        });
    }
    function loadSearchDirective() {
        var searchLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        searchReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(searchApi, undefined, searchLoadPromiseDeferred);
            });
        return searchLoadPromiseDeferred.promise;
    }
}

appControllers.controller('Integration_DataSourceLogManagementController', DataSourceLogManagementController);