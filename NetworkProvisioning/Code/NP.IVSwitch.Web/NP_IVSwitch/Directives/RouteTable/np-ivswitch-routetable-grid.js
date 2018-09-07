"use strict";
app.directive("npIvswitchRoutetableGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_RouteTableAPIService", "NP_IVSwitch_RouteTableService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "NP_IVSwitch_RouteTableViewTypeEnum",
function (UtilsService, VRNotificationService, NP_IVSwitch_RouteTableAPIService, NP_IVSwitch_RouteTableService, VRUIUtilsService, VRCommon_ObjectTrackingService, NP_IVSwitch_RouteTableViewTypeEnum) {

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
        var gridDrillDownTabsObj;
        var finalDrillDownDefinitions = [];
        var gridApi;
        var routeTableViewType;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.routeTables = [];

            $scope.scopeModel.onGridReady = function (api) {
               gridApi = api;
               finalDrillDownDefinitions.push(defineObjectRouteTableRouteDrillDown());
               function defineObjectRouteTableRouteDrillDown() {
                   var drillDownDefinition = {};
                   drillDownDefinition.title = "Routes";
                   drillDownDefinition.directive = "np-ivswitch-routetable-route-search";
                   drillDownDefinition.dontLoad = true;
                   drillDownDefinition.loadDirective = function (directiveAPI, routeTableItem) {
                       var query = {
                           RouteTableId: routeTableItem.RouteTableId,
                           RouteTableViewType: routeTableViewType
                       };
                       return directiveAPI.load(query);
                   }
                   return drillDownDefinition;
               };
                   gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(finalDrillDownDefinitions, gridApi, $scope.menuActions, true);
                defineAPI();
            };

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    routeTableViewType = payload.RouteTableViewType;
                    return gridApi.retrieveData(payload.Filter.query);
                };
                api.onRouteTableAdded = function (routeTable) {
                    gridApi.itemAdded(routeTable);
                };
                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
              };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_RouteTableAPIService.GetFilteredRouteTables(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
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
            $scope.scopeModel.gridMenuActions.push({
                name: 'Delete',
                clicked: deleteRoute,
            });
        };
        
        function editRouteTable(routeTableEntity) {
            var routeTable = {
                RouteTableId: routeTableEntity.RouteTableId,
                RouteTableViewType: routeTableViewType
            };
            var onRouteTableUpdated = function (routeTable) {
                gridApi.itemUpdated(routeTable);
                gridDrillDownTabsObj.setDrillDownExtensionObject(routeTable);

            };
            NP_IVSwitch_RouteTableService.editRouteTable(routeTable, onRouteTableUpdated);
        };

        function deleteRoute(routeTable) {
            VRNotificationService.showConfirmation().then(function (response) {
                if (response) {
                    var RouteTableId = routeTable.RouteTableId;
                    var RouteTableViewType = (routeTableViewType == NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value) ? NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value : NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value;
                    NP_IVSwitch_RouteTableAPIService.DeleteRouteTable(RouteTableId, routeTableViewType).then(function () {
                        gridApi.itemDeleted({ RouteTableId: RouteTableId, Name: routeTable.Name, Description: routeTable.Description });


                    });

                };
            });

        };

    };
    return directiveDefinitionObject;
}]);
