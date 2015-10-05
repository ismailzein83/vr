(function (appControllers) {

    "use strict";

    pricingProductGridController.$inject = ['$scope', 'WhS_BE_MainService', 'WhS_BE_PricingProductAPIService', 'VRNotificationService'];

    function pricingProductGridController($scope, WhS_BE_MainService, WhS_BE_PricingProductAPIService, VRNotificationService) {
        var gridApi = undefined;

        defineScope();

        function defineScope() {

            $scope.pricingProducts = [];
            $scope.gridMenuActions = [];

            defineMenuActions();

            $scope.pricingProductGridConnector.loadTemplateData = function () {
                return loadGrid();
            }

            $scope.pricingProductGridConnector.onPricingProductAdded = function (pricingProductObj) {
                gridApi.itemAdded(pricingProductObj);
            };

            $scope.gridReady = function (api) {
                gridApi = api;
                return loadGrid();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_PricingProductAPIService.GetFilteredPricingProducts(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }

        function loadGrid() {
            if ($scope.pricingProductGridConnector.data !== undefined && gridApi != undefined)
                return retrieveData();
        }

        function retrieveData() {
            var query = {

            };

            return gridApi.retrieveData(query);
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editPricingProduct,
            },
            {
                name: "Delete",
                clicked: deletePricingProduct,
            }
            ];
        }

        function editPricingProduct(pricingProductObj) {
            var onPricingProductUpdated = function (pricingProduct) {
                gridApi.itemUpdated(pricingProduct);
            }

            WhS_BE_MainService.editPricingProduct(pricingProductObj, onPricingProductUpdated);
        }

        function deletePricingProduct(pricingProductObj) {
            var onPricingProductDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            WhS_BE_MainService.deletePricingProduct(pricingProductObj, onPricingProductDeleted);
        }
    }

    appControllers.controller('WhS_BE_PricingProductGridController', pricingProductGridController);
})(appControllers);