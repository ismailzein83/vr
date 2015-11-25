(function (appControllers) {

    "use strict";

    saleRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Be_SaleRateOwnerTypeEnum'];

    function saleRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Be_SaleRateOwnerTypeEnum) {


        var gridAPI;
        var sellingNumberPlanDirectiveAPI;
        var saleSelectorAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }
            $scope.onSelectorReady = function (api) {
                saleSelectorAPI = api;
               
            }
            $scope.onSellingNumberPlanSelectItem = function (selectedItem) {
                    saleSelectorAPI.clearDataSource();
                    if (selectedItem != undefined) {
                        var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                        var payload = {
                            sellingNumberPlanId: (selectedItem != undefined) ? selectedItem.SellingNumberPlanId : 0,
                        }

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                    }
                    
            }

            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Be_SaleRateOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.selectedSellingProduct = undefined;
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_Be_SaleRateOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Be_SaleRateOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct = undefined;
                    }
                }
            };

           
            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
            }
          
        }
        function load() {           
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlan, loadSellingProduct, loadCarrierAccount])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }
      
        
        function loadSellingNumberPlan() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                var payload = {};
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, payload, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
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
                EffectiveOn: $scope.effectiveOn,
                SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                ZonesIds: saleZoneDirectiveAPI.getSelectedIds(),
                OwnerType:$scope.selectedOwnerType.value,
                OwnerId: ($scope.selectedOwnerType.value == WhS_Be_SaleRateOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorAPI.getSelectedIds() : carrierAccountSelectorAPI.getSelectedIds()
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleRateManagementController', saleRateManagementController);
})(appControllers);