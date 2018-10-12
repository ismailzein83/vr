(function (appControllers) {

    "use strict";

    customerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_EntityFilterEffectiveModeEnum', 'WhS_Routing_RoutingDatabaseTypeEnum'];

    function customerRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService, VRCommon_EntityFilterEffectiveModeEnum, WhS_Routing_RoutingDatabaseTypeEnum) {

        var parametersCustomersIds;
        var parametersZoneIds;
        var loadGrid;

        var gridAPI;
        var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routingDatabaseSelectorAPI;
        var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneSelectorAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeStatusSelectorAPI;
        var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routingDatabaseSelectPromiseDeferred = UtilsService.createPromiseDeferred();
        var routingDatabaseLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        var selectedRoutingDatabaseObject;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                parametersCustomersIds = parameters.CustomersIds;
                parametersZoneIds = parameters.ZoneIds;
                loadGrid = true;
            }
        }

        function defineScope() {
            $scope.legendHeader = "Legend";
            $scope.legendContent = getLegendContent();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridReadyPromiseDeferred.resolve();
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

            $scope.onRouteStatusDirectiveReady = function (api) {
                routeStatusSelectorAPI = api;
                routeStatusSelectorReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
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

                var query = {
                    isDatabaseTypeCurrent: routingDatabaseSelectorAPI.isDatabaseTypeCurrent(),
                    RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
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

            UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadSaleZoneSection, loadCustomersSection, loadRouteStatusSelector]).then(function () {
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
                routingDatabaseSelectPromiseDeferred = undefined;
            });
        }


        function loadRoutingDatabaseSelector() {
            var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

            routingDatabaseReadyPromiseDeferred.promise.then(function () {
                var routingDatabaseSelectorPayload = { onLoadRoutingDatabaseInfo: onLoadRoutingDatabaseInfo };
                VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, routingDatabaseSelectorPayload, loadRoutingDatabasePromiseDeferred);
            });

            return loadRoutingDatabasePromiseDeferred.promise;
        }

        function onLoadRoutingDatabaseInfo(selectedObject) {
            selectedRoutingDatabaseObject = selectedObject;
            routingDatabaseLoadPromiseDeferred.resolve();
        };

        function loadSaleZoneSection() {
            var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

            var payload = {};

            if (parametersZoneIds != null)
                payload.selectedIds = parametersZoneIds;

            UtilsService.waitMultiplePromises([routingDatabaseLoadPromiseDeferred.promise, saleZoneReadyPromiseDeferred.promise]).then(function () {
                var databaseType = getDatabaseEffectiveType(selectedRoutingDatabaseObject);
                if (databaseType != undefined)
                    payload.filter = { EffectiveMode: databaseType };

                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, payload, loadSaleZonePromiseDeferred);
            });

            return loadSaleZonePromiseDeferred.promise;
        }
        function loadCustomersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            var payload;
            if (parametersCustomersIds != null)
                payload = { selectedIds: parametersCustomersIds };

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }
        function loadRouteStatusSelector() {
            var loadRouteStatusPromiseDeferred = UtilsService.createPromiseDeferred();

            routeStatusSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeStatusSelectorAPI, undefined, loadRouteStatusPromiseDeferred);
            });

            return loadRouteStatusPromiseDeferred.promise;
        }

        function getDatabaseEffectiveType(selectedObject) {
            if (selectedObject == undefined) {
                return undefined;
            }

            if (selectedObject.Type == WhS_Routing_RoutingDatabaseTypeEnum.Current.value) {
                return VRCommon_EntityFilterEffectiveModeEnum.Current.value;
            }
            if (selectedObject.Type == WhS_Routing_RoutingDatabaseTypeEnum.Future.value) {
                return VRCommon_EntityFilterEffectiveModeEnum.Future.value;
            }
        }
    }

    appControllers.controller('WhS_Routing_CustomerRouteManagementController', customerRouteManagementController);

})(appControllers);