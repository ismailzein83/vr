(function (app) {

    'use strict';

    RouteruleViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'WhS_Routing_RouteRuleService'];

    function RouteruleViewDirective(UtilsService, VRNotificationService, WhS_Routing_RouteRuleService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RouteruleViewDirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteRule/Templates/RouteRuleViewTemplate.html"
        };

        function RouteruleViewDirectiveCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var defaultRouteRuleValues;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addRouteRule = function () {
                    var onRouteRuleAdded = function (addedItem) {
                        gridAPI.onRouteRuleAdded(addedItem);
                    };
                    var context = { defaultRouteRuleValues: defaultRouteRuleValues };
                    WhS_Routing_RouteRuleService.addRouteRule(onRouteRuleAdded, context);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        defaultRouteRuleValues = payload.defaultRouteRuleValues;
                    }
                    $scope.scopeModel.isGridLoading = true;
                    return gridAPI.loadGrid(buildGridPayload(payload)).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload(loadPayload) {
                return loadPayload.query;
            }
        }
    }

    app.directive('vrWhsRoutingRouteruleView', RouteruleViewDirective);

})(app);