"use strict";

app.directive("vrWhsBeCustomersellingproductGrid", [ "UtilsService", "VRNotificationService","WhS_BE_CustomerSellingProductAPIService",'WhS_BE_MainService',
function (UtilsService, VRNotificationService, WhS_BE_CustomerSellingProductAPIService, WhS_BE_MainService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "=",
                sellingproductid: '=',
                hidecustomercolumn: '@',
                hidesellingproductcolumn: '@',
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
                if ($attrs.hidecustomercolumn != undefined)
                    $scope.hideCustomerColumn = true;
                $scope.hideSellingProductColumn = false;
                if ($attrs.hidesellingproductcolumn != undefined)
                    $scope.hideSellingProductColumn = true;

                $scope.onGridReady = function (api) {  
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());
                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }

                        directiveAPI.onCustomerSellingProductAdded = function (customerSellingProductObject) {
                            gridAPI.itemAdded(customerSellingProductObject);
                        }
                        directiveAPI.onCustomerSellingProductDeleted = function (customerSellingProductObject) {
                             gridAPI.itemDeleted(customerSellingProductObject);
                        }

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

            //function deleteCustomerSellingProduct(gridObject) {
            //    var onCustomerSellingProductDeleted = function (gridObject) {
            //        gridAPI.itemUpdated(gridObject);
            //    }

            //    WhS_BE_MainService.deleteCustomerSellingProduct($scope,gridObject, onCustomerSellingProductDeleted);
            //}

            function defineMenuActions() {
                //$scope.gridMenuActions = [
                //   {
                //       name: "Delete",
                //       clicked: deleteCustomerSellingProduct
                //   }
                //];
            }
        }

        return directiveDefinitionObject;

    }]);
