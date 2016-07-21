(function (appControllers) {

    'use strict';

    DealManagementController.$inject = ['$scope', 'WhS_BE_DealService', 'WhS_BE_DealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DealManagementController($scope, WhS_BE_DealService, WhS_BE_DealAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};


            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };


            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                api.load({});
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.add = function () {
                var onDealAdded = function (addedDeal) {
                    gridAPI.onDealAdded(addedDeal);
                };
                WhS_BE_DealService.addDeal(onDealAdded);
            };

            $scope.scopeModel.hasAddPermission = function () {
                return WhS_BE_DealAPIService.HasAddDealPermission();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                CarrierAccountIds: carrierAccountSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('WhS_BE_DealManagementController', DealManagementController);

})(appControllers);