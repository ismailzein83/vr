(function (appControllers) {

    "use strict";

    customerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function customerRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        var routingDatabaseSelectorAPI;
        var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneSelectorAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeStatusSelectorAPI;
        var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.legendHeader = "Legend";
            $scope.legendContent = getLegendContent(); 

            $scope.onGridReady = function (api) {
                gridAPI = api;
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
            $scope.isLoadingFilterData = true;
            $scope.limit = 1000;
            $scope.includeBlockedSuppliers = true;

            return UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadSaleZoneSection, loadCustomersSection, loadRouteStatusSelector]).catch(function (error) {
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
        function loadSaleZoneSection() {
            var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, undefined, loadSaleZonePromiseDeferred);
            });

            return loadSaleZonePromiseDeferred.promise;
        }
        function loadCustomersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
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
    }

    appControllers.controller('WhS_Routing_CustomerRouteManagementController', customerRouteManagementController);

})(appControllers);