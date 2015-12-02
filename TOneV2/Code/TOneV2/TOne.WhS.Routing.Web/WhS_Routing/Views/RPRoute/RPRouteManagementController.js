(function (appControllers) {

    "use strict";

    rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        var routingDatabaseSelectorAPI;
        var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rpRoutePolicyAPI;
        var rpRoutePolicyReadyPromiseDeffered = UtilsService.createPromiseDeferred();

        var routingProductSelectorAPI;
        var routingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneSelectorAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.numberOfOptions = 3;

            $scope.onRoutingDatabaseSelectorReady = function (api) {
                routingDatabaseSelectorAPI = api;
                routingDatabaseReadyPromiseDeferred.resolve();
            }

            $scope.onRPRoutePolicySelectorReady = function (api) {
                rpRoutePolicyAPI = api;
                rpRoutePolicyReadyPromiseDeffered.resolve();
            };

            $scope.onRoutingProductSelectorReady = function (api) {
                routingProductSelectorAPI = api;
                routingProductReadyPromiseDeferred.resolve();
            }

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneSelectorAPI = api;
            }

            $scope.onSelectSellingNumberPlan = function (selectedItem) {
                $scope.showSaleZoneSelector = true;

                var payload = {
                    sellingNumberPlanId: selectedItem.SellingNumberPlanId
                }

                var setLoader = function (value) { $scope.isLoadingSaleZoneSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneSelectorAPI, payload, setLoader);
            }


            $scope.onGridReady = function (api) {
                gridAPI = api;
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            function getFilterObject() {

                var query = {
                    RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
                    PolicyConfigId: rpRoutePolicyAPI.getSelectedIds(),
                    NumberOfOptions: $scope.numberOfOptions,
                    RoutingProductIds: routingProductSelectorAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds()
                };
                return query;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadRPRoutePolicySelector, loadRoutingProductSelector, loadSellingNumberPlanSection]).catch(function (error) {
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

        function loadRPRoutePolicySelector()
        {
            var loadRPRoutePolicyPromiseDeferred = UtilsService.createPromiseDeferred();

            rpRoutePolicyReadyPromiseDeffered.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, undefined, loadRPRoutePolicyPromiseDeferred);
            });

            return loadRPRoutePolicyPromiseDeferred.promise;
        }

        function loadRoutingProductSelector() {
            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

            routingProductReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, undefined, loadRoutingProductPromiseDeferred);
            });

            return loadRoutingProductPromiseDeferred.promise;
        }

        function loadSellingNumberPlanSection() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
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