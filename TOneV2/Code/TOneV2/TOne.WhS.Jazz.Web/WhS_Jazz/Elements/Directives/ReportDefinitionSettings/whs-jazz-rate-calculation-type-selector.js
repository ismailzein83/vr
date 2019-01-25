
appControllers.directive('whsJazzRateCalculationTypeSelector', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Jazz_RateCalculationTypeEnum',
    function (VRNotificationService, UtilsService, VRUIUtilsService, WhS_Jazz_RateCalculationTypeEnum) {
        'use strict';

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
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;

                var typeSelector = new TypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getRateCalculationTypeTemplate(attrs);
            }
        };

        function getRateCalculationTypeTemplate(attrs) {


            var multipleselection = "";
            var label = "Rate Calculation Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Rate Calculation Type";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum/2}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="description" datavaluefield="value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="scopeModel.onRateTypeChanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>' +
                '<vr-columns colnum="{{ctrl.normalColNum/2}}"><vr-textbox ng-if="scopeModel.showFixedRateValue()" value="scopeModel.fixedRateValue" label="Fixed Rate Value" isrequired type="number" ></vr-textbox></vr-columns>';

        }


        function TypeSelector(ctrl, $scope, attrs) {


            var selectorAPI;
            var rateTypeSelectedPromise;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onRateTypeChanged = function (value) {
                    if (selectorAPI) {
                        if (value) {
                            if (rateTypeSelectedPromise != undefined)
                                rateTypeSelectedPromise = undefined;
                            else
                                $scope.scopeModel.fixedRateValue = undefined;
                        }
                    }
                };
                $scope.scopeModel.showFixedRateValue = function () {
                    if (ctrl.selectedvalues != undefined) {
                        if (ctrl.selectedvalues.value == WhS_Jazz_RateCalculationTypeEnum.PartialRate.value || ctrl.selectedvalues.value == WhS_Jazz_RateCalculationTypeEnum.FixedRate.value)
                            return true;
                    }
                    return false;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) { //payload is an object that has selectedids and filter
                    selectorAPI.clearDataSource();
                    var selectedIds;

                    if (payload != undefined && payload.AmountCalculation!=undefined) {
                        selectedIds = payload.AmountCalculation.RateCalculationType;
                        $scope.scopeModel.fixedRateValue = payload.AmountCalculation.FixedRateValue;
                    }
                    ctrl.datasource = UtilsService.getArrayEnum(WhS_Jazz_RateCalculationTypeEnum);
                    if (selectedIds != undefined) {
                        rateTypeSelectedPromise = UtilsService.createPromiseDeferred();
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);

                    }
                };

                api.getSelectedIds = function () {
                    return {
                        $type:"TOne.WhS.Jazz.Entities.AmountCalculation,TOne.WhS.Jazz.Entities",
                        RateCalculationType: VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl),
                        FixedRateValue: $scope.scopeModel.fixedRateValue
                    }
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);