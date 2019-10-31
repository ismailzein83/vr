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
            var requiredFieldTitles = [];

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.validateRequiredFields = function () {
                    if (ctrl.customvalidate != undefined && typeof (ctrl.customvalidate) == "function") {
                        var validation = ctrl.customvalidate();
                        if (validation != undefined)
                            return validation;
                    }

                    if (requiredFieldTitles.length == 0)
                        return null;

                    var selectedFieldTitles = VRUIUtilsService.getIdSelectedIds('Title', attrs, ctrl);
                    if (selectedFieldTitles == undefined)
                        return "Required Fields: " + requiredFieldTitles.join(', ');

                    if (!Array.isArray(selectedFieldTitles)) {
                        if (requiredFieldTitles.length == 1 && requiredFieldTitles.includes(selectedFieldTitles))
                            return null;
                        else
                            return "Required Fields: " + requiredFieldTitles.join(', ');
                    }
                    else {
                        var requiredFieldsNotSelected = [];
                        for (var i = 0; i < requiredFieldTitles.length; i++) {
                            var currentRequiredField = requiredFieldTitles[i];
                            if (!selectedFieldTitles.includes(currentRequiredField)) {
                                requiredFieldsNotSelected.push(currentRequiredField);
                            }
                        }

                        return requiredFieldsNotSelected.length > 0 ? "Required Fields: " + requiredFieldsNotSelected.join(', ') : null;
                    }

                    return null;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    requiredFieldTitles.length = 0;
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
                            var defaultSelectedIds = [];
                            for (var i = 0; i < response.length; i++) {
                                var currentField = response[i];
                                ctrl.datasource.push(currentField);

                                if (currentField.IsSelected) {
                                    defaultSelectedIds.push(currentField.Name);
                                }

                                if (currentField.IsRequired) {
                                    requiredFieldTitles.push(currentField.Title);
                                }
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                            }
                            else if (defaultSelectedIds.length > 0) {
                                VRUIUtilsService.setSelectedValues(defaultSelectedIds, 'Name', attrs, ctrl);
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
            var hideSelectAll = "";
            var hideClearAll = "";

            if (attrs.hideselectall != undefined)
                hideSelectAll = "hideselectall";

            if (attrs.hideclearall != undefined)
                hideClearAll = "hideclearall";

            var label = "Output Field";

            if (attrs.ismultipleselection != undefined) {
                label = "Output Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var template =
                '<vr-select ' + multipleselection + ' datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired" ' + hideSelectAll + ' ' + hideClearAll + ' label="' + label +
                '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" onbeforeselectionchanged="ctrl.onbeforeselectionchanged" customvalidate="ctrl.validateRequiredFields()" entityName="' + label +
                '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon">' +
                '</vr-select>';

            return template;
        }

    }]);