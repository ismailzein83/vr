"use strict";

app.directive('vrWhsBeRoutingproductGrid', ['VRNotificationService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_MainService',
function (VRNotificationService, WhS_BE_RoutingProductAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
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
        
        function initializeController() {
            $scope.routingProducts = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onRoutingProductAdded = function (routingProductObject) {
                        gridAPI.itemAdded(routingProductObject);
                    }

                    directiveAPI.onRoutingProductUpdated = function (routingProductObject) {
                        gridAPI.itemUpdated(routingProductObject);
                    }

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRoutingProduct,
            },
            {
                name: "Delete",
                clicked: deleteRoutingProduct,
            }
            ];
        }

        function editRoutingProduct(routingProduct) {
            var onRoutingProductUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };

            WhS_BE_MainService.editRoutingProduct(routingProduct, onRoutingProductUpdated);
        }

        function deleteRoutingProduct(routingProduct) {

            var onRoutingProductDeleted = function (deletedItem) {
                gridAPI.itemDeleted(deletedItem);
            }

            WhS_BE_MainService.deleteRoutingProduct($scope, routingProduct, onRoutingProductDeleted);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
