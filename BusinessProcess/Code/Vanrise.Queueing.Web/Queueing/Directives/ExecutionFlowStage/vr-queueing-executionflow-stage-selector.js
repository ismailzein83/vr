'use strict';

app.directive('vrQueueingExecutionflowStageSelector', ['VR_Queueing_ExecutionFlowDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Queueing_ExecutionFlowDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onblurdropdown:'=',
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

                var executionFlowStageSelector = new ExecutionFlowStageSelector(ctrl, $scope, $attrs);
                executionFlowStageSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ExecutionFlowStageSelector(ctrl, $scope, attrs) {

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

                    selectorAPI.clearDataSource();
                    var selectedIds;
                    var filter;

                    var executionFlowDefinitionId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        executionFlowDefinitionId = payload.executionFlowDefinitionId;
                    }
                    if (filter != undefined && filter.Filters == undefined)
                        return;

                    if (executionFlowDefinitionId != undefined && executionFlowDefinitionId != 0) {
                        return VR_Queueing_ExecutionFlowDefinitionAPIService.GetExecutionFlowStagesInfo(executionFlowDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }

                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'StageName', attrs, ctrl);
                                }
                            }
                        });
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('StageName', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Execution Flow Stage";

            if (attrs.ismultipleselection != undefined) {
                label = "Execution Flow Stages";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<div>' +
                   '<vr-select ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="StageName" datavaluefield="StageName" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" onblurdropdown="ctrl.onblurdropdown" ondeselectitem="ctrl.ondeselectitem" customvalidate="ctrl.customvalidate">' +
                   '</vr-select>' +
                   '</div>';
        }

    }]);