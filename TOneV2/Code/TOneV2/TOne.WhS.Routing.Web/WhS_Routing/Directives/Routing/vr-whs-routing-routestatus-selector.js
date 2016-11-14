'use strict';

app.directive('vrWhsRoutingRoutestatusSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteStatusEnum',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RouteStatusEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var routeStatusSelector = new RouteStatusSelectorCtor(ctrl, $scope, $attrs);
                routeStatusSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {

                return "/Client/Modules/WhS_Routing/Directives/Routing/Templates/RouteStatusSelectorTemplate.html"
            }
        }

        function RouteStatusSelectorCtor(ctrl, $scope, attrs) {

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    ctrl.datasource = UtilsService.getArrayEnum(WhS_Routing_RouteStatusEnum);

                    if (selectedIds != undefined) {
                        ctrl.selectedvalues = UtilsService.getEnum(WhS_Routing_RouteStatusEnum, 'value', selectedIds);
                    }
                };

                api.getSelectedIds = function () {

                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
    }]);