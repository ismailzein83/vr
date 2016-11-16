(function (appControllers) {

    "use strict";

    RouteSyncDefinitionManagementController.$inject = ['$scope', 'WhS_RouteSync_RouteSyncDefinitionAPIService', 'WhS_RouteSync_RouteSyncDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function RouteSyncDefinitionManagementController($scope, WhS_RouteSync_RouteSyncDefinitionAPIService, WhS_RouteSync_RouteSyncDefinitionService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onRouteSyncDefinitionAdded = function (addedRouteSyncDefinition) {
                    gridAPI.onRouteSyncDefinitionAdded(addedRouteSyncDefinition);
                };
                WhS_RouteSync_RouteSyncDefinitionService.addRouteSyncDefinition(onRouteSyncDefinitionAdded);
            };

            $scope.scopeModel.hasAddRouteSyncDefinitionPermission = function () {
                return WhS_RouteSync_RouteSyncDefinitionAPIService.HasAddRouteSyncDefinitionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('WhS_RouteSync_RouteSyncDefinitionManagementController', RouteSyncDefinitionManagementController);

})(appControllers);