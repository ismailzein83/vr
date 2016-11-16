'use strict';
app.directive('vrWhsRoutingRouterulesettingsFilterService', ['UtilsService',
    function (UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new serviceCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Filter/Templates/FilterByServiceDirective.html';
            }

        };

        function serviceCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Filters.ServiceOptionFilter, TOne.WhS.Routing.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
}]);