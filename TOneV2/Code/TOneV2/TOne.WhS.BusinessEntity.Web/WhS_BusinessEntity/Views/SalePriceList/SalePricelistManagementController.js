(function (appControllers) {

    "use strict";

    salePricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function salePricelistController($scope, utilsService, vrNotificationService, vruiUtilsService) {

        var gridApi;
        var gridQuery = {};
        var gridContext;

        var carrierAccountSelectorApi;
        var carrierAccountSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var salePricelistTypeSelectorApi;
        var salePricelistTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var userSelectorApi;
        var userSelectorReadyDeferred = utilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.selectedCustomer = [];

            setGridContext();

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorApi = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.onSalePricelistTypeSelectorReady = function (api) {
                salePricelistTypeSelectorApi = api;
                salePricelistTypeSelectorReadyDeferred.resolve();
            };

            $scope.onUserSelectorReady = function (api) {
                userSelectorApi = api;
                userSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridApi = api;
                loadSalePriceListGrid();
            };

            $scope.searchClicked = function () {
                return loadSalePriceListGrid();
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCarrierAccount,loadSalePricelistType,loadUserSelector])
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
        function loadSalePricelistType() {
            var salePricelistTypeSelectorLoadDeferred = utilsService.createPromiseDeferred();
            salePricelistTypeSelectorReadyDeferred.promise.then(function () {
                var payload = {};
                vruiUtilsService.callDirectiveLoad(salePricelistTypeSelectorApi, payload, salePricelistTypeSelectorLoadDeferred);
            });
            return salePricelistTypeSelectorLoadDeferred.promise;
        }

        function loadUserSelector() {
            var userSelectorLoadDeferred = utilsService.createPromiseDeferred();
            userSelectorReadyDeferred.promise.then(function () {
                var userSelectorPayload = {};
                vruiUtilsService.callDirectiveLoad(userSelectorApi, userSelectorPayload, userSelectorLoadDeferred);
            });
            return userSelectorLoadDeferred.promise;
        }

        function loadSalePriceListGrid() {
            setGridQuery();

            var gridPayload = {
                query: gridQuery,
                context: gridContext,
                HideSelectedColumn: true,
            };

            return gridApi.load(gridPayload);
        }

        function setGridQuery() {
            gridQuery = {
                OwnerId: carrierAccountSelectorApi.getSelectedIds(),
                CreationDate: $scope.CreationDate,
                SalePricelistTypes: salePricelistTypeSelectorApi.getSelectedIds(),
                UserIds: userSelectorApi.getSelectedIds()
            };
        }
        function setGridContext() {
            gridContext = {
                onSalePriceListPreviewClosed: function () {
                    return loadSalePriceListGrid();
                }
            };
        }
    }

    appControllers.controller('WhS_BE_SalePricelistManagementController', salePricelistController);

})(appControllers);