"use strict";
app.directive("vrCpCustomerGrid", ["UtilsService", "CP_SupplierPricelist_CustomerManagmentAPIService", "CP_SupplierPricelist_CustomerService","VRUIUtilsService", "VRNotificationService",
function (UtilsService, customeApiService, customerService, vRUIUtilsService, vRNotificationService) {
    var gridAPI;
    var gridDrillDownTabsObj;

    function CustomerGrid($scope, ctrl) {


        this.initializeController = initializeController;

        function initializeController() {

            $scope.customers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = customerService.getDrillDownDefinition();
                gridDrillDownTabsObj = vRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCustomerAdded = function (customerObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(customerObject);
                        gridAPI.itemAdded(customerObject);
                    }
                    return directiveAPI;
                    }

            }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return customeApiService.GetFilteredCustomers(dataRetrievalInput)
                   .then(function (response) {
                       if (response.Data != undefined) {
                           for (var i = 0; i < response.Data.length; i++) {
                               gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       vRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();


            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editCustomer
                }];
            }
        }
    function editCustomer(customer) {
        var onCustomerUpdated = function (updatedItem) {
            gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItem);
            gridAPI.itemUpdated(updatedItem);
        };
        customerService.editcustomer(customer.Entity.CustomerId, onCustomerUpdated);
    }
    
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerManagmentGrid = new CustomerGrid($scope, ctrl, $attrs);
            customerManagmentGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/Customer/Templates/CustomerGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);