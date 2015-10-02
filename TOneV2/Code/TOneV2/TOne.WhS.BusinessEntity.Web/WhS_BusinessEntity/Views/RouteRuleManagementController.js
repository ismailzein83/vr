(function (appControllers) {

    "use strict";

    routeRuleManagementController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function routeRuleManagementController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {

        defineScope();
        load();

        function defineScope() {

            $scope.routeRuleGridConnector = {};

            $scope.searchClicked = function () {
                return load();
            };

            $scope.AddNewRouteRule = AddNewRouteRule;
        }

        function load() {
            loadGrid();
        }

        function loadGrid() {
            $scope.routeRuleGridConnector.data = getFilterObject();

            if ($scope.routeRuleGridConnector.loadTemplateData != undefined) {
                return $scope.routeRuleGridConnector.loadTemplateData();
            }
        }

        function getFilterObject() {
            return null;
        }

        function AddNewRouteRule() {
            var onRouteRuleAdded = function (routeRuleObj) {
                if ($scope.routeRuleGridConnector.onRouteRuleAdded != undefined)
                    $scope.routeRuleGridConnector.onRouteRuleAdded(routeRuleObj);
            };

            WhS_BE_MainService.addRouteRule(onRouteRuleAdded);
        }
    }

    appControllers.controller('WhS_BE_RouteRuleManagementController', routeRuleManagementController);
})(appControllers);