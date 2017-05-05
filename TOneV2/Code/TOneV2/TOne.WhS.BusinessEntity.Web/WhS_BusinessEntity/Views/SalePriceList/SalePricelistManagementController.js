(function (appControllers) {

    "use strict";

    salePricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function salePricelistController($scope, utilsService, vrNotificationService, vruiUtilsService) {

        var gridApi;
        var filter = {};

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
                gridApi.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                setFilterObject();
                return gridApi.loadGrid(filter);
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

        function setFilterObject() {
            filter = {};
            filter.OwnerId = carrierAccountSelectorApi.getSelectedIds();
            filter.CreationDate = $scope.CreationDate;
        }
    }

    appControllers.controller('WhS_BE_SalePricelistManagementController', salePricelistController);
})(appControllers);