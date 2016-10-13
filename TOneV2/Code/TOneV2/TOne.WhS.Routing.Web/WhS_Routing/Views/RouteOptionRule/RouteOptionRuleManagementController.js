(function (appControllers) {

    "use strict";

    routeOptionRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService'];

    function routeOptionRuleManagementController($scope, WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeOptionRuleTypeSelectorAPI;
        var routeOptionRuleTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.routeOptionRuleTypeTemplates = [];
            $scope.selectedRouteOptionRuleTypeTemplate = undefined;

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

                var payload = {
                    sellingNumberPlanId: selectedItem.SellingNumberPlanId
                }

                var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            }

            $scope.onRouteOptionRuleTypeSelectorReady = function (api) {
                routeOptionRuleTypeSelectorAPI = api;
                routeOptionRuleTypeReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewRouteOptionRule = AddNewRouteOptionRule;

            $scope.hasAddRulePermission = function () {
                return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            }

            function getFilterObject() {

                var routeOptionRuleSettingsConfigId = $scope.selectedRouteOptionRuleTypeTemplate != undefined ? $scope.selectedRouteOptionRuleTypeTemplate.ExtensionConfigurationId : undefined;

                var query = {
                    Code: $scope.code,
                    Name: $scope.name,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    RouteOptionRuleSettingsConfigId: routeOptionRuleSettingsConfigId
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = new Date();
            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteOptionRuleTypeSection]).catch(function (error) {
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
        function loadRouteOptionRuleTypeSection() {

            return WhS_Routing_RouteOptionRuleAPIService.GetRouteOptionRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.routeOptionRuleTypeTemplates.push(item);
                });
            });
        }

        function AddNewRouteOptionRule() {
            var onRouteOptionRuleAdded = function (addedItem) {
                gridAPI.onRouteOptionRuleAdded(addedItem);
            };

            WhS_Routing_RouteOptionRuleService.addRouteOptionRule(onRouteOptionRuleAdded);
        }
    }

    appControllers.controller('WhS_Routing_RouteOptionRuleManagementController', routeOptionRuleManagementController);
})(appControllers);