"use strict";

app.directive("vrWhsBePricingproductgrid", ["UtilsService", "VRNotificationService", "WhS_BE_PricingProductAPIService","WhS_BE_MainService",
function (UtilsService, VRNotificationService, WhS_BE_PricingProductAPIService,WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var pricingProductGrid = new PricingProductGrid($scope, ctrl);
            pricingProductGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Templates/PricingProductGridTemplate.html"

    };

    function PricingProductGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.pricingProducts = [];
            $scope.gridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onPricingProductAdded = function (pricingProductObject) {
                        gridAPI.itemAdded(pricingProductObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_PricingProductAPIService.GetFilteredPricingProducts(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                setDataItemExtension(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function setDataItemExtension(dataItem) {
            var extensionObject = {};
            var query = {
                PricingProductId: dataItem.PricingProductId
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.custormerPricingProductGridAPI = api;
                extensionObject.custormerPricingProductGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;
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

    return directiveDefinitionObject;

}]);
