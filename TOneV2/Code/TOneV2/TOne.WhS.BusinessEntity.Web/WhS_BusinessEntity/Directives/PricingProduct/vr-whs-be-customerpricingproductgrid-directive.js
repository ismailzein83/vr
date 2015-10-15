"use strict";

app.directive("vrWhsBeCustomerpricingproductgrid", [ "UtilsService", "VRNotificationService","WhS_BE_CustomerPricingProductAPIService",'WhS_BE_MainService',
function (UtilsService, VRNotificationService, WhS_BE_CustomerPricingProductAPIService, WhS_BE_MainService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "=",
                pricingproductid: '=',
                hidecustomercolumn: '@',
                hidepricingproductcolumn: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var customerPricingProductGrid = new CustomerPricingProductGrid($scope, ctrl, $attrs);
                customerPricingProductGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingProduct/Templates/CustomerPricingProductGridTemplate.html"

        };

        function CustomerPricingProductGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.customerPricingProducts = [];
                $scope.hideCustomerColumn = false;
                if ($attrs.hidecustomercolumn != undefined)
                    $scope.hideCustomerColumn = true;
                $scope.hidePricingProductColumn = false;
                if ($attrs.hidepricingproductcolumn != undefined)
                    $scope.hidePricingProductColumn = true;

                $scope.onGridReady = function (api) {  
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }

                        directiveAPI.onCustomerPricingProductAdded = function (customerPricingProductObject) {
                            gridAPI.itemAdded(customerPricingProductObject);
                        }
                        directiveAPI.onCustomerPricingProductUpdated = function (customerPricingProductObject) {
                             gridAPI.itemUpdated(customerPricingProductObject);
                        }

                        return directiveAPI;
                    }
                    
                   
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_CustomerPricingProductAPIService.GetFilteredCustomerPricingProducts(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
                defineMenuActions();
            }

            function deleteCustomerPricingProduct(gridObject) {
                var onCustomerPricingProductDeleted = function (gridObject) {
                    gridAPI.itemUpdated(gridObject);
                }

                WhS_BE_MainService.deleteCustomerPricingProduct($scope,gridObject, onCustomerPricingProductDeleted);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [
                   {
                       name: "Delete",
                       clicked: deleteCustomerPricingProduct
                   }
                ];
            }
        }

        return directiveDefinitionObject;

    }]);
