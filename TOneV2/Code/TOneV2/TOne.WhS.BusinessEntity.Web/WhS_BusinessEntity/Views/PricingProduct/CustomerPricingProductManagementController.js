(function (appControllers) {

    "use strict";

    customerPricingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerPricingProductManagementController($scope, WhS_BE_CustomerPricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {

        defineScope();
        load();

        function defineScope() {

            $scope.pricingProductGridConnector = {};

            $scope.searchClicked = function () {
                return load();
            };

            $scope.AddNewCustomerPricingProduct = AddNewCustomerPricingProduct;
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

            var data = {
                name: $scope.name,
            };
            return data;
        }

        function AddNewPricingProduct() {
            var onPricingProductAdded = function (pricingProductObj) {
                if ($scope.pricingProductGridConnector.onPricingProductAdded != undefined)
                    $scope.pricingProductGridConnector.onPricingProductAdded(pricingProductObj);
            };

            WhS_BE_MainService.addPricingProduct(onPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerPricingProductManagementController', customerPricingProductManagementController);
})(appControllers);