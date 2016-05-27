'use strict';

app.directive('vrAnalyticOrdertypeSelector', ['VR_Analytic_OrderTypeEnum', 'UtilsService', 'VRUIUtilsService', function (VR_Analytic_OrderTypeEnum, UtilsService, VRUIUtilsService) {

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
            isdisabled: '=',
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var orderTypeSelector = new OrderTypeSelector(ctrl, $scope, $attrs);
            orderTypeSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getUserTemplate(attrs);
        }
    };

    function OrderTypeSelector(ctrl, $scope, attrs) {

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

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                ctrl.datasource = UtilsService.getArrayEnum(VR_Analytic_OrderTypeEnum);

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                }
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getUserTemplate(attrs) {

        var multipleselection = "";
        var label = "Order Type";

        if (attrs.ismultipleselection != undefined) {
            label = "Order Types";
            multipleselection = "ismultipleselection";
        }

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon"></vr-select></vr-columns>';
    }

}]);