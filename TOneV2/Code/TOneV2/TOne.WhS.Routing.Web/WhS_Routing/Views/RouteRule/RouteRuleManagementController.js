(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteRuleAPIService', 'VRDateTimeService'];

    function routeRuleManagementController($scope, WhS_Routing_RouteRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteRuleAPIService, VRDateTimeService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeRuleTypeSelectorAPI;
        var routeRuleTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.routeRuleTypeTemplates = [];

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onRouteRuleTypeSelectorReady = function (api) {
                routeRuleTypeSelectorAPI = api;
                routeRuleTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addNewRouteRule = addNewRouteRule;

            $scope.setRouteRulesDeleted = function () {
                var ids = gridAPI.getSelectedRouteRules();
                var onSetRouteRulesDeleted = function () {
                    gridAPI.onRouteRulesDeleted(ids);
                };
                WhS_Routing_RouteRuleService.setRouteRulesDeleted(ids, onSetRouteRulesDeleted);
            };
            $scope.disabledDeletedRouteRules = function () {
                if (gridAPI == undefined) return true;
                return gridAPI.getSelectedRouteRules().length < 1;
            };
            $scope.hasAddRulePermission = function () {
                return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
            };
            $scope.hasDeleteRulePermission = function () {
                return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
            };
            

            function getFilterObject() {

                var query = {
                    Name: $scope.name,
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    RouteRuleSettingsConfigIds: routeRuleTypeSelectorAPI.getSelectedIds(),
                    IsManagementScreen: true,
                    includecheckicon: true
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteRuleTypeSelector])
                    .catch(function (error) {
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

            saleZoneReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI,
                     //{
                     //   //sellingNumberPlanId: 16,
                     //  // selectedIds: [10748, 10754]
                     //}  
                     undefined
                    , loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        function loadRouteRuleTypeSelector() {
            var loadRouteRuleTypePromiseDeferred = UtilsService.createPromiseDeferred();

            routeRuleTypeSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeRuleTypeSelectorAPI, undefined, loadRouteRuleTypePromiseDeferred);
            });

            return loadRouteRuleTypePromiseDeferred.promise;
        }

        function addNewRouteRule() {
            var onRouteRuleAdded = function (addedItem) {
                gridAPI.onRouteRuleAdded(addedItem);
            };
            var context = {};
            WhS_Routing_RouteRuleService.addRouteRule(onRouteRuleAdded, context);
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleManagementController', routeRuleManagementController);
})(appControllers);