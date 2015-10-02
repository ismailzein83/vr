(function (appControllers) {

    "use strict";

    routeRuleGridController.$inject = ['$scope', 'WhS_BE_MainService', 'WhS_BE_RouteRuleAPIService', 'VRNotificationService'];

    function routeRuleGridController($scope, WhS_BE_MainService, WhS_BE_RouteRuleAPIService, VRNotificationService) {
        var gridApi = undefined;

        defineScope();

        function defineScope() {

            $scope.routeRules = [];
            $scope.gridMenuActions = [];

            defineMenuActions();

            $scope.routeRuleGridConnector.loadTemplateData = function () {
                return loadGrid();
            }

            $scope.routeRuleGridConnector.onRouteRuleAdded = function (routeRuleObj) {
                gridApi.itemAdded(routeRuleObj);
            };

            $scope.gridReady = function (api) {
                gridApi = api;
                return loadGrid();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_RouteRuleAPIService.GetFilteredRouteRules(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }

        function loadGrid() {
            if ($scope.routeRuleGridConnector.data !== undefined && gridApi != undefined)
                return retrieveData();
        }

        function retrieveData() {
            var query = {

            };

            return gridApi.retrieveData(query);
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRouteRule,
            },
            {
                name: "Delete",
                clicked: deleteRouteRule,
            }
            ];
        }

        function editRouteRule(routeRuleObj) {
            var onRouteRuleUpdated = function (routeRule) {
                gridApi.itemUpdated(routeRule);
            }

            WhS_BE_MainService.editRouteRule(routeRuleObj, onRouteRuleUpdated);
        }

        function deleteRouteRule(routeRuleObj) {
            var onRouteRuleDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            WhS_BE_MainService.deleteRouteRule(routeRuleObj, onRouteRuleDeleted);
        }
    }

    appControllers.controller('WhS_BE_RouteRuleGridController', routeRuleGridController);
})(appControllers);