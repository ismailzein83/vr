"use strict";
app.directive("vrAnalyticMeasuremappingrulesGrid", ["UtilsService", "VRNotificationService", "VR_Analytic_MeasureMappingRulesService",
    function (UtilsService, VRNotificationService, VR_Analytic_MeasureMappingRulesService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",

            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var rulesGrid = new RulesGrid($scope, ctrl, $attrs);
                rulesGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/AnalyticMeasureExternalSource/Templates/AnalyticMeasureMappingRulesGridTemplate.html"
        };
        function RulesGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            var tableId;

            this.initializeController = initializeController;

            function initializeController() {

                ctrl.datasource = [];

                $scope.isDisabled = function () {
                    if (context != undefined && context.getAnalyticTableId != undefined) {
                        return false;
                    }
                    else
                        return true;
                };

                ctrl.addRule = function () {
                    var onRuleAdded = function (rule) {
                        ctrl.datasource.push({ Entity: rule });
                    };
                    VR_Analytic_MeasureMappingRulesService.addRule(onRuleAdded, { context: getContext(), tableId: tableId});
                };
                ctrl.removeRule = function (rule) {
                    var index = ctrl.datasource.indexOf(rule);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editRule
                }];

                $scope.gridMenuActions = function (rule) {
                    return defaultMenuActions;
                };
            }
            function editRule(ruleObject) {
                var onRuleUpdated = function (rule) {
                    var index = ctrl.datasource.indexOf(ruleObject);
                    ctrl.datasource[index] = { Entity: rule };
                };
                VR_Analytic_MeasureMappingRulesService.editRule(ruleObject, onRuleUpdated, { context: getContext(), tableId: tableId});
            }
            function defineAPI() {
                var api = {};
                api.getData = function () {
                    var rules;
                    if (ctrl.datasource != undefined && ctrl.datasource != null) {
                        rules = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            rules.push(currentItem.Entity);
                        }
                    }
                    return rules;
                };
                api.load = function (payload) {
                    ctrl.datasource.length = 0;

                    if (payload != undefined) {
                        context = payload.context;
                        tableId = payload.tableId;

                        if (payload.rules != undefined) {
                            var rules = payload.rules;
                            for (var i = 0; i < rules.length; i++) {
                                var rule = rules[i];
                                ctrl.datasource.push({ Entity: rule });
                            }
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function getContext() {

                var currentContext = context;

                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }
        }
        return directiveDefinitionObject;
    }]);