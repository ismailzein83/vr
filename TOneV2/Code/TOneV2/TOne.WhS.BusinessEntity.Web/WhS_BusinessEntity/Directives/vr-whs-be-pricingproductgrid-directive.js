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
                PricingProductsIds: [dataItem.PricingProductId]
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
           },
           {
               name: "Assign Customer",
               clicked:assignCustomer
           }
            ];
        }

        function editPricingProduct(pricingProductObj) {
            var onPricingProductUpdated = function (pricingProduct) {
                gridAPI.itemUpdated(pricingProduct);
            }

            WhS_BE_MainService.editPricingProduct(pricingProductObj, onPricingProductUpdated);
        }
        function assignCustomer(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                PricingProductsIds: [dataItem.PricingProductId]
            }
            if (dataItem.extensionObject.custormerPricingProductGridAPI!=undefined)
             dataItem.extensionObject.custormerPricingProductGridAPI.loadGrid(query);
            var onCustomerPricingProductAdded = function (customerPricingProductObj) {
                if (dataItem.extensionObject.custormerPricingProductGridAPI != undefined)
                {
                    for (var i = 0; i < customerPricingProductObj.length; i++) {
                        if (customerPricingProductObj[i].Status == 0 && gridAPI != undefined)
                            dataItem.extensionObject.custormerPricingProductGridAPI.onCustomerPricingProductAdded(customerPricingProductObj[i]);
                        else if (customerPricingProductObj[i].Status == 1 && gridAPI != undefined) {
                            dataItem.extensionObject.custormerPricingProductGridAPI.onCustomerPricingProductUpdated(customerPricingProductObj[i]);
                        }
                    }
                }
            };
            WhS_BE_MainService.addCustomerPricingProduct(onCustomerPricingProductAdded,dataItem);
        }
        function deletePricingProduct(pricingProductObj) {
            var onPricingProductDeleted = function (gridObject) {
                gridAPI.itemDeleted(gridObject);
            };

            WhS_BE_MainService.deletePricingProduct($scope,pricingProductObj, onPricingProductDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
