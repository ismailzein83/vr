(function (appControllers) {

    "use strict";

    routeOptionRuleManagementController.$inject = ['$scope', 'WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService', 'VRDateTimeService'];

    function routeOptionRuleManagementController($scope, WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService, VRDateTimeService) {
        var gridAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierAccountDirectiveAPI;
        var supplierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routeOptionRuleTypeSelectorAPI;
        var routeOptionRuleTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.routeOptionRuleTypeTemplates = [];

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
            };

            $scope.onRouteOptionRuleSettingsSelectorReady = function (api) {
                routeOptionRuleTypeSelectorAPI = api;
                routeOptionRuleTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierAccountDirectiveReady = function (api) {
                supplierAccountDirectiveAPI = api;
                supplierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.setRouteOptionsRulesDeleted = function () {
                var ids = gridAPI.getSelectedRouteOptionsRules();
                var onSetRouteOptionsRulesDeleted = function () {
                    gridAPI.onRouteOptionsRulesDeleted();
                };
                WhS_Routing_RouteOptionRuleService.setRouteOptionsRulesDeleted(ids, onSetRouteOptionsRulesDeleted);
            };
            $scope.disabledDeletedRouteOptionsRules = function () {
                if (gridAPI == undefined) return true;
                return gridAPI.getSelectedRouteOptionsRules().length < 1;
            };

            $scope.addNewRouteOptionRule = addNewRouteOptionRule;

            $scope.hasAddRulePermission = function () {
                return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
            };

            $scope.hasDeleteRulePermission = function () {
                return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
            };

            $scope.onSelectSupplier = function (selectedItem) {
                $scope.selectedSupplierZones.length = 0;
                var payload = {
                    supplierId: selectedItem != undefined ? selectedItem.CarrierAccountId : undefined
                };

                var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader);
            };

            function getFilterObject() {

                var query = {
                    Code: $scope.code,
                    Name: $scope.name,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                    SupplierId: supplierAccountDirectiveAPI.getSelectedIds(),
                    SupplierZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                    EffectiveOn: $scope.effectiveOn,
                    RouteOptionRuleSettingsConfigIds: routeOptionRuleTypeSelectorAPI.getSelectedIds(),
                    includecheckicon: true
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection, loadSellingNumberPlanSection, loadRouteOptionRuleTypeSelector, loadSupplierSelector]).catch(function (error) {
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

        function loadSupplierSelector() {
            ;

            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierAccountReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(supplierAccountDirectiveAPI, directivePayload, supplierLoadPromiseDeferred);
                });
            return supplierLoadPromiseDeferred.promise;
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