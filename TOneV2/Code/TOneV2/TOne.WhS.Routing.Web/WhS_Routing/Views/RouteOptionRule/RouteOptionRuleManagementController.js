(function (appControllers) {

    "use strict";

    routeOptionRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService'];

    function routeOptionRuleManagementController($scope, WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeOptionRuleTypeSelectorAPI;
        var routeOptionRuleTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.routeOptionRuleTypeTemplates = [];

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            $scope.onRouteOptionRuleSettingsSelectorReady = function (api) {
                routeOptionRuleTypeSelectorAPI = api;
                routeOptionRuleTypeSelectorReadyPromiseDeferred.resolve();
            }
            
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addNewRouteOptionRule = addNewRouteOptionRule;

            $scope.hasAddRulePermission = function () {
                return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            }

            function getFilterObject() {

                var query = {
                    Code: $scope.code,
                    Name: $scope.name,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    RouteOptionRuleSettingsConfigId: routeOptionRuleTypeSelectorAPI.getSelectedIds()
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = new Date();

            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteOptionRuleTypeSelector]).catch(function (error) {
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
                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        function loadRouteOptionRuleTypeSelector() {
            var loadRouteOptionRuleTypePromiseDeferred = UtilsService.createPromiseDeferred();

            routeOptionRuleTypeSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routeOptionRuleTypeSelectorAPI, undefined, loadRouteOptionRuleTypePromiseDeferred);
            });

            return loadRouteOptionRuleTypePromiseDeferred.promise;
        }

        function addNewRouteOptionRule() {
            var onRouteOptionRuleAdded = function (addedItem) {
                gridAPI.onRouteOptionRuleAdded(addedItem);
            };

            WhS_Routing_RouteOptionRuleService.addRouteOptionRule(onRouteOptionRuleAdded);
        }
    }

    appControllers.controller('WhS_Routing_RouteOptionRuleManagementController', routeOptionRuleManagementController);
})(appControllers);