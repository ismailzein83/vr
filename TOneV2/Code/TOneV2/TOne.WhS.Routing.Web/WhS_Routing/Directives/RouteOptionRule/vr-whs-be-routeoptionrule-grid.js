"use strict";

app.directive('vrWhsRoutingRouteoptionruleGrid', ['VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService', 'WhS_Routing_RouteOptionRuleService', 'VRUIUtilsService',
function (VRNotificationService, WhS_Routing_RouteOptionRuleAPIService, WhS_Routing_RouteOptionRuleService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var routeOptionRuleGridCtor = new routeOptionRuleGrid($scope, ctrl, $attrs);
            routeOptionRuleGridCtor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteOptionRule/Templates/RouteOptionRuleGridTemplate.html"

    };

    function routeOptionRuleGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var gridDrillDownTabsObj;
        var areRulesLinked = false;
        var linkedCode = undefined;

        function initializeController() {
            $scope.routeOptionRules = [];
            $scope.hideCustomerColumn = true;
            $scope.hideSupplierColumn = true;
            $scope.hideIncludedCodesColumn = true;
            $scope.includeCheckIcon = false;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = WhS_Routing_RouteOptionRuleService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        var query = payload;
                        areRulesLinked = payload.areRulesLinked;
                        $scope.includeCheckIcon = payload.includecheckicon;
                        linkedCode = payload.linkedCode;
                        if (query.loadedFromRoutingProduct) {
                            $scope.hideCustomerColumn = false;
                            $scope.hideSupplierColumn = false;
                            $scope.hideIncludedCodesColumn = false;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onRouteOptionRuleAdded = function (routeOptionRuleObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(routeOptionRuleObject);
                        gridAPI.itemAdded(routeOptionRuleObject);
                    };

                    directiveAPI.onRouteOptionRuleUpdated = function (routeOptionRuleObject) {
                        gridAPI.itemUpdated(routeOptionRuleObject);
                    };

                    directiveAPI.getSelectedRouteOptionsRules = function () {
                        var ids = [];
                        for (var i = 0; i < $scope.routeOptionRules.length; i++) {
                            if ($scope.routeOptionRules[i].isSelected == true)
                                ids.push($scope.routeOptionRules[i].Entity.RuleId)
                        }
                        return ids;
                    };


                    directiveAPI.onRouteOptionsRulesDeleted = function (ids) {
                        for (var i = 0; i < $scope.routeOptionRules.length; i++) {
                            for (var j = 0; j < ids.length; j++) {
                                if ($scope.routeOptionRules[i].Entity.RuleId == ids[j]) {
                                    deleteRuleCallBack($scope.routeOptionRules[i])
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
                return WhS_Routing_RouteOptionRuleAPIService.GetFilteredRouteOptionRules(dataRetrievalInput)
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
                clicked: editRouteOptionRule,
                haspermission: hasUpdateRulePermission
            },
            {
                name: "Delete",
                clicked: deleteRouteOptionRule,
                haspermission: hasUpdateRulePermission
            }
            ];
        }

        function hasUpdateRulePermission() {
            return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
        }

        function editRouteOptionRule(routeOptionRule) {
            var onRouteOptionRuleUpdated = function (updatedItem) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItem);
                gridAPI.itemUpdated(updatedItem);
            };

            if (areRulesLinked)
                WhS_Routing_RouteOptionRuleService.editLinkedRouteOptionRule(routeOptionRule.Entity.RuleId, linkedCode, onRouteOptionRuleUpdated);
            else
                WhS_Routing_RouteOptionRuleService.editRouteOptionRule(routeOptionRule.Entity.RuleId, onRouteOptionRuleUpdated);
        }

        function deleteRouteOptionRule(routeOptionRule) {

            var onRouteOptionRuleDeleted = function (deletedItem) {
                gridAPI.itemDeleted(deletedItem);
            };

            WhS_Routing_RouteOptionRuleService.setRouteOptionRuleDeleted($scope, routeOptionRule, onRouteOptionRuleDeleted);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
