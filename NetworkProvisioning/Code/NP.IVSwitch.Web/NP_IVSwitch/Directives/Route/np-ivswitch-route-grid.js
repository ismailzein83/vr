'use strict';

app.directive('npIvswitchRouteGrid', ['NP_IVSwitch_RouteAPIService', 'NP_IVSwitch_RouteService', 'VRNotificationService', 'VRUIUtilsService',
    function (NP_IVSwitch_RouteAPIService, NP_IVSwitch_RouteService, VRNotificationService, VRUIUtilsService) {
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
            var gridDrillDownTabsObj;
             var carrierAccountId;

             function initializeController() {
                 $scope.scopeModel = {};
                 $scope.scopeModel.route = [];
                 $scope.scopeModel.menuActions = [];

                 $scope.scopeModel.addRoute = function () {;
                     var onRouteAdded = function (addedRoute) {
                         gridDrillDownTabsObj.setDrillDownExtensionObject(addedRoute);
                         gridAPI.itemAdded(addedRoute);
                     };
                     NP_IVSwitch_RouteService.addRoute(carrierAccountId, onRouteAdded);
                 };
                 $scope.scopeModel.hadAddRoutePermission = function () {
                     return NP_IVSwitch_RouteAPIService.HasEditRoutePermission();
                 };

                 $scope.scopeModel.onGridReady = function (api) {
                     gridAPI = api;
                     var drillDownDefinitions = NP_IVSwitch_RouteService.getDrillDownDefinition();
                     gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                     defineAPI();
                 };
                 $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                     return NP_IVSwitch_RouteAPIService.GetFilteredRoutes(dataRetrievalInput).then(function (response) {
                         if (response !=undefined && response.Data != undefined) {
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
                     carrierAccountId = query != undefined && query.CarrierAccountId != undefined ? query.CarrierAccountId : undefined;
                    return gridAPI.retrieveData(query);
                };

                 api.onRouteAdded = function (addedRoute) {
                     gridDrillDownTabsObj.setDrillDownExtensionObject(addedRoute);
                    gridAPI.itemAdded(addedRoute);
                };

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
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedRoute);
                    gridAPI.itemUpdated(updatedRoute);
                };         
            

                NP_IVSwitch_RouteService.editRoute(RouteItem.Entity.RouteId, carrierAccountId, onRouteUpdated);
            }
            function hasEditRoutePermission() {
                return NP_IVSwitch_RouteAPIService.HasEditRoutePermission();
            }

        }
    }]);

