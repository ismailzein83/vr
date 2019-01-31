'use strict';

app.directive('vrIntegrationImportedbatchExecutionstatusSelector', ['UtilsService', 'VRUIUtilsService', 'VR_Integration_ExecutionStatusEnum',
    function (UtilsService, VRUIUtilsService, VR_Integration_ExecutionStatusEnum) {
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
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new ExecutionStatusCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ExecutionStatusCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSelectorReady = function (api) {
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
                    var setDefaultValue;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        setDefaultValue = payload.setDefaultValue;
                    }
                    var executionsStatus = UtilsService.getArrayEnum(VR_Integration_ExecutionStatusEnum);

                    if (executionsStatus != null) {
                        for (var i = 0; i < executionsStatus.length; i++) {
                            ctrl.datasource.push(executionsStatus[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                        else if (setDefaultValue) {
                            VRUIUtilsService.setSelectedValues(ctrl.datasource[0].value, 'value', attrs, ctrl);
                        }
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Execution Status";

            if (attrs.ismultipleselection != undefined) {
                label = "Executions Status";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            return '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="'
                + label + '" datasource="ctrl.datasource" on-ready="scopeModel.onSelectorReady" '
                + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="'
                +'" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                '</vr-select>';
        }
    }]);