﻿'use strict';
app.directive('cdranalysisPstnRuletypeSelector', ['PSTN_BE_NormalizationRuleTypeEnum', 'UtilsService', 'VRUIUtilsService',
    function (PSTN_BE_NormalizationRuleTypeEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

              
                var typeSelector = new TypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();


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
                return getTypeTemplate(attrs);
            }

        };

        function getTypeTemplate(attrs) {

            var multipleselection = "";
            var label = "Rule Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Rule Types";
                multipleselection = "ismultipleselection";
            }

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Rule Type" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }
        function TypeSelector(ctrl, $scope, attrs) {

            var selectorAPI;
            ctrl.datasource = UtilsService.getArrayEnum(PSTN_BE_NormalizationRuleTypeEnum);
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        
        return directiveDefinitionObject;
    }]);

