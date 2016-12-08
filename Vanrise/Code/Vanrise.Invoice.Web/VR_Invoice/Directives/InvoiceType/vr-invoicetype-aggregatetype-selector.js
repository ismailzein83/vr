(function (app) {

    'use strict';

    AggregatetypeSelectorDirective.$inject = ['VR_Invoice_AggregateTypeEnum', 'UtilsService', 'VRUIUtilsService'];

    function AggregatetypeSelectorDirective(VR_Invoice_AggregateTypeEnum, UtilsService, VRUIUtilsService) {

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

                var aggregatetype = new AggregatetypeSelector(ctrl, $scope, $attrs);
                aggregatetype.initializeController();
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

        function AggregatetypeSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.datasource = UtilsService.getArrayEnum(VR_Invoice_AggregateTypeEnum);

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
            var label = attrs.label ? attrs.label : 'Aggregate Type';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = label == 'Aggregate Type' ? 'Aggregate Types' : label;
                ismultipleselection = ' ismultipleselection';
            }
            var fullLabel = ' label ="' + label + '"';
            if (attrs.hidelabel != undefined)
                fullLabel = " ";
            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon != null) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
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
                    + ' entityName="' + label + '" '
                     + fullLabel + '>'

                + '</vr-select>'
            + '</vr-columns>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrInvoicetypeAggregatetypeSelector', AggregatetypeSelectorDirective);

})(app);
