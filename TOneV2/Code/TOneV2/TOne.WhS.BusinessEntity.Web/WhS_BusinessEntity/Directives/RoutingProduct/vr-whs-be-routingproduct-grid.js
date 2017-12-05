"use strict";

app.directive("vrWhsBeRoutingproductGrid", ["VRNotificationService", "WhS_BE_RoutingProductAPIService", "WhS_Routing_RouteRuleService", "WhS_BE_RoutingProductService", "UtilsService", "VRUIUtilsService", "WhS_Routing_RouteRuleAPIService", "VRCommon_ObjectTrackingService",
function (VRNotificationService, WhS_BE_RoutingProductAPIService, WhS_Routing_RouteRuleService, WhS_BE_RoutingProductService, UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleAPIService, VRCommon_ObjectTrackingService) {

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
                var serviceViewerLoadPromises = [];
               WhS_BE_RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
                   .then(function (response) {
                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                               setService(response.Data[i]);
                               serviceViewerLoadPromises.push(response.Data[i].serviceViewerLoadDeferred.promise);
                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
                return UtilsService.waitMultiplePromises(serviceViewerLoadPromises);
            };

            defineMenuActions();
        }

        function setService(item) {
            item.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
            item.onServiceViewerReady = function (api) {
                item.serviceViewerAPI = api;
                var serviceViewerPayload = { selectedIds: item.Entity.Settings.DefaultServiceIds };
                VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, serviceViewerPayload, item.serviceViewerLoadDeferred);
            };
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var directiveTab = {
                title: "Route Rules",
                directive: "vr-whs-routing-routerule-view",
                loadDirective: function (directiveAPI, routingProductDataItem) {
                    routingProductDataItem.routeRuleGridAPI = directiveAPI;

                    var routeRuleGridPayload = {                        
                        query: {
                            loadedFromRoutingProduct: true,
                            RoutingProductId: routingProductDataItem.Entity.RoutingProductId,
                            sellingNumberPlanId: routingProductDataItem.Entity.SellingNumberPlanId
                        }
                    };

                    return routingProductDataItem.routeRuleGridAPI.load(routeRuleGridPayload);
                }
            };
            directiveTabs.push(directiveTab);
            var drillDownDefinition = {

                title: VRCommon_ObjectTrackingService.getObjectTrackingGridTitle(),
                directive: "vr-common-objecttracking-grid",


                loadDirective: function (directiveAPI, routingProductItem) {
                    routingProductItem.objectTrackingGridAPI = directiveAPI;

                    var query = {
                        ObjectId: routingProductItem.Entity.RoutingProductId,
                        EntityUniqueName: WhS_BE_RoutingProductService.getEntityUniqueName(),

                    };
                    return routingProductItem.objectTrackingGridAPI.load(query);
                }
            };


            directiveTabs.push(drillDownDefinition);

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

        function editRoutingProduct(routingProduct) {
            var onRoutingProductUpdated = function (updatedItem) {
                drillDownManager.setDrillDownExtensionObject(updatedItem);
                gridAPI.itemUpdated(updatedItem);
                setService(updatedItem);
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
