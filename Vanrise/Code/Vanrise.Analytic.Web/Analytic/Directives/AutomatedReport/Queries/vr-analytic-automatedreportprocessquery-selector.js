'use strict';

app.directive('vrAnalyticAutomatedreportprocessquerySelector', ['VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService', 'UtilsService','VRUIUtilsService',

    function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService) {
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
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var automatedReportProcessScheduledSelector = new AutomatedReportProcessScheduledSelector(ctrl, $scope, $attrs);
                automatedReportProcessScheduledSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function AutomatedReportProcessScheduledSelector(ctrl, $scope, attrs) {

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
                api.load = function (payload)   {

                     var selectedIds;

                     if (payload != undefined) {
                         selectedIds = payload.selectedIds;

                     }

                     var filter = {};
                   
                     VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionsInfo(filter).then(function (response) {
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
            var label = "Definition";
            var hideremoveicon = '';

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select datatextfield="Name" datavaluefield="DefinitionId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>';
        }

    }]);