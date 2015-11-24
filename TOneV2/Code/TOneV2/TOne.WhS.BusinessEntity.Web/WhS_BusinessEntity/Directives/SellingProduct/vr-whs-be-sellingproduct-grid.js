"use strict";

app.directive("vrWhsBeSellingproductGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SellingProductAPIService","WhS_BE_SellingProductService" ,"WhS_BE_CustomerSellingProductService",
function (UtilsService, VRNotificationService, WhS_BE_SellingProductAPIService,  WhS_BE_SellingProductService , WhS_BE_CustomerSellingProductService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var sellingProductGrid = new SellingProductGrid($scope, ctrl);
            sellingProductGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingProduct/Templates/SellingProductGridTemplate.html"

    };

    function SellingProductGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.sellingProducts = [];
            $scope.gridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSellingProductAdded = function (sellingProductObject) {
                        gridAPI.itemAdded(sellingProductObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SellingProductAPIService.GetFilteredSellingProducts(dataRetrievalInput)
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
                SellingProductsIds: [dataItem.Entity.SellingProductId]
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.custormerSellingProductGridAPI = api;
                extensionObject.custormerSellingProductGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSellingProduct,
            },
           {
               name: "Delete",
               clicked: deleteSellingProduct,
           },
           {
               name: "Assign Customer",
               clicked:assignCustomer
           }
            ];
        }

        function editSellingProduct(sellingProductObj) {
            var onSellingProductUpdated = function (sellingProduct) {
                gridAPI.itemUpdated(sellingProduct);
            }

            WhS_BE_SellingProductService.editSellingProduct(sellingProductObj.Entity, onSellingProductUpdated);
        }
        function assignCustomer(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                SellingProductsIds: [dataItem.Entity.SellingProductId]
            }
            if (dataItem.extensionObject.custormerSellingProductGridAPI!=undefined)
             dataItem.extensionObject.custormerSellingProductGridAPI.loadGrid(query);
            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.extensionObject.custormerSellingProductGridAPI != undefined)
                {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        if (customerSellingProductObj[i].Status == 0 && gridAPI != undefined)
                            dataItem.extensionObject.custormerSellingProductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);
                        else if (customerSellingProductObj[i].Status == 1 && gridAPI != undefined) {
                            dataItem.extensionObject.custormerSellingProductGridAPI.onCustomerSellingProductUpdated(customerSellingProductObj[i]);
                        }
                    }
                }
            };
            WhS_BE_CustomerSellingProductService.addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem.Entity);
        }
        function deleteSellingProduct(sellingProductObj) {
            var onSellingProductDeleted = function (gridObject) {
                gridAPI.itemDeleted(gridObject);
            };

            WhS_BE_SellingProductService.deleteSellingProduct($scope, sellingProductObj.Entity, onSellingProductDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
