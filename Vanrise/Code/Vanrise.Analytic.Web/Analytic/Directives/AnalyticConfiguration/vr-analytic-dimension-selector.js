(function (app) {

    'use strict';

    AnalyticDimensionSelectorDirective.$inject = ['VR_Analytic_AnalyticItemConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticDimensionSelectorDirective(VR_Analytic_AnalyticItemConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
                normalColNum: '=',
                isloading: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var analyticDimensionSelector = new AnalyticDimensionSelector(ctrl, $scope, $attrs);
                analyticDimensionSelector.initializeController();
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

        function AnalyticDimensionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_Analytic_AnalyticItemConfigAPIService.GetDimensionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        ctrl.datasource.length = 0;
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Dimension';
            if (attrs.ismultipleselection != undefined) {
                label = 'Dimensions';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                      + '<span vr-loader="ctrl.isloading">'
                         + '<vr-select on-ready="ctrl.onSelectorReady"'
                             + ' datasource="ctrl.datasource"'
                             + ' selectedvalues="ctrl.selectedvalues"'
                             + ' onselectionchanged="ctrl.onselectionchanged"'
                             + ' onselectitem="ctrl.onselectitem"'
                             + ' ondeselectitem="ctrl.ondeselectitem"'
                             + ' datavaluefield="AnalyticItemConfigId"'
                             + ' datatextfield="Title"'
                             + ' ' + multipleselection
                             + ' ' + hideselectedvaluessection
                             + ' isrequired="ctrl.isrequired"'
                             + ' ' + hideremoveicon
                             + ' vr-disabled="ctrl.isdisabled"'
                             + ' label="' + label + '"'
                             + ' entityName="' + label + '"'
                         + '</vr-select>'
                      + '</span>'
                   + '</vr-columns>';
        }
    }

    app.directive('vrAnalyticDimensionSelector', AnalyticDimensionSelectorDirective);

})(app);
