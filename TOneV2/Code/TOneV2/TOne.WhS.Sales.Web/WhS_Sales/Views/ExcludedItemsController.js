(function (appControllers) {

    "use strict";

    ExcludedItemsController.$inject = ["$scope", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService"];

    function ExcludedItemsController($scope, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService) {

        var processInstanceId;

        var excludedItemsAPI;
        var excludedItemsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                processInstanceId = parameters.subscriberProcessInstanceId;
            }
        }

        function defineScope() {
            $scope.title = 'Excluded Items';
            $scope.onExcludedItemsGridReady = function (api) {
                excludedItemsAPI = api;
                excludedItemsReadyPromiseDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadGrid])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadGrid() {
            var loadExcludedItemsPromiseDeferred = UtilsService.createPromiseDeferred();
            var gridPayload =
            {
                ProcessInstanceId : processInstanceId
            };
            excludedItemsReadyPromiseDeferred.promise.then(function () {
              
                VRUIUtilsService.callDirectiveLoad(excludedItemsAPI, gridPayload, loadExcludedItemsPromiseDeferred);
            });
            return loadExcludedItemsPromiseDeferred.promise;
        }
    }

    appControllers.controller("WhS_Sales_ExcludedItemsController", ExcludedItemsController);

})(appControllers);
