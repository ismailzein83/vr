'use strict';

app.directive('vrWhsRoutingCustomerrouteGridfilters', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'VRCommon_EntityFilterEffectiveModeEnum','Whs_BusinessEntity_ModuleNamesEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RoutingDatabaseTypeEnum, VRCommon_EntityFilterEffectiveModeEnum, Whs_BusinessEntity_ModuleNamesEnum) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new customerRouteGridfilters(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteGridFilterTemplate.html"
        };

        function customerRouteGridfilters(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var parametersCustomersIds;
            var parametersSuppliersIds;
            var parametersZoneIds;
            var parametersSaleCode;
            var routingDatabase;

            var selectedRoutingDatabaseObject;

            var routingDatabaseSelectorAPI;
            var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneSelectorAPI;
            var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var saleZoneSelectionChangedDeferred;

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierCarrierAccountDirectiveAPI;
            var supplierCarrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var routeStatusSelectorAPI;
            var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var routingDatabaseSelectPromiseDeferred = UtilsService.createPromiseDeferred();
            var routingDatabaseLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showRoutingDatabaseSelector;

                $scope.scopeModel.onRoutingDatabaseSelectorReady = function (api) {
                    routingDatabaseSelectorAPI = api;
                    routingDatabaseReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSupplierCarrierAccountDirectiveReady = function (api) {
                    supplierCarrierAccountDirectiveAPI = api;
                    supplierCarrierAccountReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRouteStatusDirectiveReady = function (api) {
                    routeStatusSelectorAPI = api;
                    routeStatusSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRoutingDatabaseSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        if (routingDatabaseSelectPromiseDeferred != undefined) {
                            routingDatabaseSelectPromiseDeferred.resolve();
                        } else {
                            var databaseType = getDatabaseEffectiveType(selectedItem);
                            var payload = { effectiveMode: databaseType };
                            saleZoneSelectorAPI.reLoadSaleZoneSelector(payload);
                        }
                    }
                };

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.limit = 1000;
                    $scope.scopeModel.includeBlockedSuppliers = true;

                    if (payload != undefined) {
                        parametersCustomersIds = payload.customersIds;
                        parametersSuppliersIds = payload.suppliersIds;
                        parametersZoneIds = payload.zoneIds;
                        parametersSaleCode = payload.saleCode;
                        routingDatabase = payload.routingDatabase;
                    }

                    $scope.scopeModel.showRoutingDatabaseSelector = routingDatabase == undefined;

                    $scope.scopeModel.code = parametersSaleCode;

                    var promises = [];

                    if ($scope.scopeModel.showRoutingDatabaseSelector)
                        promises.push(loadRoutingDatabaseSelector());

                    promises.push(loadSaleZoneSection());
                    promises.push(loadCustomersSection());
                    promises.push(loadSuppliersSection());
                    promises.push(loadRouteStatusSelector());

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        saleZoneSelectionChangedDeferred = undefined;
                        routingDatabaseSelectPromiseDeferred = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });

                    function loadRoutingDatabaseSelector() {
                        var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

                        routingDatabaseReadyPromiseDeferred.promise.then(function () {
                            var routingDatabaseSelectorPayload = { onLoadRoutingDatabaseInfo: onLoadRoutingDatabaseInfo };
                            if (routingDatabase != undefined)
                                routingDatabaseSelectorPayload.selectedIds = routingDatabase.routingDatabaseId;

                            VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, routingDatabaseSelectorPayload, loadRoutingDatabasePromiseDeferred);
                        });

                        return loadRoutingDatabasePromiseDeferred.promise;
                    }

                    function onLoadRoutingDatabaseInfo(selectedObject) {
                        selectedRoutingDatabaseObject = selectedObject;
                        routingDatabaseLoadPromiseDeferred.resolve();
                    }

                    function loadSaleZoneSection() {
                        var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
                        var saleZonePromises = [];

                        saleZonePromises.push(saleZoneReadyPromiseDeferred.promise);
                        if ($scope.scopeModel.showRoutingDatabaseSelector)
                            saleZonePromises.push(routingDatabaseLoadPromiseDeferred.promise);

                        UtilsService.waitMultiplePromises(saleZonePromises).then(function () {
                            var payload = {
                                onSellingNumberPlanSelectionChanged: onSellingNumberPlanSelectionchanged,
                                areSaleZonesRequired: false
                            };

                            if (parametersZoneIds != null) {
                                saleZoneSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                                payload.selectedIds = parametersZoneIds;
                            }

                            var databaseType;

                            if (routingDatabase != undefined)
                                databaseType = routingDatabase.routingDatabaseType;
                            else
                                databaseType = getDatabaseEffectiveType(selectedRoutingDatabaseObject);

                            if (databaseType != undefined)
                                payload.filter = { EffectiveMode: databaseType };

                            VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, payload, loadSaleZonePromiseDeferred);
                        });

                        return loadSaleZonePromiseDeferred.promise;
                    }
                    function loadCustomersSection() {
                        var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                        var payload = { filter: { Filters: getCarrierAccountSelectorFilter() } };

                        if (parametersCustomersIds != null)
                            payload.selectedIds = parametersCustomersIds;

                        carrierAccountReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
                        });

                        return loadCarrierAccountPromiseDeferred.promise;
                    }
                    function loadSuppliersSection() {
                        var loadSupplierCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                        var payload = { filter: { Filters: getCarrierAccountSelectorFilter() } };

                        if (parametersSuppliersIds != null)
                            payload.selectedIds = parametersSuppliersIds;

                        supplierCarrierAccountReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(supplierCarrierAccountDirectiveAPI, payload, loadSupplierCarrierAccountPromiseDeferred);
                        });

                        return loadSupplierCarrierAccountPromiseDeferred.promise;
                    }
                    function loadRouteStatusSelector() {
                        var loadRouteStatusPromiseDeferred = UtilsService.createPromiseDeferred();

                        routeStatusSelectorReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(routeStatusSelectorAPI, undefined, loadRouteStatusPromiseDeferred);
                        });

                        return loadRouteStatusPromiseDeferred.promise;
                    }

                    function getDatabaseEffectiveType(selectedObject) {
                        if (selectedObject == undefined) {
                            return undefined;
                        }

                        if (selectedObject.Type == WhS_Routing_RoutingDatabaseTypeEnum.Current.value) {
                            return VRCommon_EntityFilterEffectiveModeEnum.Current.value;
                        }
                        if (selectedObject.Type == WhS_Routing_RoutingDatabaseTypeEnum.Future.value) {
                            return VRCommon_EntityFilterEffectiveModeEnum.Future.value;
                        }
                    }

                    function onSellingNumberPlanSelectionchanged(selectedSellingNumberPlan) {
                        if (saleZoneSelectionChangedDeferred != undefined) {
                            saleZoneSelectionChangedDeferred.resolve();
                        }
                        else {
                            var selectedSellingNumberPlanId = selectedSellingNumberPlan != undefined ? selectedSellingNumberPlan.SellingNumberPlanId : undefined;

                            var customerSelectorPayload = {};
                            if (selectedSellingNumberPlanId != undefined) {
                                customerSelectorPayload.filter = { SellingNumberPlanId: selectedSellingNumberPlanId };
                            }
                            if ($scope.scopeModel.selectedCustomer != undefined && (selectedSellingNumberPlanId == undefined || selectedSellingNumberPlanId == $scope.scopeModel.selectedCustomer.SellingNumberPlanId)) {
                                customerSelectorPayload.selectedIds = $scope.scopeModel.selectedCustomer.CarrierAccountId;
                            }
                            VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, customerSelectorPayload, undefined);
                        }
                    }

                    function getCarrierAccountSelectorFilter() {
                        var carrierAccountSelectorFilters = [];

                        var carrierAccountFilterAffected = {
                            $type: 'TOne.WhS.BusinessEntity.Business.AssignedCarrierAccountsForAccountManager, TOne.WhS.BusinessEntity.Business',
                            ModuleName: Whs_BusinessEntity_ModuleNamesEnum.CustomerRoute.value
                        };
                        carrierAccountSelectorFilters.push(carrierAccountFilterAffected);

                        return carrierAccountSelectorFilters;
                    }
                };

                api.getData = function () {
                    return getFilterObject();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getFilterObject() {

                var query = {
                    isDatabaseTypeCurrent: routingDatabase != undefined ? routingDatabase.routingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Current.value : routingDatabaseSelectorAPI.isDatabaseTypeCurrent(),
                    RoutingDatabaseId: routingDatabase != undefined ? routingDatabase.routingDatabaseId : routingDatabaseSelectorAPI.getSelectedIds(),
                    SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                    SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
                    Code: $scope.scopeModel.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    SupplierIds: supplierCarrierAccountDirectiveAPI.getSelectedIds(),
                    RouteStatus: routeStatusSelectorAPI.getSelectedIds(),
                    LimitResult: $scope.scopeModel.limit,
                    IncludeBlockedSuppliers: $scope.scopeModel.includeBlockedSuppliers,
                    ForceRoutingDatabaseId: routingDatabase != undefined
                };
                return query;
            }

        }
        return directiveDefinitionObject;
    }]);