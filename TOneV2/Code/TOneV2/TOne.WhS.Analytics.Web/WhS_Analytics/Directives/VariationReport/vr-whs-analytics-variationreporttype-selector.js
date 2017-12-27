'use strict';
app.directive('vrWhsAnalyticsVariationreporttypeSelector', ['WhS_Analytics_VariationReportTypeEnum', 'UtilsService', 'VRUIUtilsService', 'SecurityService',
    function (WhS_Analytics_VariationReportTypeEnum, UtilsService, VRUIUtilsService, SecurityService) {

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
            template: function (element, attrs) {
                return getVariationTemplate(attrs);
            }

        };

        function getVariationTemplate(attrs) {

            var multipleselection = "";
            var label = "Variation";
            if (attrs.ismultipleselection != undefined) {
                label = "Variations";
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
                var allDataSource = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTypeEnum);
                var reportTypes = UtilsService.getFilteredArrayFromArray(allDataSource, true, 'isVisible');


                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };              
                api.load = function (payload) {

                    return SecurityService.IsAllowed("BillingData: View").then(function (response) {
                        ctrl.datasource.length = 0;
                        angular.forEach(reportTypes, function (itm) {
                            if (itm.isBilling == false || (response == true))
                                ctrl.datasource.push(itm);
                        });

                        var selectedIds;
                        var selectfirstitem;
                        if (payload != undefined) {
                            selectedIds = payload.selectedIds;
                            selectfirstitem = payload.selectfirstitem != undefined && payload.selectfirstitem == true;
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                        else if (selectfirstitem == true) {
                            var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].value] : ctrl.datasource[0].value;
                            VRUIUtilsService.setSelectedValues(defaultValue, 'value', attrs, ctrl);
                        }
                    });

                   
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
