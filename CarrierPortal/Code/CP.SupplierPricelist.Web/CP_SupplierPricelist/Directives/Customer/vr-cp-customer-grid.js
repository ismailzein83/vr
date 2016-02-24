"use strict";
app.directive("vrCpCustomerGrid", ["UtilsService", "CP_SupplierPricelist_CustomerManagmentAPIService", "CP_SupplierPricelist_CustomerService", "VRNotificationService",
function (UtilsService, customeApiService, customerService, vRNotificationService) {
    var gridAPI;

    function CustomerGrid($scope, ctrl) {


        this.initializeController = initializeController;

        function initializeController() {

            $scope.customers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCustomerAdded = function (customerObject) {
                        gridAPI.itemAdded(customerObject);
                    }

                    directiveAPI.onCustomerUpdated = function (customerObject) {
                        gridAPI.itemUpdated(customerObject);
                    }
                    directiveAPI.onUserAdded = function (UserObject) {
                        gridAPI.itemAdded(UserObject);
                    }
                    return directiveAPI;
                }

            }
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return customeApiService.GetFilteredCustomers(dataRetrievalInput)
               .then(function (response) {
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
            },
            {
                name: "Assign User",
                clicked: assignUser,
            }
            ];
        }
    }
    function editCustomer(customer) {
        var onCustomerUpdated = function (updatedItem) {
            gridAPI.itemUpdated(updatedItem);
        };
        customerService.editcustomer(customer.Entity.CustomerId, onCustomerUpdated);
    }
    function assignUser(customer) {
        var onAssigningUser = function (updatedItem) {
            gridAPI.itemUpdated(updatedItem);
        };
        customerService.assignUser(customer.Entity.CustomerId, onAssigningUser);
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