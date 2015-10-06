"use strict";

app.directive("vrWhsBeCustomerpricingproductgrid", [ "UtilsService", "VRNotificationService","WhS_BE_CustomerPricingProductAPIService",
function ( UtilsService, VRNotificationService,WhS_BE_CustomerPricingProductAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var customerPricingProductGrid = new CustomerPricingProductGrid($scope, ctrl);
                customerPricingProductGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Templates/CustomerPricingProductGridTemplate.html"

        };

        function CustomerPricingProductGrid($scope, ctrl) {
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.customerPricingProducts = [];
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
                    gridAPI.itemDeleted(gridObject);
                }

                PSTN_BE_Service.deleteSwitchTrunk(gridObject, onCustomerPricingProductDeleted);
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
