(function (appControllers) {

    "use strict";

    salePricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function salePricelistController($scope, utilsService, vrNotificationService, vruiUtilsService) {

        var gridApi;
        var gridQuery = {};

        var carrierAccountSelectorApi;
        var carrierAccountSelectorReadyDeferred = utilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.selectedCustomer = [];

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorApi = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridApi = api;
                var gridPayload = { query: gridQuery };
                gridApi.load(gridPayload);
            };

            $scope.searchClicked = function () {
                setGridQuery();
                var gridPayload = { query: gridQuery };
                return gridApi.load(gridPayload);
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCarrierAccount])
              .catch(function (error) {
                  vrNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
        function loadCarrierAccount() {
            var carrierAccountSelectorLoadDeferred = utilsService.createPromiseDeferred();
            carrierAccountSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                vruiUtilsService.callDirectiveLoad(carrierAccountSelectorApi, payload, carrierAccountSelectorLoadDeferred);
            });
            return carrierAccountSelectorLoadDeferred.promise;
        }

        function setGridQuery() {
            gridQuery = {
                OwnerId: carrierAccountSelectorApi.getSelectedIds(),
                CreationDate: $scope.CreationDate
            };
        }
    }

    appControllers.controller('WhS_BE_SalePricelistManagementController', salePricelistController);
})(appControllers);