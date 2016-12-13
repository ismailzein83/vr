(function (appControllers) {

    "use strict";

    BusinessProcess_BP_InstanceHistoryController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceService', 'VRValidationService','VRNotificationService'];

    function BusinessProcess_BP_InstanceHistoryController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceService, VRValidationService, VRNotificationService) {
        var searchAPI;
        var searchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.onSearchReady = function (api) {
                searchAPI = api;
                searchReadyPromiseDeferred.resolve();
            };
        }
        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadSearchDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function loadSearchDirective() {
            var searchLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            searchReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(searchAPI, undefined, searchLoadPromiseDeferred);
            });
            return searchLoadPromiseDeferred.promise;
        }
    }

    appControllers.controller('BusinessProcess_BP_InstanceHistoryController', BusinessProcess_BP_InstanceHistoryController);
})(appControllers);