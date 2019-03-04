
appControllers.directive('whsJazzRateCalculationTypeSelector', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Jazz_AmountTypeEnum',
    function (VRNotificationService, UtilsService, VRUIUtilsService, WhS_Jazz_AmountTypeEnum) {
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
            var label = "Amount Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Amount Type";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="description" datavaluefield="value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged=ctrl.onselectionchanged onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';
              

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
                  
                };
           
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) { //payload is an object that has selectedids and filter
                    selectorAPI.clearDataSource();
                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    ctrl.datasource = UtilsService.getArrayEnum(WhS_Jazz_AmountTypeEnum);
                    if (selectedIds != undefined) {
                        rateTypeSelectedPromise = UtilsService.createPromiseDeferred();
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);

                    }
                };

                api.getData = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);