(function (appControllers) {

    "use strict";

    saleRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum'];

    function saleRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_SalePriceListOwnerTypeEnum) {


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
            $scope.effectiveOn = Date.now();
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };
           
            $scope.onSellingNumberPlanSelectionChanged = function () {

                if (sellingNumberPlanDirectiveAPI.getSelectedIds() == undefined)
                    return;

                var setSaleZoneLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };
                var saleZoneSelectorPayload = {
                    sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, saleZoneSelectorPayload, setSaleZoneLoader);

                var sellingProductPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds() }
                }

                var carrierAccountPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds() }
                }

                var sellingProductSelectorLoadPromise = sellingProductSelectorAPI.load(sellingProductPayload);
                var carrierAccountSelectorLoadPromise = carrierAccountSelectorAPI.load(carrierAccountPayload);

                $scope.isLoadingOwnerSelector = true;
                UtilsService.waitMultiplePromises([sellingProductSelectorLoadPromise, carrierAccountSelectorLoadPromise]).finally()
                {
                    $scope.isLoadingOwnerSelector = false;
                };
            };

            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_BE_SalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.selectedSellingProduct = undefined;
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
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
                ;
            }
          
        }
        function load() {           
            $scope.isLoadingFilter = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlan, loadSellingProduct, loadCarrierAccount])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
      
        
        function loadSellingNumberPlan() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
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
                OwnerId: ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorAPI.getSelectedIds() : carrierAccountSelectorAPI.getSelectedIds()
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleRateManagementController', saleRateManagementController);
})(appControllers);