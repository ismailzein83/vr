(function (app) {

    'use strict';

    SubstringStartDirectionSelectorDirective.$inject = ['VR_Rules_SubstringStartDirectionEnum', 'UtilsService', 'VRUIUtilsService'];

    function SubstringStartDirectionSelectorDirective(VR_Rules_SubstringStartDirectionEnum, UtilsService, VRUIUtilsService) {

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

                var ctor = new SubstringStartDirectionSelector(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function SubstringStartDirectionSelector(ctrl, $scope, attrs) {
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
                    ctrl.datasource = UtilsService.getArrayEnum(VR_Rules_SubstringStartDirectionEnum);

                    var selectedIds;
                    var selectFirstItem;

                    if (payload) {
                        selectedIds = payload.selectedIds;
                        selectFirstItem = payload.selectFirstItem;
                    }

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                    else if (selectFirstItem) {
                        if (attrs.ismultipleselection != undefined) {
                            ctrl.selectedvalues.push(ctrl.datasource[0]);
                        } else {
                            ctrl.selectedvalues = ctrl.datasource[0];
                        }
                    }
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var label = 'Start Direction';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = 'Start Directions';
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

    app.directive('vrRulesSubstringstartdirectionSelector', SubstringStartDirectionSelectorDirective);

})(app);
