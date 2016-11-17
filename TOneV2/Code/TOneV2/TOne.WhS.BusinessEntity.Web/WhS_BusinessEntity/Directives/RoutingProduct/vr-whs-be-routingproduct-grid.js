"use strict";

app.directive("vrWhsBeRoutingproductGrid", ["VRNotificationService", "WhS_BE_RoutingProductAPIService", "WhS_Routing_RouteRuleService", "WhS_BE_RoutingProductService", "VRUIUtilsService", "WhS_Routing_RouteRuleAPIService",
function (VRNotificationService, WhS_BE_RoutingProductAPIService, WhS_Routing_RouteRuleService, WhS_BE_RoutingProductService, VRUIUtilsService, WhS_Routing_RouteRuleAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var routingProductGridCtor = new routingProductGrid($scope, ctrl, $attrs);
            routingProductGridCtor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/RoutingProduct/Templates/RoutingProductGridTemplate.html"

    };

    function routingProductGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var drillDownManager;

        function initializeController() {
            $scope.routingProducts = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onRoutingProductAdded = function (routingProductObject) {
                        drillDownManager.setDrillDownExtensionObject(routingProductObject);
                        gridAPI.itemAdded(routingProductObject);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
                   .then(function (response) {
                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var directiveTab = {
                title: "Route Rules",
                directive: "vr-whs-routing-routerule-grid",
                loadDirective: function (directiveAPI, routingProductDataItem) {
                    routingProductDataItem.routeRuleGridAPI = directiveAPI;

                    var routeRuleGridPayload = {
                        loadedFromRoutingProduct: true,
                        query: { RoutingProductId: routingProductDataItem.Entity.RoutingProductId }
                    };

                    return routingProductDataItem.routeRuleGridAPI.loadGrid(routeRuleGridPayload);
                }
            };

            directiveTabs.push(directiveTab);

            return directiveTabs;
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [
                {
                    name: "View",
                    clicked: viewRoutingProduct,
                    haspermission: hasViewRoutingProductPermission
                },
                    {
                    name: "Edit",
                    clicked: editRoutingProduct,
                    haspermission: hasUpdateRoutingProductPermission
                },
                {
                    name: "Add Route Rule",
                    clicked: addRouteRule,
                    haspermission: hasAddRulePermission
                }
            ];
        }

        function hasUpdateRoutingProductPermission() {
            return WhS_BE_RoutingProductAPIService.HasUpdateRoutingProductPermission();
        }

        function hasViewRoutingProductPermission() {
            return WhS_BE_RoutingProductAPIService.HasUpdateRoutingProductPermission().then(function (response) {
                return !response;
            });
        }


        function hasAddRulePermission() {
            return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
        }

        function addRouteRule(dataItem) {
            gridAPI.expandRow(dataItem);

            var onRouteRuleAdded = function (addedItem) {
                dataItem.routeRuleGridAPI.onRouteRuleAdded(addedItem);
            };

            WhS_Routing_RouteRuleService.addRouteRule(onRouteRuleAdded, dataItem.Entity.RoutingProductId, dataItem.Entity.SellingNumberPlanId);
        }

        function editRoutingProduct(routingProduct) {
            var onRoutingProductUpdated = function (updatedItem) {
                drillDownManager.setDrillDownExtensionObject(updatedItem);
                gridAPI.itemUpdated(updatedItem);
            };

            WhS_BE_RoutingProductService.editRoutingProduct(routingProduct.Entity.RoutingProductId, onRoutingProductUpdated);
        }

        function viewRoutingProduct(routingProduct) {
            WhS_BE_RoutingProductService.viewRoutingProduct(routingProduct.Entity.RoutingProductId);
        }

        //function deleteRoutingProduct(routingProduct) {

        //    var onRoutingProductDeleted = function (deletedItem) {
        //        gridAPI.itemDeleted(deletedItem);
        //    }

        //    WhS_BE_RoutingProductService.deleteRoutingProduct($scope, routingProduct, onRoutingProductDeleted);
        //}

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
