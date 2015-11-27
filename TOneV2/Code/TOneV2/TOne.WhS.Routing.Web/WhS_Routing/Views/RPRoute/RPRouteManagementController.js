(function (appControllers) {

    "use strict";

    rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        var routingProductSelectorAPI;
        var routingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneSelectorAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onRoutingProductSelectorReady = function (api) {
                routingProductSelectorAPI = api;
                routingProductReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneSelectorAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({ RoutingDatabaseId: '229' });
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            function getFilterObject() {

                var query = {
                    RoutingDatabaseId: '229',
                    RoutingProductIds: routingProductSelectorAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds()
                };
                return query;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([loadRoutingProductSelector, loadSaleZoneSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function loadRoutingDatabaseSelector() {
            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

            routingProductReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, undefined, loadRoutingProductPromiseDeferred);
            });

            return loadRoutingProductPromiseDeferred.promise;
        }

        function loadSaleZoneSelector() {
            var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, undefined, loadSaleZonePromiseDeferred);
            });

            return loadSaleZonePromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_Routing_RPRouteManagementController', rpRouteManagementController);
})(appControllers);