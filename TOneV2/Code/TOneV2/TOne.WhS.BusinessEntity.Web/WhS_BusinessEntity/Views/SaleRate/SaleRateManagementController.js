(function (appControllers) {

    "use strict";

    saleRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_SaleRateAPIService', 'VRCommon_CurrencyAPIService', 'WhS_BE_CarrierAccountAPIService', 'VRDateTimeService'];

    function saleRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_SalePriceListOwnerTypeEnum, whSBeSaleRateApiService, VRCommon_CurrencyAPIService, WhS_BE_CarrierAccountAPIService, VRDateTimeService) {

        var gridAPI;
        var sellingNumberPlanDirectiveAPI;
        var saleSelectorAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var primarySaleEntity;

        var gridPayload = {};

        var systemCurrencyId;
        var customerCurrencyId;

        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
            $scope.searchClicked = function () {
                setGridPayload();
                return gridAPI.load(gridPayload);
            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.onSellingNumberPlanSelectionChanged = function () {

                if (sellingNumberPlanDirectiveAPI.getSelectedIds() == undefined)
                    return;

                var sellingProductPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds() }
                };

                var carrierAccountPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds() }
                };

                var sellingProductSelectorLoadPromise = sellingProductSelectorAPI.load(sellingProductPayload);
                var carrierAccountSelectorLoadPromise = carrierAccountSelectorAPI.load(carrierAccountPayload);

                $scope.isLoadingOwnerSelector = true;
                UtilsService.waitMultiplePromises([sellingProductSelectorLoadPromise, carrierAccountSelectorLoadPromise]).finally(function () {
                    $scope.isLoadingOwnerSelector = false;

                });
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
            };

            $scope.onCustomerSelected = function (selectedCustomer) {
                $scope.isLoadingFilter = true;
                getCustomerCurrencyId(selectedCustomer.CarrierAccountId).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilter = false;
                });
            };
        }
        function load() {
            $scope.isLoadingFilter = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlan, loadSellingProduct, loadCarrierAccount, loadPrimarySaleEntity, getSystemCurrencyId, loadCountrySelector])
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
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, { selectifsingleitem: true }, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        function loadSellingProduct() {
            var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            sellingProductSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, { selectifsingleitem: true }, sellingProductSelectorLoadDeferred);
            });
            return sellingProductSelectorLoadDeferred.promise;
        }
        function loadCarrierAccount() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            carrierAccountSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
            });
            return carrierAccountSelectorLoadDeferred.promise;
        }
        function loadPrimarySaleEntity() {
            return whSBeSaleRateApiService.GetPrimarySaleEntity().then(function (response) {
                primarySaleEntity = response;
            });
        }

        function getSystemCurrencyId() {
            return VRCommon_CurrencyAPIService.GetSystemCurrencyId().then(function (response) {
                systemCurrencyId = response;
            });
        }
        function getCustomerCurrencyId(customerId) {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrencyId(customerId).then(function (response) {
                customerCurrencyId = response;
            });
        }

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function setGridPayload() {
            gridPayload = {
                query: {
                    EffectiveOn: $scope.effectiveOn,
                    CountriesIds: countryDirectiveApi.getSelectedIds(),
                    SaleZoneName: $scope.saleZoneName,
                    OwnerType: $scope.selectedOwnerType.value,
                    SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                    IsSystemCurrency: $scope.isSystemCurrency
                }
            };
            if ($scope.selectedOwnerType.value == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                gridPayload.query.OwnerId = sellingProductSelectorAPI.getSelectedIds();
                gridPayload.query.CurrencyId = systemCurrencyId;
            }
            else {
                gridPayload.query.OwnerId = carrierAccountSelectorAPI.getSelectedIds();
                gridPayload.query.CurrencyId = customerCurrencyId;
            }
            gridPayload.primarySaleEntity = primarySaleEntity;
        }
    }

    appControllers.controller('WhS_BE_SaleRateManagementController', saleRateManagementController);
})(appControllers);