(function (appControllers) {

    "use strict";

    genericRuleManagementController.$inject = ['$scope', 'UtilsService'];

    function genericRuleManagementController($scope, UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.sellingNumberPlans = [];
            $scope.selectedSellingNumberPlans = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

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

        }

        function AddNewRoutingProduct() {

            var onRoutingProductAdded = function (addedItem) {
                gridAPI.onRoutingProductAdded(addedItem);
            };

            WhS_BE_RoutingProductService.addRoutingProduct(onRoutingProductAdded);
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);
})(appControllers);