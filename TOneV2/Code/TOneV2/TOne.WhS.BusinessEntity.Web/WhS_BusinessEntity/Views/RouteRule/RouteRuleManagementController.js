(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_BE_MainService'];

    function routeRuleManagementController($scope, WhS_BE_MainService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.AddNewRouteRule = AddNewRouteRule;
        }

        function load() {

        }

        function AddNewRouteRule() {
            var onRouteRuleAdded = function (addedItem) {
                gridAPI.onRouteRuleAdded(addedItem);
            };

            WhS_BE_MainService.addRouteRule(onRouteRuleAdded);
        }
    }

    appControllers.controller('WhS_BE_RouteRuleManagementController', routeRuleManagementController);
})(appControllers);