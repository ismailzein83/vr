'use strict';
app.directive('vrQueueingExecutionflowdefinitionSelector', ['VR_Queueing_ExecutionFlowDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Queueing_ExecutionFlowDefinitionAPIService, UtilsService, VRUIUtilsService) {



        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                customlabel: "@",
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];




                var ctor = new executionFlowCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

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
                return getExecutionFlowDefinitionTemplate(attrs);
            }

        };


        function getExecutionFlowDefinitionTemplate(attrs) {

            var multipleselection = "";

            var label = "Execution Flow Definition";
            if (attrs.ismultipleselection != undefined) {
                label = "Execution Flow Definitions";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewExecutionFlow"';
            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<div>'
                + '<vr-select ' + multipleselection + ' ' + hideremoveicon + '  datatextfield="Title" datavaluefield="ID" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Execution Flow Definition" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function executionFlowCtor(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter = {};
                    var selectedIds;

                    if (payload) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;


                    }

                    return VR_Queueing_ExecutionFlowDefinitionAPIService.GetExecutionFlowDefinitions(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ID', attrs, ctrl);
                        }


                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ID', attrs, ctrl);
                };

                api.setDisabled = function (isDisabled) {
                    ctrl.isdisabled = isDisabled;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);