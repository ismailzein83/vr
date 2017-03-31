'use strict';

app.directive('whsRoutesyncRoutesyncdefinitionGrid', ['WhS_RouteSync_RouteSyncDefinitionAPIService', 'WhS_RouteSync_RouteSyncDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (WhS_RouteSync_RouteSyncDefinitionAPIService, WhS_RouteSync_RouteSyncDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var routeSyncDefinitionGrid = new RouteSyncDefinitionGrid($scope, ctrl, $attrs);
                routeSyncDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/RouteSyncDefinition/Templates/RouteSyncDefinitionGridTemplate.html'
        };

        function RouteSyncDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.routeSyncDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = WhS_RouteSync_RouteSyncDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_RouteSync_RouteSyncDefinitionAPIService.GetFilteredRouteSyncDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onRouteSyncDefinitionAdded = function (addedRouteSyncDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedRouteSyncDefinition);
                    gridAPI.itemAdded(addedRouteSyncDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editRouteSyncDefinition,
                    haspermission: hasEditRouteSyncDefinitionPermission
                });
            }

            function editRouteSyncDefinition(routeSyncDefinitionItem) {
                var onRouteSyncDefinitionUpdated = function (updatedRouteSyncDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedRouteSyncDefinition);
                    gridAPI.itemUpdated(updatedRouteSyncDefinition);
                };

                WhS_RouteSync_RouteSyncDefinitionService.editRouteSyncDefinition(routeSyncDefinitionItem.Entity.RouteSyncDefinitionId, onRouteSyncDefinitionUpdated);
            }
            function hasEditRouteSyncDefinitionPermission() {
                return WhS_RouteSync_RouteSyncDefinitionAPIService.HasEditRouteSyncDefinitionPermission();
            }
        }
    }]);
