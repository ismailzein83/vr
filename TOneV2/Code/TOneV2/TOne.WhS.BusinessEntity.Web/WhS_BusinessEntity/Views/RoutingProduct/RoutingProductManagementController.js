(function (appControllers) {

    "use strict";

    routingProductManagementController.$inject = ["$scope", "WhS_BE_SellingNumberPlanAPIService", "UtilsService", "VRNotificationService", "WhS_BE_RoutingProductService", "WhS_BE_RoutingProductAPIService"];

    function routingProductManagementController($scope, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRNotificationService, WhS_BE_RoutingProductService, WhS_BE_RoutingProductAPIService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.hasAddRoutingProductPermission = function () {
                return WhS_BE_RoutingProductAPIService.HasAddRoutingProductPermission();
            };

            $scope.sellingNumberPlans = [];
            $scope.selectedSellingNumberPlans = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            };

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewRoutingProduct = AddNewRoutingProduct;

            function getFilterObject() {
                var data = {
                    Name: $scope.name,
                    SellingNumberPlanIds: UtilsService.getPropValuesFromArray($scope.selectedSellingNumberPlans, "SellingNumberPlanId")
                };
                return data;
            }
        }

        function load() {
            $scope.isLoadingFilterData = true;

            WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.sellingNumberPlans.push(item);
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function AddNewRoutingProduct() {

            var onRoutingProductAdded = function (addedItem) {
                gridAPI.onRoutingProductAdded(addedItem);
            };

            WhS_BE_RoutingProductService.addRoutingProduct(onRoutingProductAdded);
        }
    }

    appControllers.controller("WhS_BE_RoutingProductManagementController", routingProductManagementController);
})(appControllers);