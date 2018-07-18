(function (appControllers) {
    "use strict";

    routeTableManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_RouteTableService'];

    function routeTableManagementController($scope, UtilsService, VRUIUtilsService, NP_IVSwitch_RouteTableService) {

        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };

            $scope.scopeModel.addRouteTable = function () {
                var onRouteTableAdded = function (routeTable) {
                        if (gridApi != undefined) {
                            gridApi.onRouteTableAdded(routeTable);
                        }
                    };
                NP_IVSwitch_RouteTableService.addRouteTable(onRouteTableAdded);
            };

        };

        function load() {

        }


        function getFilter() {
            return {
                query:{
                    Name: $scope.scopeModel.name
                }
            };
        };

    };

    appControllers.controller('NP_IVSwitch_RouteTableManagementController', routeTableManagementController);
})(appControllers);