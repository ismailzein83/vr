'use strict';
app.directive('vrWhsAnalyticsBillingreporttypeSelector', ['WhS_Analytics_BillingReportTypeEnum', 'UtilsService', 'VRUIUtilsService',
    function (WhS_Analytics_BillingReportTypeEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new Selector(ctrl, $scope, $attrs);
                selector.initializeController();
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
                return getPhoneNumberTemplate(attrs);
            }

        };

        function getPhoneNumberTemplate(attrs) {

            var multipleselection = "";
            var label = "Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-select ' + multipleselection + ' hideremoveicon datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>';

        }

        function Selector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                ctrl.datasource = UtilsService.getArrayEnum(WhS_Analytics_BillingReportTypeEnum);

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                api.getSelectedValues = function () {
                    return VRUIUtilsService.getIdSelectedIds('description', attrs, ctrl);
                };
                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                    else
                        VRUIUtilsService.setSelectedValues(ctrl.datasource[0].value, 'value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
