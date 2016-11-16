'use strict';

app.directive('npIvswitchRouteGrid', ['NP_IVSwitch_RouteAPIService', 'NP_IVSwitch_RouteService', 'VRNotificationService',
    function (NP_IVSwitch_RouteAPIService, NP_IVSwitch_RouteService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var routeGrid = new RouteGrid($scope, ctrl, $attrs);
                routeGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/Route/Templates/RouteGridTemplate.html'
        };

        function RouteGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
             var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.route = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_RouteAPIService.GetFilteredRoutes(dataRetrievalInput).then(function (response) {
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

                api.onRouteAdded = function (addedRoute) {
                     gridAPI.itemAdded(addedRoute);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editRoute,
                    haspermission: hasEditRoutePermission
                });
            }
            function editRoute(RouteItem) {
                var onRouteUpdated = function (updatedRoute) {
                    gridAPI.itemUpdated(updatedRoute);
                };

                NP_IVSwitch_RouteService.editRoute(RouteItem.Entity.RouteId, onRouteUpdated);
            }
            function hasEditRoutePermission() {
                return NP_IVSwitch_RouteAPIService.HasEditRoutePermission();
            }

        }
    }]);

