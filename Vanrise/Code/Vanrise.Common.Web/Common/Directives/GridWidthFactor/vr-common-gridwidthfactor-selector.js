(function (app) {

    'use strict';

    HourSelectorDirective.$inject = ['VRCommon_GridWidthFactorEnum', 'UtilsService', 'VRUIUtilsService'];

    function HourSelectorDirective(VRCommon_GridWidthFactorEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                ismultipleselection: '@',
                isdisabled: '@',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];

                //ctrl.isrequired = ($attrs.isrequired != undefined);

                var hourSelector = new WidthFactorSelector(ctrl, $scope, $attrs);
                hourSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function WidthFactorSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.datasource = UtilsService.getArrayEnum(VRCommon_GridWidthFactorEnum);

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {

                    var selectedIds;

                    if (payload) {
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var label = attrs.label ? attrs.label : 'Width';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = label == 'Width' ? 'Widths' : label;
                ismultipleselection = ' ismultipleselection';
            }
            var fullLabel = ' label ="' + label + '"';
            if (attrs.hidelabel != undefined)
                fullLabel = " ";
            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' 
                + '<span vr-disabled="ctrl.isdisabled"><vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                 
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="value"'
                    + ' datatextfield="description"'
                    + ismultipleselection
                    + ' isrequired="ctrl.isrequired"'
                    + hideremoveicon
                    + ' entityName="' + label + '" '
                     + fullLabel +'>'
               
                + '</vr-select></span>'
            +'</vr-columns>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonGridwidthfactorSelector', HourSelectorDirective);

})(app);
