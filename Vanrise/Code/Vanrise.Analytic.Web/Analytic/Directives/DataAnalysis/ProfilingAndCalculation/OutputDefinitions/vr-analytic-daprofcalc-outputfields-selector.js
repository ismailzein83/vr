'use strict';

app.directive('vrAnalyticDaprofcalcOutputfieldsSelector', ['VR_Analytic_DAProfCalcOutputSettingsAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Analytic_DAProfCalcOutputSettingsAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '=',
                onbeforeselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var daprofcalcOutputfieldsSelector = new DaprofcalcOutputfieldsSelector(ctrl, $scope, $attrs);
                daprofcalcOutputfieldsSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DaprofcalcOutputfieldsSelector(ctrl, $scope, attrs) {

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
                    var filter;
                    var dataAnalysisItemDefinitionId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        dataAnalysisItemDefinitionId = payload.dataAnalysisItemDefinitionId;
                    }

                    var serializedFilter = UtilsService.serializetoJson(filter) != undefined ? UtilsService.serializetoJson(filter) : {};

                    return VR_Analytic_DAProfCalcOutputSettingsAPIService.GetFilteredOutputFields(dataAnalysisItemDefinitionId, serializedFilter).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Output Field";

            if (attrs.ismultipleselection != undefined) {
                label = "Output Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var template =
                '<vr-select ' + multipleselection + ' datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired" label="' + label +
                    '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" onbeforeselectionchanged="ctrl.onbeforeselectionchanged" entityName="' + label +
                    '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                '</vr-select>';

            return template;
        }

    }]);