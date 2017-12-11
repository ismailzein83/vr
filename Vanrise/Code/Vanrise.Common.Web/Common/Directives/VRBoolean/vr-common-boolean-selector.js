'use strict';
app.directive('vrCommonBooleanSelector', ['UtilsService', 'VRUIUtilsService',
function ( UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '=',
            normalColNum: '@'

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];
            var ctor = new booleanSelectorCtor(ctrl, $scope, $attrs);
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
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };


    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "";
        if (attrs.ismultipleselection != undefined) {
            multipleselection = "ismultipleselection";
        }

        if (attrs.label != undefined)
            label = attrs.label;

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
            + '<vr-select ' + multipleselection +  ' datatextfield="description" datavaluefield="value" '
        + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" on-ready="onSelectorReady"></vr-select>'
           + '</vr-columns>';
    }

    function booleanSelectorCtor(ctrl, $scope, $attrs) {
        var selectorAPI;
        function initializeController() {
            $scope.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', $attrs, ctrl);
            };
            api.load = function (payload) {
                selectorAPI.clearDataSource();
                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }
                ctrl.datasource = [{
                    value: true,
                    description: "True"
                }, {
                    value: false,
                    description: "False"
                }];
                if (selectedIds != undefined)
                    VRUIUtilsService.setSelectedValues(selectedIds, 'value', $attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);