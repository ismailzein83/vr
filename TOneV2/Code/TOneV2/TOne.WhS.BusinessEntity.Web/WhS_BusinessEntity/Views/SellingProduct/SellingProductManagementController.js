(function (appControllers) {

    "use strict";

    sellingProductManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function sellingProductManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridReady;
        var sellingNumberPlanDirectiveAPI;
        var routingProductDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (gridReady != undefined)
                 return gridReady.loadGrid(getFilterObject());
            };
            $scope.onGridReady = function (api) {
                gridReady = api;
                var filter = {}
                api.loadGrid(filter);
            }
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                load();
            }
            $scope.onRoutingProductDirectiveReady = function (api) {
                routingProductDirectiveAPI = api;
                load();
            }
            $scope.AddNewSellingProduct = AddNewSellingProduct;
        }

        function load() {
            $scope.isGettingData = true;
            if (sellingNumberPlanDirectiveAPI == undefined || routingProductDirectiveAPI == undefined)
                return;
            UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans, loadRoutingProducts]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function loadSellingNumberPlans() {
           return sellingNumberPlanDirectiveAPI.load();
        }
        function loadRoutingProducts() {
            return routingProductDirectiveAPI.load();
        }

        function getFilterObject() {

            var data = {
                name: $scope.name,
                SellingNumberPlansIds: UtilsService.getPropValuesFromArray(sellingNumberPlanDirectiveAPI.getData(), "SellingNumberPlanId"),
                RoutingProductsIds: UtilsService.getPropValuesFromArray(routingProductDirectiveAPI.getData(), "RoutingProductId"),
            };
            return data;
        }

        function AddNewSellingProduct() {
            var onSellingProductAdded = function (sellingProductObj) {
                if (gridReady != undefined)
                    gridReady.onSellingProductAdded(sellingProductObj);
            };
            WhS_BE_MainService.addSellingProduct(onSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_SellingProductManagementController', sellingProductManagementController);
})(appControllers);