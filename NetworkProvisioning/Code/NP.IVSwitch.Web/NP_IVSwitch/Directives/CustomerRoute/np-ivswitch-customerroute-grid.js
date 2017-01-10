"use strict";

app.directive("npIvswitchCustomerrouteGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_CustomerRouteAPIService",
function (utilsService, vrNotificationService, npIvSwitchCustomerRouteApiService) {

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
        function initializeController() {
            $scope.scopeModel = {};
            $scope.customerRoutes = [];

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
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;

}]);
