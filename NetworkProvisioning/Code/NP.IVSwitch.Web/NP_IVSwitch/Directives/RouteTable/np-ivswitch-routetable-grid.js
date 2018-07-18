"use strict";
app.directive("npIvswitchRoutetableGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_RouteTableAPIService", "NP_IVSwitch_RouteTableService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, NP_IVSwitch_RouteTableAPIService, NP_IVSwitch_RouteTableService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var routeTableGrid = new RouteTableGrid($scope, ctrl, $attrs);
            routeTableGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/RouteTable/Templates/RouteTableGridTemplate.html"
    };

    function RouteTableGrid($scope, ctrl) {

        var gridApi;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.routeTables = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        return gridApi.retrieveData(payload.query);
                    };

                    directiveApi.onRouteTableAdded = function (routeTable) {
                        gridApi.itemAdded(routeTable);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return NP_IVSwitch_RouteTableAPIService.GetFilteredRouteTables(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editRouteTable,

            }];
        };

        function editRouteTable(routeTable) {
            var onRouteTableUpdated = function (routeTable) {
                gridApi.itemUpdated(routeTable);
            };
            NP_IVSwitch_RouteTableService.editRouteTable(routeTable.RouteTableId, onRouteTableUpdated);
        };
      
    };
    return directiveDefinitionObject;
}]);
