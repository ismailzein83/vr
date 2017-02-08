(function (appControllers) {

    "use strict";

    zoneRoutingProductManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_SaleRateAPIService'];

    function zoneRoutingProductManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, whSBeSalePriceListOwnerTypeEnum, whSBeSaleRateApiService) {

        var gridApi;
        var sellingNumberPlanDirectiveApi;

        var sellingNumberPlanReadyPromiseDeferred = utilsService.createPromiseDeferred();
        var saleZoneDirectiveApi;
        var saleZoneReadyPromiseDeferred = utilsService.createPromiseDeferred();

        var sellingProductSelectorApi;
        var sellingProductSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var carrierAccountSelectorApi;
        var carrierAccountSelectorReadyDeferred = utilsService.createPromiseDeferred();
        var saleAreaSettingsData;

        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.effectiveOn = new Date();
            $scope.searchClicked = function () {
                setFilterObject();
                return gridApi.loadGrid(filter);
            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveApi = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveApi = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onSellingNumberPlanSelectionChanged = function () {

                if (sellingNumberPlanDirectiveApi.getSelectedIds() == undefined)
                    return;

                var setSaleZoneLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };
                var saleZoneSelectorPayload = {
                    sellingNumberPlanId: sellingNumberPlanDirectiveApi.getSelectedIds()
                };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveApi, saleZoneSelectorPayload, setSaleZoneLoader);

                var sellingProductPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveApi.getSelectedIds() }
                };

                var carrierAccountPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveApi.getSelectedIds() }
                };

                var sellingProductSelectorLoadPromise = sellingProductSelectorApi.load(sellingProductPayload);
                var carrierAccountSelectorLoadPromise = carrierAccountSelectorApi.load(carrierAccountPayload);

                $scope.isLoadingOwnerSelector = true;
                utilsService.waitMultiplePromises([sellingProductSelectorLoadPromise, carrierAccountSelectorLoadPromise]).finally(function () {
                    $scope.isLoadingOwnerSelector = false;

                });
            };

            $scope.ownerTypes = utilsService.getArrayEnum(whSBeSalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.selectedSellingProduct = undefined;
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

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
        }
        function load() {
            $scope.isLoadingFilter = true;
            loadAllControls();
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([loadSellingNumberPlan, loadSellingProduct, loadCarrierAccount, loadSaleAreaSettingsData])
              .catch(function (error) {
                  vrNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
        function loadSellingNumberPlan() {
            var loadSellingNumberPlanPromiseDeferred = utilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                vruiUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveApi, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        function loadSellingProduct() {
            var sellingProductSelectorLoadDeferred = utilsService.createPromiseDeferred();

            sellingProductSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: null,
                    selectedIds: null
                };

                vruiUtilsService.callDirectiveLoad(sellingProductSelectorApi, payload, sellingProductSelectorLoadDeferred);
            });
            return sellingProductSelectorLoadDeferred.promise;
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
            filter = {
                EffectiveOn: $scope.effectiveOn,
                SellingNumberPlanId: sellingNumberPlanDirectiveApi.getSelectedIds(),
                ZonesIds: saleZoneDirectiveApi.getSelectedIds(),
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: ($scope.selectedOwnerType.value === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value) ? sellingProductSelectorApi.getSelectedIds() : carrierAccountSelectorApi.getSelectedIds(),
                SaleAreaSettings: saleAreaSettingsData
            };

        }
        function loadSaleAreaSettingsData() {
            return whSBeSaleRateApiService.GetSaleAreaSettingsData().then(function (response) {
                saleAreaSettingsData = response;
            });
        }
    }

    appControllers.controller('WhS_BE_ZoneRoutingProductManagementController', zoneRoutingProductManagementController);
})(appControllers);