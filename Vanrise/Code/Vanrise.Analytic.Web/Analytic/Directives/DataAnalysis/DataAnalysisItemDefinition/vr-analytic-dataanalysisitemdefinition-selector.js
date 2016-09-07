﻿'use strict';

app.directive('vrAnalyticDataanalysisdefinitionSelector', ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Analytic_DataAnalysisItemDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var dataAnalysisItemDefinitionSelector = new DataAnalysisItemDefinitionSelector(ctrl, $scope, $attrs);
                dataAnalysisItemDefinitionSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DataAnalysisItemDefinitionSelector(ctrl, $scope, attrs) {

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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                    }

                    return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinitionsInfo(UtilsService.serializetoJson(filter), dataAnalysisDefinitionId).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'DataAnalysisItemDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DataAnalysisItemDefinitionId', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Data Analysis Item Definition";

            if (attrs.ismultipleselection != undefined) {
                label = "Data Analysis Item Definitions";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var template =
                '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="DataAnalysisItemDefinitionId" isrequired="ctrl.isrequired" label="' + label +
                    '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                    '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                '</vr-select>'

            return template;
        }

    }]);