"use strict";
app.directive("vrCpCustomeruserGrid", ["UtilsService", "CP_SupplierPricelist_CustomerUserAPIService", "CP_SupplierPricelist_CustomerUserService", "VRNotificationService",
function (UtilsService, customerUserApiService, customerUserService, vRNotificationService) {
  
    function CustomerUserGrid($scope, ctrl) {

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.customerUsers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCustomerUserAdded = function (customerUserObject) {
                        gridAPI.itemAdded(customerUserObject);
                    }
                    return directiveAPI;
                }

            }
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return customerUserApiService.GetFilteredCustomerUsers(dataRetrievalInput)
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
                name: "Unassign User",
                clicked: unassignUser
             }
            ];
        }

        function unassignUser(object) {
            var onCustomerUserDeleted = function () {
                gridAPI.itemDeleted(object);
            }
            customerUserService.unassignUser($scope, object.Entity.UserId, onCustomerUserDeleted);
        }
    }
   
   
   

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerUserGrid = new CustomerUserGrid($scope, ctrl, $attrs);
            customerUserGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/CustomerUser/Templates/CustomerUserGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);