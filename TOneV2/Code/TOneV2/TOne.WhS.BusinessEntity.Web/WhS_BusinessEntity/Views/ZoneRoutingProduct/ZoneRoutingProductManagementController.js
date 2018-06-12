(function (appControllers) {

    "use strict";

    zoneRoutingProductManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_SaleRateAPIService', 'VRDateTimeService'];

    function zoneRoutingProductManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, whSBeSalePriceListOwnerTypeEnum, whSBeSaleRateApiService, VRDateTimeService) {

        var primarySaleEntity;
        var filter = {};

        var sellingNumberPlanSelectorApi;
        var sellingNumberPlanSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var saleZoneSelectorApi;
        var saleZoneSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var sellingProductSelectorApi;
        var sellingProductSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var carrierAccountSelectorApi;
        var carrierAccountSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var gridApi;


        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();
            $scope.ownerTypes = utilsService.getArrayEnum(whSBeSalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.selectedSellingProduct = undefined;
            $scope.showSellingNumberPlanSelector = true;
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanSelectorApi = api;
                sellingNumberPlanSelectorReadyDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneSelectorApi = api;
                saleZoneSelectorReadyDeferred.resolve();
            };

            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorApi = api;
                sellingProductSelectorReadyDeferred.resolve();
            };

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorApi = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridApi = api;
            };

            $scope.onSellingNumberPlanSelectionChanged = function () {

                if (sellingNumberPlanSelectorApi.getSelectedIds() == undefined)
                    return;

                var setSaleZoneLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };
                var saleZoneSelectorPayload = {
                    sellingNumberPlanId: sellingNumberPlanSelectorApi.getSelectedIds()
                };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorApi, saleZoneSelectorPayload, setSaleZoneLoader);

                var sellingProductPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanSelectorApi.getSelectedIds() }
                };
                var sellingProductSelectorLoadPromise = sellingProductSelectorApi.load(sellingProductPayload);

                var carrierAccountPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanSelectorApi.getSelectedIds() }
                };
                var carrierAccountSelectorLoadPromise = carrierAccountSelectorApi.load(carrierAccountPayload);

                $scope.isLoadingOwnerSelector = true;
                utilsService.waitMultiplePromises([sellingProductSelectorLoadPromise, carrierAccountSelectorLoadPromise]).finally(function () {
                    $scope.isLoadingOwnerSelector = false;
                });
            };

            $scope.onOwnerTypeChanged = function () {
                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value === whSBeSalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct = undefined;
                    }
                }
            };

            $scope.searchClicked = function () {
                setFilterObject();
                return gridApi.load(filter);
            };
        }
        function load() {
            $scope.isLoadingFilter = true;
            loadAllControls();
        }

        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, loadSellingProductSelector, loadCarrierAccountSelector, loadSaleAreaSettingsData])
                      .catch(function (error) {
                          vrNotificationService.notifyExceptionWithClose(error, $scope);
                      })
                      .finally(function () {
                          $scope.isLoadingFilter = false;
                      });
        }
        function loadSellingNumberPlanSelector() {
            var loadSellingNumberPlanPromiseDeferred = utilsService.createPromiseDeferred();

            sellingNumberPlanSelectorReadyDeferred.promise.then(function () {
                var sellingNumberPlanSelectorPayload = { selectifsingleitem: true };
                vruiUtilsService.callDirectiveLoad(sellingNumberPlanSelectorApi, sellingNumberPlanSelectorPayload, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise.then(function () {
                if (sellingNumberPlanSelectorApi.hasSingleItem()) {
                    $scope.showSellingNumberPlanSelector = false;
                }
            });
        }
        function loadSellingProductSelector() {
            var sellingProductSelectorLoadDeferred = utilsService.createPromiseDeferred();

            sellingProductSelectorReadyDeferred.promise.then(function () {
                vruiUtilsService.callDirectiveLoad(sellingProductSelectorApi, undefined, sellingProductSelectorLoadDeferred);
            });
            return sellingProductSelectorLoadDeferred.promise;
        }
        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = utilsService.createPromiseDeferred();
            carrierAccountSelectorReadyDeferred.promise.then(function () {
                vruiUtilsService.callDirectiveLoad(carrierAccountSelectorApi, undefined, carrierAccountSelectorLoadDeferred);
            });
            return carrierAccountSelectorLoadDeferred.promise;
        }
        function loadSaleAreaSettingsData() {
            return whSBeSaleRateApiService.GetPrimarySaleEntity().then(function (response) {
                primarySaleEntity = response;
            });
        }

        function setFilterObject() {
            filter = {
                EffectiveOn: $scope.effectiveOn,
                SellingNumberPlanId: sellingNumberPlanSelectorApi.getSelectedIds(),
                ZonesIds: saleZoneSelectorApi.getSelectedIds(),
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: ($scope.selectedOwnerType.value === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorApi.getSelectedIds() : carrierAccountSelectorApi.getSelectedIds(),
                PrimarySaleEntity: primarySaleEntity
            };
        }
    }

    appControllers.controller('WhS_BE_ZoneRoutingProductManagementController', zoneRoutingProductManagementController);
})(appControllers);