﻿(function (appControllers) {

    "use strict";

    rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RoutingProductFilterEnum'];

    function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RoutingProductFilterEnum) {

        var currencyId;

        var gridAPI;

        var routingDatabaseSelectorAPI;
        var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rpRoutePolicyAPI;

        var saleZoneSelectorAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var singleRoutingProductSelectorAPI;
        var singleRoutingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var multipleRoutingProductSelectorAPI;
        var multipleRoutingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeStatusSelectorAPI;
        var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var customerSelectorAPI;
        var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();



        defineScope();
        load();

        function defineScope() {
            $scope.numberOfOptions = 3;

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.onRoutingDatabaseSelectorReady = function (api) {
                routingDatabaseSelectorAPI = api;
                routingDatabaseReadyPromiseDeferred.resolve();
            };

            $scope.onRPRoutePolicySelectorReady = function (api) {
                rpRoutePolicyAPI = api;
            };

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneSelectorAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onMultipleRoutingProductSelectorReady = function (api) {
                multipleRoutingProductSelectorAPI = api;
                multipleRoutingProductReadyPromiseDeferred.resolve();
            };

            $scope.onSingleRoutingProductSelectorReady = function (api) {
                singleRoutingProductSelectorAPI = api;
                singleRoutingProductReadyPromiseDeferred.resolve();
            };

            $scope.onRouteStatusDirectiveReady = function (api) {
                routeStatusSelectorAPI = api;
                routeStatusSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onCustomerSelectorReady = function (api) {
                customerSelectorAPI = api;
                customerSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onRoutingProductFilterSelectorReady = function (api) {
            };

            $scope.onRoutingDatabaseSelectorChange = function () {
                var selectedId = routingDatabaseSelectorAPI.getSelectedIds();

                if (selectedId == undefined)
                    return;

                var policySelectorPayload = {
                    filter: {
                        RoutingDatabaseId: selectedId
                    },
                    selectDefaultPolicy: true
                };

                var setLoader = function (value) {
                    $scope.isLoadingFilterData = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rpRoutePolicyAPI, policySelectorPayload, setLoader, undefined);
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            function getFilterObject() {
                currencyId = null;

                if (!$scope.showInSystemCurrency && $scope.selectedCustomer != undefined)
                    currencyId = $scope.selectedCustomer.CurrencyId;

                var query = {
                    RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
                    PolicyConfigId: rpRoutePolicyAPI.getSelectedIds(),
                    NumberOfOptions: $scope.numberOfOptions,
                    RoutingProductIds: $scope.selectedRoutingProductFilter.value == 0 || $scope.selectedCustomer == undefined ? multipleRoutingProductSelectorAPI.getSelectedIds() : undefined,
                    SimulatedRoutingProductId: $scope.selectedRoutingProductFilter.value == 1 && $scope.selectedCustomer != undefined ? singleRoutingProductSelectorAPI.getSelectedIds() : undefined,
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                    FilteredPolicies: rpRoutePolicyAPI.getFilteredPoliciesIds(),
                    DefaultPolicyId: rpRoutePolicyAPI.getDefaultPolicyId(),
                    RouteStatus: routeStatusSelectorAPI.getSelectedIds(),
                    LimitResult: $scope.limit,
                    CustomerId: customerSelectorAPI.getSelectedIds(),
                    ShowInSystemCurrency: $scope.showInSystemCurrency,
                    CurrencyId: currencyId,
                    IncludeBlockedSuppliers: $scope.includeBlockedSuppliers
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.limit = 1000;

            $scope.routingProductFilters = UtilsService.getArrayEnum(WhS_Routing_RoutingProductFilterEnum);
            $scope.selectedRoutingProductFilter = $scope.routingProductFilters[0];

            return UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadSingleRoutingProductSelector, loadMultipleRoutingProductSelector, loadSaleZoneSection, loadRouteStatusSelector, loadCustomerSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function loadRoutingDatabaseSelector() {
            var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

            routingDatabaseReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, undefined, loadRoutingDatabasePromiseDeferred);
            });

            return loadRoutingDatabasePromiseDeferred.promise;
        }
        function loadSingleRoutingProductSelector() {
            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

            singleRoutingProductReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(singleRoutingProductSelectorAPI, undefined, loadRoutingProductPromiseDeferred);
            });

            return loadRoutingProductPromiseDeferred.promise;
        }

        function loadMultipleRoutingProductSelector() {
            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

            multipleRoutingProductReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(multipleRoutingProductSelectorAPI, undefined, loadRoutingProductPromiseDeferred);
            });

            return loadRoutingProductPromiseDeferred.promise;
        }

        function loadSaleZoneSection() {
            var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, undefined, loadSaleZonePromiseDeferred);
            });

            return loadSaleZonePromiseDeferred.promise;
        }
        function loadRouteStatusSelector() {
            var loadRouteStatusPromiseDeferred = UtilsService.createPromiseDeferred();

            routeStatusSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeStatusSelectorAPI, undefined, loadRouteStatusPromiseDeferred);
            });

            return loadRouteStatusPromiseDeferred.promise;
        }
        function loadCustomerSelector() {
            var loadCustomerPromiseDeferred = UtilsService.createPromiseDeferred();

            customerSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(customerSelectorAPI, undefined, loadCustomerPromiseDeferred);
            });

            return loadCustomerPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_Routing_RPRouteManagementController', rpRouteManagementController);
})(appControllers);