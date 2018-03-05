(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteRuleAPIService', 'VRDateTimeService', 'WhS_Routing_RouteRuleCriteriaTypeEnum'];

    function routeRuleManagementController($scope, WhS_Routing_RouteRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteRuleAPIService, VRDateTimeService, WhS_Routing_RouteRuleCriteriaTypeEnum) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveAPI;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onCountrySelectorReady = function (api) {
                countryDirectiveAPI = api;
                countryReadyPromiseDeferred.resolve();
            }

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
                    Code: $scope.selectedRouteRuleCriteriaType.value == 'Code' ? $scope.code : undefined,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: $scope.selectedRouteRuleCriteriaType.value == 'SaleZone' ? saleZoneDirectiveAPI.getSelectedIds() : undefined,
                    CountryIds: $scope.selectedRouteRuleCriteriaType.value == 'Country' ? countryDirectiveAPI.getSelectedIds() : undefined,
                    EffectiveOn: $scope.effectiveOn,
                    RouteRuleSettingsConfigIds: routeRuleTypeSelectorAPI.getSelectedIds(),
                    IsManagementScreen: true,
                    includecheckicon: true
                };
                return query;
            }
        }
        function load() {
            $scope.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
            $scope.selectedRouteRuleCriteriaType = $scope.routeRuleCriteriaTypes[0];

            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteRuleTypeSelector, loadCountrySection])
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
                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }

        function loadCountrySection() {
            var countryReadyPromiseLoadDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveAPI, undefined, countryReadyPromiseLoadDeferred);
            });

            return countryReadyPromiseLoadDeferred.promise;
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