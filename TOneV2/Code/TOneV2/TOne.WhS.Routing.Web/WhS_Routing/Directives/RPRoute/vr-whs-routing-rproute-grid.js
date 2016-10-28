﻿"use strict";

app.directive('vrWhsRoutingRprouteGrid', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteRuleService',
function (VRNotificationService, UtilsService, VRUIUtilsService, WhS_Routing_RPRouteAPIService, WhS_Routing_RouteRuleService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
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
        var gridAPI;
        var gridDrillDownTabsObj;
        var routingDatabaseId;
        var policies;
        var defaultPolicyId;
        var selectedPolicyConfigId;

        function initializeController() {
            $scope.showGrid = false;
            $scope.rpRoutes = [];

            var drillDownDefinitions = initDrillDownDefinitions();
            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        routingDatabaseId = query.RoutingDatabaseId;
                        policies = query.FilteredPolicies;
                        defaultPolicyId = query.DefaultPolicyId;
                        selectedPolicyConfigId = query.PolicyConfigId;
                        return gridAPI.retrieveData(query);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();

                WhS_Routing_RPRouteAPIService.GetFilteredRPRoutes(dataRetrievalInput)
                   .then(function (response) {
                       var promises = [];
                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               var rpRouteDetail = response.Data[i];
                               gridDrillDownTabsObj.setDrillDownExtensionObject(rpRouteDetail);
                               promises.push(setRouteOptionDetailsDirectiveonEachItem(rpRouteDetail));
                           }
                       }
                       $scope.showGrid = true;
                       onResponseReady(response);

                       UtilsService.waitMultiplePromises(promises).then(function () {
                           loadGridPromiseDeffered.resolve();
                       }).catch(function (error) {
                           loadGridPromiseDeffered.reject();
                       });
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyException(error, $scope);
                   });

                return loadGridPromiseDeffered.promise;
            };

            $scope.getRowStyle = function (dataItem) {

                var rowStyle;

                if (dataItem.IsBlocked)
                    rowStyle = { CssClass: "bg-danger" }

                return rowStyle
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Rule",
                clicked: openRouteRuleEditor,
            }
            ];
        }
        function openRouteRuleEditor(dataItem) {
            WhS_Routing_RouteRuleService.editRouteRule(dataItem.ExecutedRuleId);
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
                    RouteOptions: rpRouteDetail.RouteOptionsDetails
                };
                VRUIUtilsService.callDirectiveLoad(rpRouteDetail.RouteOptionsAPI, payload, rpRouteDetail.RouteOptionsLoadDeferred);
            };
            promises.push(rpRouteDetail.RouteOptionsLoadDeferred.promise);

            rpRouteDetail.saleZoneServiceLoadDeferred = UtilsService.createPromiseDeferred();
            rpRouteDetail.onServiceViewerReady = function (api) {
                rpRouteDetail.serviceViewerAPI = api;

                var serviceViewerPayload;
                if (rpRouteDetail != undefined) {
                    serviceViewerPayload = {
                        selectedIds: rpRouteDetail.SaleZoneServiceIds
                    };
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
                    defaultPolicyId: selectedPolicyConfigId
                };

                return directiveAPI.load(payload);
            };

            return [drillDownDefinition];
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);
