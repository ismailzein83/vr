(function (appControllers) {

    "use strict";

    rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RoutingProductFilterEnum', 'WhS_Routing_RouteFilterEnum'];

    function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RoutingProductFilterEnum, WhS_Routing_RouteFilterEnum) {

        var currencyId;

        var gridAPI;
        var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.showRPRouteGrid = true;

            $scope.numberOfOptions = 3;
            $scope.legendHeader = "Legend";
            $scope.legendContent = getLegendContent();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridReadyPromiseDeferred.resolve();
            };

            $scope.onGridByCodeReady = function (api) {
                gridAPI = api;
                gridReadyPromiseDeferred.resolve();
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

            $scope.onRouteFilterSelectorReady = function (api) {
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
                gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var oldShowRPRouteGrid = $scope.showRPRouteGrid;
                $scope.showRPRouteGrid = $scope.selectedRouteFilter == WhS_Routing_RouteFilterEnum.Zone;
                if (oldShowRPRouteGrid != $scope.showRPRouteGrid)
                    gridAPI = undefined;

                if (gridAPI != undefined) {
                    return gridAPI.loadGrid(getFilterObject());
                }
                else {
                    var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
                    gridReadyPromiseDeferred.promise.then(function () {
                        gridAPI.loadGrid(getFilterObject()).then(function () {
                            loadGridPromiseDeferred.resolve();
                        });
                    });
                    return loadGridPromiseDeferred.promise;
                }
            };

            function getLegendContent() {
                return '<div style="font-size:12px; margin:10px">' +
                            '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FF0000; margin: 0px 3px"></div> Blocked </div>' +
                            '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FFA500; margin: 0px 3px"></div> Lossy </div>' +
                            '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #0000FF; margin: 0px 3px"></div> Forced </div>' +
                            '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #28A744; margin: 0px 3px"></div> Market Price </div>' +
                        '</div>';
            }

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
                    IncludeBlockedSuppliers: $scope.includeBlockedSuppliers,
                    MaxSupplierRate: $scope.maxSupplierRate,
                    CodePrefix: $scope.codePrefix
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.limit = 1000;
            $scope.includeBlockedSuppliers = true;

            $scope.routingProductFilters = UtilsService.getArrayEnum(WhS_Routing_RoutingProductFilterEnum);
            $scope.selectedRoutingProductFilter = $scope.routingProductFilters[0];

            $scope.routeFilters = UtilsService.getArrayEnum(WhS_Routing_RouteFilterEnum);
            $scope.selectedRouteFilter = $scope.routeFilters[0];

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