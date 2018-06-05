(function (appControllers) {

    "use strict";

    receivedSupplierPriceListController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_SupPL_ReceivedPriceListStatusEnum'];

    function receivedSupplierPriceListController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_SupPL_ReceivedPriceListStatusEnum) {

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedStatus = [];

            $scope.scopeModel.Status = UtilsService.getArrayEnum(WhS_SupPL_ReceivedPriceListStatusEnum);

            $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };
        }

        function load() {
          //  $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector])
           .catch(function (error) {
               $scope.scopeModel.isLoading = false;
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function loadCarrierAccountSelector() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });
            return loadCarrierAccountPromiseDeferred.promise;
        }


    }
    appControllers.controller('WhS_SupPL_ReceivedSupplierPriceListController', receivedSupplierPriceListController);
})(appControllers);