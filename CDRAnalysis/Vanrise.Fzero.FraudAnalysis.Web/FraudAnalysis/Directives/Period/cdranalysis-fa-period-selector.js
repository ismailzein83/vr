(function (app) {

    'use strict';

    PeriodSelectorDirective.$inject = ['CDRAnalysis_FA_PeriodAPIService', 'UtilsService', 'VRUIUtilsService'];

    function PeriodSelectorDirective(CDRAnalysis_FA_PeriodAPIService, UtilsService, VRUIUtilsService) {

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
                isrequired: '@',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined && $attrs.ismultipleselection != null)
                    ctrl.selectedvalues = [];

                ctrl.isrequired = ($attrs.isrequired != undefined && $attrs.isrequired != null);

                var periodSelector = new PeriodSelector(ctrl, $scope, $attrs);
                periodSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function PeriodSelector(ctrl, $scope, attrs) {
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
                    var selectedIds;

                    if (payload) {
                        selectedIds = payload.selectedIds;
                    }

                    return CDRAnalysis_FA_PeriodAPIService.GetPeriods().then(function (response) {
                        selectorAPI.clearDataSource();
                        
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Id', attrs, ctrl);
                        }
                    });
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Id', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var label = attrs.label ? attrs.label : 'Period';

            var ismultipleselection = '';
            if (attrs.ismultipleselection != undefined && attrs.ismultipleselection != null) {
                label = label == 'Period' ? 'Periods' : label;
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
                    + ' datavaluefield="Id"'
                    + ' datatextfield="Name"'
                    + ismultipleselection
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' isrequired="ctrl.isrequired"'
                    + hideremoveicon
                    + ' entityName="' + label + '">'
                + '</vr-select>'
            + '</div>'
        }

        return directiveDefinitionObject;
    }

    app.directive('cdranalysisFaPeriodSelector', PeriodSelectorDirective);

})(app);
