(function (app) {

    'use strict';

    MonthSelectorDirective.$inject = ['VRCommon_MonthEnum', 'UtilsService', 'VRUIUtilsService'];

    function MonthSelectorDirective(VRCommon_MonthEnum, UtilsService, VRUIUtilsService) {

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
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];
                
                var monthSelector = new MonthSelector(ctrl, $scope, $attrs);
                monthSelector.initializeController();
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

        function MonthSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
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
                    selectorAPI.clearDataSource();
                    ctrl.datasource = UtilsService.getArrayEnum(VRCommon_MonthEnum);
                  
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
            var label ='Month';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = 'Months';
                ismultipleselection = ' ismultipleselection';
            }

            if (attrs.label != undefined)
                label = attrs.label;

            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' label="' + label + '"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="value"'
                    + ' datatextfield="description"'
                    + ismultipleselection
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' isrequired="ctrl.isrequired"'
                    + hideremoveicon
                    + ' entityName="' + label + '">'
                + '</vr-select>'
            + '</div>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonMonthSelector', MonthSelectorDirective);

})(app);
