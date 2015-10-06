(function (appControllers) {

    "use strict";

    customerPricingProductManagementController.$inject = ['$scope', 'WhS_BE_CustomerPricingProductAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function customerPricingProductManagementController($scope, WhS_BE_CustomerPricingProductAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        defineScope();
        load();

        function defineScope() {

            $scope.pricingProductGridConnector = {};

            $scope.searchClicked = function () {
                return load();
            };

            $scope.AddNewCustomerPricingProduct = AddNewCustomerPricingProduct;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {

                }
                api.loadGrid(filter);
                
            }
        }

        function load() {
        }

        function getFilterObject() {

            var data = {
                name: $scope.name,
            };
            return data;
        }

        function AddNewCustomerPricingProduct() {
            var onPricingProductAdded = function (pricingProductObj) {
                if (gridAPI != undefined)
                    gridAPI.onCustomerPricingProductAdded(pricingProductObj);
            };

            WhS_BE_MainService.addCustomerPricingProduct(onPricingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_CustomerPricingProductManagementController', customerPricingProductManagementController);
})(appControllers);