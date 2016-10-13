(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteRuleAPIService'];

    function routeRuleManagementController($scope, WhS_Routing_RouteRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteRuleAPIService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeRuleTypeSelectorAPI;
        var routeRuleTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.routeRuleTypeTemplates = [];
            $scope.selectedRouteRuleTypeTemplate = undefined;

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

            $scope.onRouteRuleTypeSelectorReady = function (api) {
                routeRuleTypeSelectorAPI = api;
                routeRuleTypeReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewRouteRule = AddNewRouteRule;

            $scope.hasAddRulePermission = function () {
                return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
            }

            function getFilterObject() {

                var routeRuleSettingsConfigId = $scope.selectedRouteRuleTypeTemplate != undefined ? $scope.selectedRouteRuleTypeTemplate.ExtensionConfigurationId : undefined;

                var query = {
                    Name: $scope.name,
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    RouteRuleSettingsConfigId: routeRuleSettingsConfigId
                };
                return query;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = new Date();
            
            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteRuleTypeSection])
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
        function loadRouteRuleTypeSection() {

            return WhS_Routing_RouteRuleAPIService.GetRouteRuleSettingsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.routeRuleTypeTemplates.push(item);
                });
            });
        }

        function AddNewRouteRule() {
            var onRouteRuleAdded = function (addedItem) {
                gridAPI.onRouteRuleAdded(addedItem);
            };

            WhS_Routing_RouteRuleService.addRouteRule(onRouteRuleAdded);
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleManagementController', routeRuleManagementController);
})(appControllers);