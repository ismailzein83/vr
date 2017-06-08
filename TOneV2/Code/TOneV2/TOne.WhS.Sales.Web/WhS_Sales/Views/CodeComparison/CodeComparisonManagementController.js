(function (appControllers) {

    "use strict";

    codeCompareManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];

    function codeCompareManagementController($scope, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        

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
            $scope.onSellingNumberPlanSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
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
            var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSNPPromiseDeferred);
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

    appControllers.controller('WhS_Sales_CodeCompareManagementController', codeCompareManagementController);
})(appControllers);