"use strict";

app.directive("npIvswitchRoutetableRouteSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'NP_IVSwitch_RouteTableRouteService', 'NP_IVSwitch_RouteTableViewTypeEnum',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, NP_IVSwitch_RouteTableRouteService, NP_IVSwitch_RouteTableViewTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new RouteTableRTSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/RouteTableRoute/Templates/RouteTableRouteSearchTemplate.html"

    };

    function RouteTableRTSearch($scope, ctrl, $attrs) {
        var gridAPI;
        var routeTableId;
        var routeTableViewType;

        var gridAPIDirectiveReadyDeffered = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.limit = 100;
            $scope.scopeModel.canSearch = function () {
                if ($scope.scopeModel.limit == "" || $scope.scopeModel.limit == undefined || $scope.scopeModel.limit == null || parseInt($scope.scopeModel.limit)<1 ||parseInt($scope.scopeModel.limit)>999999999)
                    return true;
                return false;

            };

            $scope.scopeModel.addRouteTableRT = function () {
                var onRouteTableRTAdded = function (obj) {
                    if (gridAPI != undefined)
                        gridAPI.onRouteTableRouteAdded(obj);
                };
                NP_IVSwitch_RouteTableRouteService.addRouteTableRoutes(onRouteTableRTAdded, routeTableId);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPIDirectiveReadyDeffered.resolve();


            };

            $scope.scopeModel.search = function () {
                gridAPI.loadGrid(getFilter());
            };

            defineAPI();

        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    routeTableId = payload.RouteTableId;
                    routeTableViewType = payload.RouteTableViewType;
                    $scope.scopeModel.isANumber = (NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value == routeTableViewType) ? true : false;

                };

                var promises = [];
                gridAPIDirectiveReadyDeffered.promise.then(function () {
                    promises.push(gridAPI.loadGrid(getFilter()));

                });

                return UtilsService.waitMultiplePromises(promises);

            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
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


    }

    return directiveDefinitionObject;

}]);
