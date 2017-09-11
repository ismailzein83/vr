(function (appControllers) {

    "use strict";

    supplierTargetMatchController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function supplierTargetMatchController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        var targetMatchFilterDirectiveAPI;
        var targetMatchFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        
        function defineScope() {
            $scope.scopeModel = {};
            $scope.loadClicked = function () {
                $scope.showExportButton = true;
                return gridAPI.load(getFilter());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.onTargetMatchFilterDirectiveReady = function (api) {
                targetMatchFilterDirectiveAPI = api;
                targetMatchFilterReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.isLoadingFilter = true;

            UtilsService.waitMultipleAsyncOperations([loadTargetMatchFilter]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }



        function loadTargetMatchFilter() {
            var loadTargetMatchFilterDeferred = UtilsService.createPromiseDeferred();
            targetMatchFilterReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(targetMatchFilterDirectiveAPI, undefined, loadTargetMatchFilterDeferred);
            });

            return loadTargetMatchFilterDeferred.promise;
        }

        function getFilter() {
            return targetMatchFilterDirectiveAPI.getData();
        }
    }

    appControllers.controller('WhS_Sales_SupplierTargetMatchController', supplierTargetMatchController);
})(appControllers);