(function (appControllers) {

    "use strict";

    customerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_EntityFilterEffectiveModeEnum', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_UtilsService'];

    function customerRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VRCommon_EntityFilterEffectiveModeEnum, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_UtilsService) {

        var parametersCustomersIds;
        var parametersSuppliersIds;
        var parametersZoneIds;
        var parametresSaleCode;
        var loadGrid;

        var gridAPI;
        var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var gridFiltersAPI;
        var gridFiltersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        var selectedRoutingDatabaseObject;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                parametersCustomersIds = parameters.CustomersIds;
                parametersSuppliersIds = parameters.SuppliersIds;
                parametersZoneIds = parameters.ZoneIds;
                parametresSaleCode = parameters.SaleCode;
                loadGrid = true;
            }
        }

        function defineScope() {
            $scope.legendHeader = "Legend";
            $scope.legendContent = WhS_Routing_UtilsService.getLegendContent();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridReadyPromiseDeferred.resolve();
            };

            $scope.onGridFiltersReady = function (api) {
                gridFiltersAPI = api;
                gridFiltersReadyPromiseDeferred.resolve();
            };

            $scope.onRoutingDatabaseSelectorReady = function (api) {
                routingDatabaseSelectorAPI = api;
                routingDatabaseReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneSelectorAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierCarrierAccountDirectiveReady = function (api) {
                supplierCarrierAccountDirectiveAPI = api;
                supplierCarrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onRouteStatusDirectiveReady = function (api) {
                routeStatusSelectorAPI = api;
                routeStatusSelectorReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(gridFiltersAPI.getData());
            };

            function getFilterObject() {

                var query = {
                    isDatabaseTypeCurrent: routingDatabaseSelectorAPI.isDatabaseTypeCurrent(),
                    RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
                    SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SupplierIds: supplierCarrierAccountDirectiveAPI.getSelectedIds(),
                    RouteStatus: routeStatusSelectorAPI.getSelectedIds(),
                    LimitResult: $scope.limit,
                    IncludeBlockedSuppliers: $scope.includeBlockedSuppliers
                };
                return query;
            }
        }
        function load() {
            $scope.limit = 1000;
            $scope.isLoading = true;
            $scope.includeBlockedSuppliers = true;

            $scope.onRoutingDatabaseSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {
                    if (routingDatabaseSelectPromiseDeferred != undefined) {
                        routingDatabaseSelectPromiseDeferred.resolve();
                    } else {
                        var databaseType = getDatabaseEffectiveType(selectedItem);
                        var payload = { effectiveMode: databaseType };
                        saleZoneSelectorAPI.reLoadSaleZoneSelector(payload);
                    }
                }
            };

            var promiseDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultipleAsyncOperations([loadGridFilters]).then(function () {
                gridReadyPromiseDeferred.promise.then(function () {
                    if (loadGrid) {
                        $scope.searchClicked();
                    }
                    promiseDeferred.resolve();
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

            return promiseDeferred.promise.then(function () {
                $scope.isLoading = false;
            });
        }

        function loadGridFilters() {
            var loadGridFiltersPromiseDeferred = UtilsService.createPromiseDeferred();

            var payload = {
                zoneIds: parametersZoneIds,
                customersIds: parametersCustomersIds,
                suppliersIds: parametersSuppliersIds,
                saleCode: parametresSaleCode
            };

            gridFiltersReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(gridFiltersAPI, payload, loadGridFiltersPromiseDeferred);
            });

            return loadGridFiltersPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_Routing_CustomerRouteManagementController', customerRouteManagementController);

})(appControllers);