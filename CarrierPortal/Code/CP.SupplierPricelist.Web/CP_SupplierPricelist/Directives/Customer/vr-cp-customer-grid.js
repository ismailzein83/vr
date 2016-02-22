"use strict";
app.directive("vrCpCustomerGrid", ["UtilsService", "CP_SupplierPricelist_CustomerManagmentAPIService",
function (UtilsService, customeApiService) {

    function CustomerGrid($scope, ctrl) {

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.customers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCustomerAdded = function (customerObject) {
                        gridAPI.itemAdded(customerObject);
                    }

                    directiveAPI.onCustomerUpdated = function (customerObject) {
                        gridAPI.itemUpdated(customerObject);
                    }

                    return directiveAPI;
                }

            }
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return customeApiService.GetCustomers(dataRetrievalInput)
               .then(function (response) {
                   onResponseReady(response);
               })
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               });
        };

        defineMenuActions();


        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit"//,
                // clicked: editSellingRule,
            },
            {
                name: "Assign User"//,
                //clicked: deleteSellingRule,
            }
            ];
        }
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