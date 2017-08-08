(function (appControllers) {

    "use strict";

    supplierTargetMatchController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];

    function supplierTargetMatchController($scope, UtilsService, VRUIUtilsService) {
        var gridAPI;

        var targetMatchFilterDirectiveAPI;
        var targetMatchFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.scopeModel = {};
            $scope.loadClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.ontargetMatchFilterDirectiveReady = function (api) {
                targetMatchFilterDirectiveAPI = api;
                targetMatchFilterReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.isLoadingFilter = true;

            UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans, loadCarrierAccountSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }



        function loadSellingNumberPlans() {
            var loadSupplierTargetMatchPromiseDeferred = UtilsService.createPromiseDeferred();
            targetMatchFilterReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(targetMatchFilterDirectiveAPI, undefined, loadSNPPromiseDeferred);
            });

            return loadSNPPromiseDeferred.promise;
        }

        function loadCarrierAccountSelector() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred)
            });

            return loadCarrierAccountPromiseDeferred.promise;

        }

        function setFilterObject() {
            filter = {
                threshold: $scope.threshold,
                sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                supplierIds: carrierAccountDirectiveAPI.getSelectedIds(),
                codeStartWith: ($scope.codeStartWith != null) ? $scope.codeStartWith : null

            };
        }

    }

    appControllers.controller('WhS_Sales_SupplierTargetMatchController', supplierTargetMatchController);
})(appControllers);