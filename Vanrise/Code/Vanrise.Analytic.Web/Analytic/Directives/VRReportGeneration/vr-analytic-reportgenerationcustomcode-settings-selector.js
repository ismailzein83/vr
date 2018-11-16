'use strict';
app.directive('vrAnalyticReportgenerationcustomcodeSettingsSelector', ['VR_Analytic_ReportGenerationCustomCodeAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Analytic_ReportGenerationCustomCodeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '=',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var customCodeSelector = new ReportGenerationCustomCodeSettingsSelector(ctrl, $scope, $attrs);
                customCodeSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ReportGenerationCustomCodeSettingsSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;

                    }


                    VR_Analytic_ReportGenerationCustomCodeAPIService.GetReportGenerationCustomCodeSettingsInfo().then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                        if (selectedIds != undefined) {
                            ctrl.selectedvalues =
                                UtilsService.getItemByVal(ctrl.datasource, selectedIds, 'DefinitionId');
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var label = attrs.label != undefined ? attrs.label : "Definition";
            var hideremoveicon = '';

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select datatextfield="Name" datavaluefield="DefinitionId" isrequired="ctrl.isrequired" label="' + label +
                '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                '" onselectitem="ctrl.onselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                '</vr-select>';
        }

    }]);

