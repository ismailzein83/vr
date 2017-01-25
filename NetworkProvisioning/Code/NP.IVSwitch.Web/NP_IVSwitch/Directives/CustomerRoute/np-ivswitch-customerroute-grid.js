"use strict";

app.directive("npIvswitchCustomerrouteGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_CustomerRouteAPIService", "NP_IVSwitch_CustomerRouteService",
function (utilsService, vrNotificationService, npIvSwitchCustomerRouteApiService, npIvSwitchCustomerRouteService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerRouteGrid = new CustomerRouteGrid($scope, ctrl, $attrs);
            customerRouteGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/CustomerRoute/Templates/CustomerRouteGridTemplate.html"
    };

    function CustomerRouteGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var customerId;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.customerRoutes = [];
            $scope.menuActions = [];
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return npIvSwitchCustomerRouteApiService.GetFilteredCustomerRoutes(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };
            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                customerId = query.CustomerId;
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function defineMenuActions() {
            $scope.menuActions.push({
                name: 'Edit',
                clicked: editCustomerRoute
            });
        }
        function editCustomerRoute(customerRouteEntity) {
            var onCustomerRouteUpdated = function (updatedCustomerroutes) {
                gridAPI.itemUpdated(updatedCustomerroutes);
            };
            console.log(customerRouteEntity.Entity);
            npIvSwitchCustomerRouteService.editCustomerRoute(customerRouteEntity.Entity.DestinationCode, customerId, onCustomerRouteUpdated);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;

}]);
