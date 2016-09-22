'use strict';

app.directive('vrCommonTextfiltertypeSelector', ['VRCommon_TextFilterTypeEnum', 'UtilsService', 'VRUIUtilsService', function (VRCommon_TextFilterTypeEnum, UtilsService, VRUIUtilsService) {
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
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var textFilterTypeSelector = new TextFilterTypeSelector(ctrl, $scope, $attrs);
            textFilterTypeSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function TextFilterTypeSelector(ctrl, $scope, attrs) {
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

                ctrl.datasource = UtilsService.getArrayEnum(VRCommon_TextFilterTypeEnum);

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

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Text Filter Type";

        if (attrs.ismultipleselection != undefined) {
            label = "Text Filter Types";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined && (attrs.hideremoveicon === 'true' || attrs.hideremoveicon === true)) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + '></vr-select></vr-columns>';
    }
}]);