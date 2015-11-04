(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRUIUtilsService'];

    function routeRuleManagementController($scope, WhS_BE_MainService, UtilsService, VRUIUtilsService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred;

        defineScope();
        load();

        function defineScope() {

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            $scope.onSelectSellingNumberPlan = function (selectedItem) {
                $scope.showSaleZoneSelector = true;
                $scope.isLoadingSaleZonesSelector = true;

                saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

                var payload = {
                    filter: { SellingNumberPlanId: selectedItem.SellingNumberPlanId },
                }

                saleZoneReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
                });

                loadSaleZonePromiseDeferred.promise.finally(function () { $scope.isLoadingSaleZonesSelector = false; })
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewRouteRule = AddNewRouteRule;

            function getFilterObject() {
                var query = {
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds()
                };
                return query;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function loadCustomersSection()
        {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }

        function loadSellingNumberPlanSection() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }

        function AddNewRouteRule() {
            var onRouteRuleAdded = function (addedItem) {
                gridAPI.onRouteRuleAdded(addedItem);
            };

            WhS_BE_MainService.addRouteRule(onRouteRuleAdded);
        }
    }

    appControllers.controller('WhS_BE_RouteRuleManagementController', routeRuleManagementController);
})(appControllers);