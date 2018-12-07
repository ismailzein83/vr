'use strict';
app.directive('vrCommonDynamicapiCustomcodeMethodType', ['VRUIUtilsService', 'UtilsService', 'VRCommon_CustomCodeDynamicAPIMethodTypeEnum',
    function (VRUIUtilsService, UtilsService, VRCommon_CustomCodeDynamicAPIMethodTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@',
                onselectitem: "=",
                ondeselectitem: "=",
                hideselectedvaluessection: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new MethodTypeSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {
            var label = "Method Type";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Method Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="ctrl.onSelectorReady"'
                + '  selectedvalues="ctrl.selectedvalues"'
                + '  onselectionchanged="ctrl.onselectionchanged"'
                + '  datasource="ctrl.datasource"'
                + '  onselectitem="ctrl.onselectitem"'
                + '  ondeselectitem="ctrl.ondeselectitem"'
                + '  datavaluefield="value"'
                + '  datatextfield="description"'
                + '  ' + multipleselection
                + '  ' + hideremoveicon
                + '  ' + hideselectedvaluessection
                + '  isrequired="ctrl.isrequired"'
                + '  label="' + label + '"'
                + ' entityName="' + label + '" >'
                + '</vr-select>'
                + '</vr-columns>';
        }

        function MethodTypeSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
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
                    ctrl.datasource = UtilsService.getArrayEnum(VRCommon_CustomCodeDynamicAPIMethodTypeEnum);
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);