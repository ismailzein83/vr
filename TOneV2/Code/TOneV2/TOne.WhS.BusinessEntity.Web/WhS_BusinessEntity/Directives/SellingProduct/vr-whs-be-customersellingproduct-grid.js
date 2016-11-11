"use strict";

app.directive("vrWhsBeCustomersellingproductGrid", ["UtilsService", "VRNotificationService", "WhS_BE_CustomerSellingProductAPIService", 'WhS_BE_CustomerSellingProductService',
function (UtilsService, VRNotificationService, WhS_BE_CustomerSellingProductAPIService, WhS_BE_CustomerSellingProductService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            sellingproductid: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerSellingProductGrid = new CustomerSellingProductGrid($scope, ctrl, $attrs);
            customerSellingProductGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingProduct/Templates/CustomerSellingProductGridTemplate.html"

    };

    function CustomerSellingProductGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;
        function initializeController() {
            $scope.customerSellingProducts = [];
            $scope.hideCustomerColumn = false;
            $scope.hideSellingProductColumn = false;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {

                        var query = payload;
                        if (payload.hideCustomerColumn) {
                            $scope.hideCustomerColumn = true;
                            query = payload.query;
                        }
                        else if (payload.hideSellingProductColumn) {
                            $scope.hideSellingProductColumn = true;
                            query = payload.query;
                        }
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onCustomerSellingProductAdded = function (customerSellingProductObject) {
                        gridAPI.itemAdded(customerSellingProductObject);
                    };
                    directiveAPI.onCustomerSellingProductDeleted = function (customerSellingProductObject) {
                        gridAPI.itemDeleted(customerSellingProductObject);
                    };

                    return directiveAPI;
                }


            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CustomerSellingProductAPIService.GetFilteredCustomerSellingProducts(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);

                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            var defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editCustomerSellingProduct,
                            haspermission: hasUpdateCustomerSellingProductPermission
                        }
            ];

            $scope.gridMenuActions = function (dataItem) {

                var date = new Date();
                if (dataItem.Entity.BED > UtilsService.dateToServerFormat(date)) {
                    return defaultMenuActions;
                }
                else {
                    return undefined;
                }
            };
        }

        function hasUpdateCustomerSellingProductPermission() {
            return WhS_BE_CustomerSellingProductAPIService.HasUpdateCustomerSellingProductPermission();
        }

        function editCustomerSellingProduct(customerSellingProductObj) {
            var onCustomerSellingProductUpdated = function (customerSellingProduct) {
                gridAPI.itemUpdated(customerSellingProduct);
            };

            WhS_BE_CustomerSellingProductService.editCustomerSellingProduct(customerSellingProductObj.Entity, onCustomerSellingProductUpdated);
        }
    }

    return directiveDefinitionObject;

}]);
