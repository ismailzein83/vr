(function (appControllers) {

    "use strict";

    customerSoldZonesManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRDateTimeService'];

    function customerSoldZonesManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {

        var gridAPI;
        var sellingNumberPlanSelectorAPI;
        var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var customerApi;
        var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routingProductApi;
        var routingProductSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var zoneSelectorDirectiveAPI;
        var zoneSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.top = 6;
            $scope.scopeModel.effectiveOn = VRDateTimeService.getNowDateTime();
            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(buildGridQuery());
            };
            $scope.scopeModel.onSellingNumberReady = function (api) {
                sellingNumberPlanSelectorAPI = api;
                
                sellingReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSellingNumberPlanSelectionChanged = function () {
                $scope.scopeModel.selectedCustomers = [];
                $scope.scopeModel.selectedRoutingProducts = [];
                if (countryDirectiveApi != undefined) {
                    $scope.scopeModel.country.length = 0;
                }
                if (sellingNumberPlanSelectorAPI.getSelectedIds() == undefined) {
                    return;
                }
                var customerPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds() }
                };
                var routingProductPayload = {
                    filter: { SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds() }
                };
                var carrierAccountSelectorLoadPromise = customerApi.load(customerPayload);
                var routingProductSelectorLoadPromise = routingProductApi.load(routingProductPayload);

                $scope.scopeModel.isLoadingCustomerSelector = true;
                $scope.scopeModel.isLoadingRoutingProductSelector = true;


                UtilsService.waitMultiplePromises([carrierAccountSelectorLoadPromise, routingProductSelectorLoadPromise]).finally(function () {
                    $scope.scopeModel.isLoadingCustomerSelector = false;
                    $scope.scopeModel.isLoadingRoutingProductSelector = false;
                });
            };
            $scope.onSaleZoneDirectiveReady = function (api) {
                zoneSelectorDirectiveAPI = api;
                zoneSelectorReadyPromiseDeferred.resolve();

            };
            $scope.scopeModel.onCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSelector = value;
                };
                var payload = {
                        sellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds(),
                            filter: {
                                CountryIds: countryDirectiveApi.getSelectedIds(),
                },
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneSelectorDirectiveAPI, payload, setLoader);
                }
                if (zoneSelectorDirectiveAPI != undefined)
                    $scope.salezones.length = 0;
            };
            $scope.scopeModel.onCustomerSelectorReady = function (api) {
                customerApi = api;
                customerSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRoutingProductSelectorReady = function (api) {
                routingProductApi = api;
                routingProductSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;

            };

        }
        function load() {
            $scope.scopeModel.isGettingData = true;
            loadAllControls();

        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadSellingSelector, loadCurrencySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isGettingData = false;
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
        function loadSellingSelector() {
            var sellingLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = { selectifsingleitem: true };
                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, directivePayload, sellingLoadPromiseDeferred);
                });
            return sellingLoadPromiseDeferred.promise;
        }
        function loadCurrencySelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            currencySelectorReadyDeferred.promise.then(function () {
                var payload = {
                    selectSystemCurrency: true
                };
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, currencySelectorLoadDeferred);
            });

            return currencySelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                ZoneIds: zoneSelectorDirectiveAPI.getSelectedIds(),
                Code: $scope.scopeModel.saleCode,
                SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds(),
                CountriesIds: countryDirectiveApi.getSelectedIds(),
                CustomersIds: customerApi.getSelectedIds(),
                RoutingProductsIds:routingProductApi.getSelectedIds(),
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                Top: $scope.scopeModel.top
            };

        }

    }

    appControllers.controller('WhS_BE_CustomerSoldZonesManagementController', customerSoldZonesManagementController);
})(appControllers);