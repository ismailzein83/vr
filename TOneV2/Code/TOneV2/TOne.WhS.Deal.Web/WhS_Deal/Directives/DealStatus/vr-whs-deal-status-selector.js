(function (app) {

    'use strict';

    StatusSelectorDirective.$inject = ['WhS_Deal_DealStatusTypeEnum', 'UtilsService', 'VRUIUtilsService'];

    function StatusSelectorDirective(WhS_Deal_DealStatusTypeEnum, UtilsService, VRUIUtilsService) {

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

                var statusSelector = new StatusSelector(ctrl, $scope, $attrs);
                statusSelector.initializeController();
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

        function StatusSelector(ctrl, $scope, attrs) {
            var selectorAPI;
            this.initializeController = initializeController;

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

                ctrl.datasource = UtilsService.getArrayEnum(WhS_Deal_DealStatusTypeEnum);

                directiveAPI.load = function (payload) {
                    selectorAPI.clearDataSource();


                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined) {
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
            var label = attrs.label ? attrs.label : 'Deal Status';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                ismultipleselection = ' ismultipleselection';
            }

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

    app.directive('vrWhsDealStatusSelector', StatusSelectorDirective);

})(app);
