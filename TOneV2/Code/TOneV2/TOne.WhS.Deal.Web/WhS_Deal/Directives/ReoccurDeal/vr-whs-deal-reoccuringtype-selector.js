(function (app) {
    'use strict';
    vrWhsDealReoccuringtypeSelector.$inject = ['Whs_Deal_ReoccuringTypeEnum', 'UtilsService', 'VRUIUtilsService'];

    function vrWhsDealReoccuringtypeSelector(Whs_Deal_ReoccuringTypeEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];

                var reoccuringTypeSelector = new ReoccuringTypeSelector(ctrl, $scope, $attrs);
                reoccuringTypeSelector.initializeController();
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

        function ReoccuringTypeSelector(ctrl, $scope, attrs) {
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
                    ctrl.datasource = UtilsService.getArrayEnum(Whs_Deal_ReoccuringTypeEnum);
                    var selectedIds;

                    if (payload) {
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined) {
                        ctrl.selectedvalues = UtilsService.getEnum(Whs_Deal_ReoccuringTypeEnum, 'value', selectedIds);
                    }
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var label = 'Recurring Type';

            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' label="' + label + '"'
                + ' datasource="ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' datavaluefield="value"'
                + ' datatextfield="description"'
                + ' isrequired="ctrl.isrequired"'
                + hideremoveicon
                + ' entityName="' + label + '">'
                + '</vr-select>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrWhsDealReoccuringtypeSelector', vrWhsDealReoccuringtypeSelector);

})(app);
