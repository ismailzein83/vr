(function (appControllers) {

    "use strict";

    sellingRuleManagementController.$inject = ["$scope", "WhS_Sales_SellingRuleService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_Sales_SellingRuleAPIService"];

    function sellingRuleManagementController($scope, WhS_Sales_SellingRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Sales_SellingRuleAPIService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.hasAddRulePermission = function () {
                return WhS_Sales_SellingRuleAPIService.HasAddRulePermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
            };

            $scope.onSelectSellingNumberPlan = function (selectedItem) {
                $scope.showSaleZoneSelector = true;

                var payload = {
                    sellingNumberPlanId: selectedItem.SellingNumberPlanId
                };

                var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            };


            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewSellingRule = AddNewSellingRule;

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
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function loadCustomersSection() {
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

        function AddNewSellingRule() {
            var onSellingRuleAdded = function (addedItem) {
                gridAPI.onSellingRuleAdded(addedItem);
            };

            WhS_Sales_SellingRuleService.addSellingRule(onSellingRuleAdded);
        }
    }

    appControllers.controller("WhS_Sales_SellingRuleManagementController", sellingRuleManagementController);
})(appControllers);