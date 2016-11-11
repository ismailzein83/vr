(function (appControllers) {

    "use strict";

    salePricelistController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'VRUIUtilsService'];

    function salePricelistController($scope, UtilsService, VRNotificationService, WhS_BE_SalePriceListOwnerTypeEnum, VRUIUtilsService) {


        var gridApi;
        var filter = {};

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();

        load();

        function defineScope() {


            $scope.onGridReady = function (api) {
                gridApi = api;
                gridApi.loadGrid(filter);
            };




            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_BE_SalePriceListOwnerTypeEnum);

            $scope.selectedSellingProduct = [];
            $scope.selectedCustomer = [];

            $scope.isRequiredSellingProductSelector = false;
            $scope.isRequiredCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer.length = 0;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct.length = 0;
                    }
                }
            };

            $scope.searchClicked = function () {
                setFilterObject();
                return gridApi.loadGrid(filter);
            };


            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();

            };

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };


        }


        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return UtilsService.waitMultipleAsyncOperations([loadSellingProduct, loadCarrierAccount])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }



        function loadSellingProduct() {
            var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            sellingProductSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, payload, sellingProductSelectorLoadDeferred);
            });
            return sellingProductSelectorLoadDeferred.promise;
        }


        function loadCarrierAccount() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            carrierAccountSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
            });
            return carrierAccountSelectorLoadDeferred.promise;
        }

        function setFilterObject() {
            filter = {
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorAPI.getSelectedIds() : carrierAccountSelectorAPI.getSelectedIds()

            };

        }




    }

    appControllers.controller('WhS_BE_SalePricelistController', salePricelistController);
})(appControllers);