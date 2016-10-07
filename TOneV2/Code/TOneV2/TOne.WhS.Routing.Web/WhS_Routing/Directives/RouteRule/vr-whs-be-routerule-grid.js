"use strict";

app.directive('vrWhsRoutingRouteruleGrid', ['VRNotificationService', 'WhS_Routing_RouteRuleAPIService', 'WhS_Routing_RouteRuleService',
function (VRNotificationService, WhS_Routing_RouteRuleAPIService, WhS_Routing_RouteRuleService) {

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
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteRule/Templates/RouteRuleGridTemplate.html"

    };

    function routeRuleGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.routeRules = [];
            $scope.hideCustomerColumn = true;
            $scope.hideIncludedCodesColumn = true;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        var query = payload;
                        if (query.loadedFromRoutingProduct) {
                            $scope.hideCustomerColumn = false;
                            $scope.hideIncludedCodesColumn = false;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onRouteRuleAdded = function (routeRuleObject) {
                        gridAPI.itemAdded(routeRuleObject);
                    }

                    directiveAPI.onRouteRuleUpdated = function (routeRuleObject) {
                        gridAPI.itemUpdated(routeRuleObject);
                    }

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_RouteRuleAPIService.GetFilteredRouteRules(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            $scope.getRowStyle = function (dataItem) {
                var rowStyle;

                if (dataItem.Entity != undefined && dataItem.Entity.Settings != undefined && dataItem.Entity.Settings.ConfigId == 'bbb0ca31-0fcd-4035-a8ed-5d4bad06c662')
                    rowStyle = { CssClass: "bg-danger" }

                return rowStyle
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRouteRule,
                haspermission: hasUpdateRulePermission
            }//,
            //{
            //    name: "Delete",
            //    clicked: deleteRouteRule,
            //    haspermission: hasDeleteRulePermission
            //}
            ];
        }

        function hasUpdateRulePermission() {
            return WhS_Routing_RouteRuleAPIService.HasUpdateRulePermission();
        }

        function hasDeleteRulePermission() {
            return WhS_Routing_RouteRuleAPIService.HasDeleteRulePermission();
        }


        function editRouteRule(routeRule) {
            var onRouteRuleUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            WhS_Routing_RouteRuleService.editRouteRule(routeRule.Entity.RuleId, onRouteRuleUpdated);
        }

        function deleteRouteRule(routeRule) {

            var onRouteRuleDeleted = function (deletedItem) {
                gridAPI.itemDeleted(deletedItem);
            }

            WhS_Routing_RouteRuleService.deleteRouteRule($scope, routeRule, onRouteRuleDeleted);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
