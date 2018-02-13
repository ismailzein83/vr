"use strict";

app.directive('vrWhsRoutingRouteruleGrid', ['VRNotificationService', 'WhS_Routing_RouteRuleAPIService', 'WhS_Routing_RouteRuleService', 'VRUIUtilsService',
    function (VRNotificationService, WhS_Routing_RouteRuleAPIService, WhS_Routing_RouteRuleService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var routeRuleGridCtor = new routeRuleGrid($scope, ctrl, $attrs);
                routeRuleGridCtor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteRule/Templates/RouteRuleGridTemplate.html"
        };

        function routeRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            var areRulesLinked = false;
            var customerRouteData = undefined;
            var onLinkedRouteRuleUpdated;

            function initializeController() {
                $scope.routeRules = [];
                $scope.showCustomerColumn = true;
                $scope.showIncludedCodesColumn = true;

                $scope.includeCheckIcon = false;

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = WhS_Routing_RouteRuleService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            var query = payload;
                            areRulesLinked = payload.areRulesLinked;
                            customerRouteData = payload.customerRouteData;
                            $scope.includeCheckIcon = payload.includecheckicon;
                            onLinkedRouteRuleUpdated = payload.onRouteRuleUpdated;
                            if (query.loadedFromRoutingProduct) {
                                $scope.showCustomerColumn = false;
                                $scope.showIncludedCodesColumn = false;
                                if (payload.query != undefined)
                                    query = payload.query;
                            }

                            if (query != undefined && query.hideCustomerColumn) {
                                $scope.showCustomerColumn = false;
                            }

                            if (query != undefined && query.hideIncludedCodesColumn) {
                                $scope.showIncludedCodesColumn = false;
                            }
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.onRouteRuleAdded = function (routeRuleObject) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(routeRuleObject);
                            gridAPI.itemAdded(routeRuleObject);
                        };

                        directiveAPI.getSelectedRouteRules = function () {
                            var ids = [];
                            for (var i = 0; i < $scope.routeRules.length; i++) {
                                if ($scope.routeRules[i].isSelected == true)
                                    ids.push($scope.routeRules[i].Entity.RuleId)
                            }
                            return ids;
                        };

                        directiveAPI.onRouteRuleUpdated = function (routeRuleObject) {
                            gridAPI.itemUpdated(routeRuleObject);
                        };

                        directiveAPI.onRouteRulesDeleted = function (ids) {
                            for (var i = 0; i < $scope.routeRules.length; i++) {
                                for (var j = 0; j < ids.length; j++) {
                                    if ($scope.routeRules[i].Entity.RuleId == ids[j]) {
                                        deleteRuleCallBack($scope.routeRules[i]);
                                    }
                                }
                            }
                        };

                        return directiveAPI;
                    }

                };


                function deleteRuleCallBack(rule) {
                    rule.isSelected = false;
                    gridAPI.itemDeleted(rule);
                }
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Routing_RouteRuleAPIService.GetFilteredRouteRules(dataRetrievalInput)
                       .then(function (response) {
                           if (response.Data != undefined) {
                               for (var i = 0; i < response.Data.length; i++) {
                                   gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                               }
                           }
                           onResponseReady(response);
                       })
                       .catch(function (error) {
                           VRNotificationService.notifyExceptionWithClose(error, $scope);
                       });
                };

                $scope.getRowStyle = function (dataItem) {
                    var rowStyle;
                    if (dataItem.CssClass != undefined)
                        rowStyle = { CssClass: dataItem.CssClass };

                    return rowStyle
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editRouteRule,
                    haspermission: hasUpdateRulePermission
                },
                {
                    name: "Delete",
                    clicked: deleteRouteRule,
                    haspermission: hasUpdateRulePermission
                }
                ];
            }

            function hasUpdateRulePermission() {
                return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
            }

            function editRouteRule(routeRule) {
                var onRouteRuleUpdated = function (updatedItem) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItem);
                    gridAPI.itemUpdated(updatedItem);

                    if (onLinkedRouteRuleUpdated != undefined && typeof (onLinkedRouteRuleUpdated) == 'function')
                        onLinkedRouteRuleUpdated(updatedItem);
                };
                if (areRulesLinked)
                    WhS_Routing_RouteRuleService.editLinkedRouteRule(routeRule.Entity.RuleId, customerRouteData, onRouteRuleUpdated);
                else
                    WhS_Routing_RouteRuleService.editRouteRule(routeRule.Entity.RuleId, onRouteRuleUpdated);
            }

            function deleteRouteRule(routeRule) {
                var onRouteRuleDeleted = function (deletedItem) {
                    gridAPI.itemDeleted(deletedItem);
                };

                WhS_Routing_RouteRuleService.setRouteRuleDeleted($scope, routeRule, onRouteRuleDeleted);
            }
        }

        return directiveDefinitionObject;

    }]);
