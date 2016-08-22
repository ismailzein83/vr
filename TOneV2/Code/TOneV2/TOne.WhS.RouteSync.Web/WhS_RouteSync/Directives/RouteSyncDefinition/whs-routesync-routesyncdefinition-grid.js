'use strict';

app.directive('whsRoutesyncRoutesyncdefinitionGrid', ['WhS_RouteSync_RouteSyncDefinitionAPIService', 'WhS_RouteSync_RouteSyncDefinitionService', 'VRNotificationService',
    function (WhS_RouteSync_RouteSyncDefinitionAPIService, WhS_RouteSync_RouteSyncDefinitionService, VRNotificationService) {
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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.routeSyncDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_RouteSync_RouteSyncDefinitionAPIService.GetFilteredRouteSyncDefinitions(dataRetrievalInput).then(function (response) {
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
                    gridAPI.itemAdded(addedRouteSyncDefinition);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editRouteSyncDefinition,
                });
            }

            function editRouteSyncDefinition(routeSyncDefinitionItem) {
                var onRouteSyncDefinitionUpdated = function (updatedRouteSyncDefinition) {
                    gridAPI.itemUpdated(updatedRouteSyncDefinition);
                };

                WhS_RouteSync_RouteSyncDefinitionService.editRouteSyncDefinition(routeSyncDefinitionItem.Entity.RouteSyncDefinitionId, onRouteSyncDefinitionUpdated);
            }
        }
    }]);
