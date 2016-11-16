"use strict";

app.directive('vrWhsRoutingRouteoptionruleGrid', ['VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService', 'WhS_Routing_RouteOptionRuleService',
function (VRNotificationService, WhS_Routing_RouteOptionRuleAPIService, WhS_Routing_RouteOptionRuleService) {

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

        function initializeController() {
            $scope.routeOptionRules = [];
            $scope.hideCustomerColumn = true;
            $scope.hideSupplierColumn = true;
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
                            $scope.hideSupplierColumn = false;
                            $scope.hideIncludedCodesColumn = false;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onRouteOptionRuleAdded = function (routeOptionRuleObject) {
                        gridAPI.itemAdded(routeOptionRuleObject);
                    };

                    directiveAPI.onRouteOptionRuleUpdated = function (routeOptionRuleObject) {
                        gridAPI.itemUpdated(routeOptionRuleObject);
                    };

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_RouteOptionRuleAPIService.GetFilteredRouteOptionRules(dataRetrievalInput)
                   .then(function (response) {
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
            }//,
            //{
            //    name: "Delete",
            //    clicked: deleteRouteOptionRule,
            //    haspermission: hasDeleteRulePermission
            //}
            ];
        }

        function hasUpdateRulePermission() {
            return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
        }

        //function hasDeleteRulePermission() {
        //    return WhS_Routing_RouteOptionRuleAPIService.HasDeleteRulePermission();
        //}


        function editRouteOptionRule(routeOptionRule) {
            var onRouteOptionRuleUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            WhS_Routing_RouteOptionRuleService.editRouteOptionRule(routeOptionRule.Entity.RuleId, onRouteOptionRuleUpdated);
        }

        //function deleteRouteOptionRule(routeOptionRule) {

        //    var onRouteOptionRuleDeleted = function (deletedItem) {
        //        gridAPI.itemDeleted(deletedItem);
        //    }

        //    WhS_Routing_RouteOptionRuleService.deleteRouteOptionRule($scope, routeOptionRule, onRouteOptionRuleDeleted);
        //}

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
