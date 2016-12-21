'use strict';

app.directive('vrWhsRoutingRouterulesettingsOrdertypeSelector', ['WhS_Routing_OrderTypeEnum', 'UtilsService', 'VRUIUtilsService',

    function (WhS_Routing_OrderTypeEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var routeRuleSettingsOrderTypeSelector = new RouteRuleSettingsOrderTypeSelector(ctrl, $scope, $attrs);
                routeRuleSettingsOrderTypeSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                 return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Order/Templates/RouteRuleOrderTypeSelector.html';;
            }
        };

        function RouteRuleSettingsOrderTypeSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

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
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    ctrl.datasource = UtilsService.getArrayEnum(WhS_Routing_OrderTypeEnum);
                    //ctrl.selectedvalues = WhS_Routing_OrderTypeEnum.Percentage;

                    if (selectedIds != undefined) {
                        ctrl.selectedvalues = UtilsService.getEnum(WhS_Routing_OrderTypeEnum, 'value', selectedIds);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);