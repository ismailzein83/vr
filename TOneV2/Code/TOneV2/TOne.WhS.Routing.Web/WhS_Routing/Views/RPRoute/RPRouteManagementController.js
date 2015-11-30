(function (appControllers) {

    "use strict";

    rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        var routingProductSelectorAPI;
        var routingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneSelectorAPI;
        //var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

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
                //saleZoneReadyPromiseDeferred.resolve();
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
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds()
                };
                return query;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([loadRoutingProductSelector, loadSellingNumberPlanSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
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