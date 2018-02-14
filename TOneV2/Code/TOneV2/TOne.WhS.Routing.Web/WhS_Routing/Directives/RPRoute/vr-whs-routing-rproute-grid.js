"use strict";

app.directive('vrWhsRoutingRprouteGrid', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteRuleService', 'WhS_BE_ZoneRouteOptionsEnum',
    function (VRNotificationService, UtilsService, VRUIUtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RouteRuleService, WhS_BE_ZoneRouteOptionsEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new customerRouteGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteGridTemplate.html"
        };

        function customerRouteGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var routingDatabaseId;
            var policies;
            var defaultPolicyId;
            var selectedPolicyConfigId;
            var customerId;
            var showInSystemCurrency;
            var currencyId;
            var includeBlockedSuppliers;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.showGrid = false;
                $scope.rpRoutes = [];
                $scope.isCustomerSelected = false;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = initDrillDownDefinitions();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Routing_RPRouteAPIService.GetFilteredRPRoutes(dataRetrievalInput).then(function (response) {
                        var promises = [];
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var rpRouteDetail = response.Data[i];
                                gridDrillDownTabsObj.setDrillDownExtensionObject(rpRouteDetail);
                                promises.push(setRouteOptionDetailsDirectiveonEachItem(rpRouteDetail));
                            }
                        }
                        onResponseReady(response);

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            $scope.showGrid = true;
                            loadGridPromiseDeffered.resolve();
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            loadGridPromiseDeffered.reject();
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                        loadGridPromiseDeffered.reject();
                    });

                    return loadGridPromiseDeffered.promise;
                };

                $scope.getRowStyle = function (dataItem) {
                    var rowStyle;

                    if (dataItem.IsBlocked)
                        rowStyle = { CssClass: "bg-danger" };

                    return rowStyle;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {

                    routingDatabaseId = query.RoutingDatabaseId;
                    policies = query.FilteredPolicies;
                    defaultPolicyId = query.DefaultPolicyId;
                    selectedPolicyConfigId = query.PolicyConfigId;
                    customerId = query.CustomerId;
                    showInSystemCurrency = query.ShowInSystemCurrency;
                    currencyId = query.CurrencyId;
                    includeBlockedSuppliers = query.IncludeBlockedSuppliers;

                    if (query.CustomerId != undefined)
                        $scope.isCustomerSelected = true;

                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Matching Rule",
                    clicked: openRouteRuleEditor,
                }
                ];
            }

            function openRouteRuleEditor(dataItem) {
                WhS_Routing_RouteRuleService.viewRouteRule(dataItem.ExecutedRuleId);
            }

            function setRouteOptionDetailsDirectiveonEachItem(rpRouteDetail) {
                var promises = [];

                rpRouteDetail.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();
                rpRouteDetail.onRouteOptionsReady = function (api) {
                    rpRouteDetail.RouteOptionsAPI = api;

                    var payload = {
                        RoutingDatabaseId: routingDatabaseId,
                        SaleZoneId: rpRouteDetail.SaleZoneId,
                        RoutingProductId: rpRouteDetail.RoutingProductId,
                        RouteOptions: rpRouteDetail.RouteOptionsDetails,
                        display: WhS_BE_ZoneRouteOptionsEnum.SupplierRateWithNameAndPercentage.value,
                        currencyId: currencyId
                    };
                    VRUIUtilsService.callDirectiveLoad(rpRouteDetail.RouteOptionsAPI, payload, rpRouteDetail.RouteOptionsLoadDeferred);
                };
                promises.push(rpRouteDetail.RouteOptionsLoadDeferred.promise);

                rpRouteDetail.saleZoneServiceLoadDeferred = UtilsService.createPromiseDeferred();
                rpRouteDetail.onServiceViewerReady = function (api) {
                    rpRouteDetail.serviceViewerAPI = api;

                    var serviceViewerPayload;
                    if (rpRouteDetail != undefined) {
                        serviceViewerPayload = { selectedIds: rpRouteDetail.SaleZoneServiceIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(rpRouteDetail.serviceViewerAPI, serviceViewerPayload, rpRouteDetail.saleZoneServiceLoadDeferred);
                };
                promises.push(rpRouteDetail.saleZoneServiceLoadDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            }

            function initDrillDownDefinitions() {
                var drillDownDefinition = {};

                drillDownDefinition.title = "Details";
                drillDownDefinition.directive = "vr-whs-routing-rproute-details";

                drillDownDefinition.loadDirective = function (directiveAPI, rpRouteDetail) {
                    var payload = {
                        rpRouteDetail: rpRouteDetail,
                        routingDatabaseId: routingDatabaseId,
                        filteredPolicies: policies,
                        defaultPolicyId: selectedPolicyConfigId,
                        customerId: customerId,
                        showInSystemCurrency: showInSystemCurrency,
                        currencyId: currencyId,
                        includeBlockedSuppliers: includeBlockedSuppliers
                    };

                    return directiveAPI.load(payload);
                };

                return [drillDownDefinition];
            }
        }

        return directiveDefinitionObject;
    }]);
