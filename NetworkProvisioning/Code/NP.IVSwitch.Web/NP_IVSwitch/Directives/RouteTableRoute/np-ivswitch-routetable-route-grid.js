'use strict';

app.directive('npIvswitchRoutetableRouteGrid', ['NP_IVSwitch_RouteTableRouteAPIService', 'UtilsService', 'VRNotificationService', 'NP_IVSwitch_RouteTableRouteService', 'VRUIUtilsService', 'NP_IVSwitch_RouteTableViewTypeEnum', function (NP_IVSwitch_RouteTableRouteAPIService, UtilsService, VRNotificationService, NP_IVSwitch_RouteTableRouteService, VRUIUtilsService, NP_IVSwitch_RouteTableViewTypeEnum) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.showAccount = true;
            var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
            financialAccountGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/RouteTableRoute/Templates/RouteTableRouteGridTemplate.html"
    };

    function FinancialAccountGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var routeTableId;
        var routeTableViewType;
        var gridAPI;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.routeOptions = [];
            $scope.scopeModel.routeTablesRT = [];
            $scope.scopeModel.menuActions = [];
            var datasources = [];
            $scope.scopeModel.limit = 100;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            function defineAPI() {

                var api = {};
                api.loadGrid = function (payload) {

                    if (payload != undefined) {
                        gridAPI.clearAll();
                        gridAPI.retrieveData(payload);
                    };
                    if (payload != undefined && payload.RouteTableViewType != undefined && payload.RouteTableId != undefined) {
                        routeTableViewType = payload.RouteTableViewType;
                        routeTableId = payload.RouteTableId;
                    };

                    if (routeTableViewType == NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value)
                        $scope.scopeModel.isANumber = true;

                    else
                        $scope.scopeModel.isANumber = false;
                };

                api.onRouteTableRouteAdded = function () {
                    gridAPI.clearAll();
                    return gridAPI.retrieveData(getFilter());
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                };

            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return NP_IVSwitch_RouteTableRouteAPIService.GetFilteredRouteTableRoutes(dataRetrievalInput).then(function (response) {
                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data[i];
                        if ($scope.scopeModel.isANumber == true)
                            var dataItem = { RouteOptionsDetailId: "RouteOptionsDetailId" + (i + 1), Destination: item.Destination, TechPrefix: item.TechPrefix, Options: item.RouteOptionsDetails };
                        else
                            var dataItem = { RouteOptionsDetailId: "RouteOptionsDetailId" + (i + 1), Destination: item.Destination, Options: item.RouteOptionsDetails };
                        $scope.scopeModel.routeTablesRT.push(dataItem);
                    };

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        };

        function getFilter() {
            var filter = {
                RouteTableId: routeTableId,
                ANumber: $scope.scopeModel.aNumber,
                Bnumber: $scope.scopeModel.bNumber,
                RouteTableViewType: routeTableViewType,
                Limit: $scope.scopeModel.limit
            };
            return filter;
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editRouteTableRoute,

            }];
            $scope.scopeModel.gridMenuActions.push({
                name: 'Delete',
                clicked: deleteRouteTableRoute,
            });
        };

        function editRouteTableRoute(routeTableEntity) {
            var routeTable = {
                RouteTableId: routeTableId,
                Destination: routeTableEntity.Destination
            };
            var onRouteTableUpdated = function (routeTable) {
                $scope.scopeModel.routeTablesRT = [];
                gridAPI.clearAll();
                gridAPI.retrieveData(getFilter());
            };
            NP_IVSwitch_RouteTableRouteService.editRouteTableRoutes(routeTable, onRouteTableUpdated);
        };

        function deleteRouteTableRoute(routeTable) {

            VRNotificationService.showConfirmation().then(function (response) {
                if (response) {
                    NP_IVSwitch_RouteTableRouteAPIService.DeleteRouteTableRoute(routeTableId, routeTable.Destination).then(function () {
                        gridAPI.itemDeleted({ RouteOptionsDetailId: routeTable.RouteOptionsDetailId, Destination: routeTable.Destination, TechPrefix: routeTable.TechPrefix, Options: routeTable.Options });

                    })
                };
            });


        };
    };

}]);