"use strict";

app.directive('vrWhsRoutingRproutebycodeGrid', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteRuleService', 'WhS_BE_ZoneRouteOptionsEnum',
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteByCodeGridTemplate.html"
        };

        function customerRouteGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var routingDatabaseId;
            var currencyId;

            var gridAPI;

            function initializeController() {
                $scope.showGrid = false;
                $scope.rpRoutes = [];
                $scope.isCustomerSelected = false;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                    WhS_Routing_RPRouteAPIService.GetFilteredRPRoutesByCodes(dataRetrievalInput).then(function (response) {
                        var promises = [];
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var rpRouteDetail = response.Data[i];
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

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    routingDatabaseId = query.RoutingDatabaseId;
                    currencyId = query.CurrencyId;

                    if (query.CustomerId != undefined)
                        $scope.isCustomerSelected = true;

                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{ name: "Matching Rule", clicked: openRouteRuleEditor }];
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
                        currencyId: currencyId,
                        saleRate: rpRouteDetail.EffectiveRateValue
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
        }

        return directiveDefinitionObject;
    }]);
