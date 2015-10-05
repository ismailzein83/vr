(function (appControllers) {

    "use strict";

    pricingProductManagementController.$inject = ['$scope', 'WhS_BE_PricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function pricingProductManagementController($scope, WhS_BE_PricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {

        defineScope();
        load();

        function defineScope() {

            $scope.pricingProductGridConnector = {};

            $scope.searchClicked = function () {
                return load();
            };

            $scope.AddNewPricingProduct = AddNewPricingProduct;
        }

        function load() {
            loadGrid();
        }

        function loadGrid() {
            $scope.pricingProductGridConnector.data = getFilterObject();

            if ($scope.pricingProductGridConnector.loadTemplateData != undefined) {
                return $scope.pricingProductGridConnector.loadTemplateData();
            }
        }

        function getFilterObject() {
            return null;
        }

        function AddNewPricingProduct() {
            var onPricingProductAdded = function (pricingProductObj) {
                if ($scope.pricingProductGridConnector.onPricingProductAdded != undefined)
                    $scope.pricingProductGridConnector.onPricingProductAdded(pricingProductObj);
            };

            WhS_BE_MainService.addPricingProduct(onPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_PricingProductManagementController', pricingProductManagementController);
})(appControllers);