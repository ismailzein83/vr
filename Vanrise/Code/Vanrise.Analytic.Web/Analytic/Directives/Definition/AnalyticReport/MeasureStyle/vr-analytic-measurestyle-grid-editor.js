(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_AnalyticService', 'UtilsService', 'VR_Analytic_MeasureStyleRuleAPIService'];

    function MeasureStyleGridEditorDirective(Analytic_AnalyticService, UtilsService, VR_Analytic_MeasureStyleRuleAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var measureStyleGridEditor = new MeasureStyleGridEditor($scope, ctrl, $attrs);
                measureStyleGridEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/MeasureStyle/Templates/MeasureStyleGridEditorTemplate.html"
        };

        function MeasureStyleGridEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var gridAPI;
            var counter = 0;
            var analyticTableId;
            function initializeController() {
                ctrl.measureStyles = [];
                ctrl.measureFields = [];
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addMeasureStyle = function () {
                    if (ctrl.selectedMeasureName != undefined) {
                        var onMeasureStyleAdded = function (measureStyleObj) {
                            ctrl.measureStyles.push(measureStyleObj);
                            ctrl.measureFields = getMeasureNames();
                            ctrl.selectedMeasureName = undefined;
                        };
                        Analytic_AnalyticService.addMeasureStyle(onMeasureStyleAdded, ctrl.selectedMeasureName, context,analyticTableId);
                    }

                };

                ctrl.removeMeasureStyle = function (measureStyle) {
                    var measures = getMeasureNames();
                    ctrl.measureFields = getMeasureNames(measureStyle);
                    ctrl.measureStyles.splice(ctrl.measureStyles.indexOf(measureStyle), 1);
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        analyticTableId = payload.analyticTableId;
                        context = payload.context;
                        context.getMeasure = function (name) {
                            var measureFields = context.getMeasures();
                            var measure = UtilsService.getItemByVal(measureFields, name, "Name");
                            return measure;
                        };
                        ctrl.measureFields = getMeasureNames();
                        ctrl.descriptons = [];
                        if (payload.measureStyles != undefined && payload.measureStyles.length > 0) {
                            var filter = {
                                AnalyticTableId: analyticTableId,
                                MeasureStyleRules: payload.measureStyles
                            };
                            ctrl.measureStyles.length = 0;
                            return VR_Analytic_MeasureStyleRuleAPIService.GetMeasureStyleRuleEditorRuntime(filter).then(function (response) {
                                if (response != undefined && response.MeasureStyleRulesRuntime != undefined) {
                                    ctrl.measureStyles = response.MeasureStyleRulesRuntime;
                                }

                            });
                        }
                    }


                };
                api.reloadMeasures = function () {

                    ctrl.measureFields = getMeasureNames();
                    var measureFields = context.getMeasures();
                    if (ctrl.measureStyles.length > 0) {
                        for (var i = 0; i < ctrl.measureStyles.length; i++) {
                            var measureStyle = ctrl.measureStyles[i];
                            if (UtilsService.getItemByVal(measureFields, measureStyle.MeasureStyleRule.MeasureName, "Name") == undefined) {
                                ctrl.measureStyles.splice(ctrl.measureStyles.indexOf(measureStyle), 1);
                            }
                        }
                    }

                };
                api.getData = function () {
                    var measureStyles = [];
                    for (var i = 0; i < ctrl.measureStyles.length; i++) {
                        var measureStyle = ctrl.measureStyles[i];
                        measureStyles.push(measureStyle.MeasureStyleRule);
                    }
                    return measureStyles;
                };
                return api;
            }

            function defineMenuActions() {
                ctrl.measureStylesGridMenuActions = [{
                    name: 'Edit',
                    clicked: editMeasureStyle
                }];
            }

            function editMeasureStyle(measureStyle) {
                var onMeasureStyleUpdated = function (measureStyleObj) {
                    ctrl.measureStyles[ctrl.measureStyles.indexOf(measureStyle)] = measureStyleObj;
                };
                var selectedMeasure = UtilsService.getItemByVal(getMeasureNames(measureStyle), measureStyle.MeasureName, "Name");
                Analytic_AnalyticService.editMeasureStyle(measureStyle.MeasureStyleRule, onMeasureStyleUpdated, selectedMeasure, context, analyticTableId);
            }

            function getMeasureNames(measureStyle) {
                var measures = [];
                var measureFields = context.getMeasures();
                for (var x = 0; x < measureFields.length; x++) {
                    var currentMeasureField = measureFields[x];
                    if ((measureStyle != undefined && measureStyle.MeasureStyleRule.MeasureName == currentMeasureField.Name) || UtilsService.getItemByVal(ctrl.measureStyles, currentMeasureField.Name, "MeasureStyleRule.MeasureName") == undefined) {
                        measures.push(currentMeasureField);
                    }
                }
                return measures;
            }


        }
    }

    app.directive('vrAnalyticMeasurestyleGridEditor', MeasureStyleGridEditorDirective);

})(app);