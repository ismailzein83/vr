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
                isrequired: '=',
                hideremoveicon: '@',
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
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };
        function getTemplate(attrs) {
            var label = 'Apply';
            //if (attrs.ismultipleselection != undefined)
            //    label = 'Types';

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-select on-ready="ctrl.onSelectorReady" datasource=" ctrl.datasource" datatextfield="description" datavaluefield="value" selectedvalues="ctrl.selectedvalues" '
           + 'onselectionchanged="ctrl.onselectionchanged" customvalidate="ctrl.customvalidate" ' + hideremoveicon + ' isrequired="ctrl.isrequired" label="' + label + '"></vr-select>';

        }

        function RouteRuleSettingsOrderTypeSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;
            var OrderTypeEnumArray = UtilsService.getArrayEnum(WhS_Routing_OrderTypeEnum);

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var filter;
                    var criteriasLength;
                    var selectedvalues = ctrl.selectedvalues;
                    ctrl.datasource.length = 0;
                    ctrl.selectedvalues = undefined;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    loadDataSource(filter);
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    else if (selectedvalues != undefined && ctrl.datasource.indexOf(selectedvalues) != -1)
                        ctrl.selectedvalues = selectedvalues;
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            var loadDataSource = function (filter) {
                if (filter != undefined) {
                    for (var i = 0; i < OrderTypeEnumArray.length; i++) {
                        if ((OrderTypeEnumArray[i].from == undefined || OrderTypeEnumArray[i].from <= filter.OrderOptionCriteriaLength) && (OrderTypeEnumArray[i].to == undefined || OrderTypeEnumArray[i].to >= filter.OrderOptionCriteriaLength))
                            ctrl.datasource.push(OrderTypeEnumArray[i]);
                    }
                }
            };

        }
    }]);