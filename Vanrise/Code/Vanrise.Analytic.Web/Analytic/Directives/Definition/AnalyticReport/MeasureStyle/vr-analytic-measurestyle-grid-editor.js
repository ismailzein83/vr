(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_AnalyticService','UtilsService'];

    function MeasureStyleGridEditorDirective(Analytic_AnalyticService, UtilsService) {
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
                        Analytic_AnalyticService.addMeasureStyle(onMeasureStyleAdded, ctrl.selectedMeasureName);
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
                        context = payload.context;
                        ctrl.measureStyles.length = 0;
                        if (payload.measureStyles && payload.measureStyles.length > 0) {
                            for (var y = 0; y < payload.measureStyles.length; y++) {
                                var currentMeasureStyle = payload.measureStyles[y];
                                ctrl.measureStyles.push(currentMeasureStyle);
                            }
                        }
                        ctrl.measureFields = getMeasureNames();
                    }
                };
                api.reloadMeasures = function () {

                    ctrl.measureFields = getMeasureNames();
                    var measureFields = context.getMeasures();
                    if (ctrl.measureStyles.length > 0) {
                        for (var i = 0; i < ctrl.measureStyles.length ; i++) {
                            var measureStyle = ctrl.measureStyles[i];
                            if (UtilsService.getItemByVal(measureFields, measureStyle.MeasureName, "Name") == undefined) {
                                ctrl.measureStyles.splice(ctrl.measureStyles.indexOf(measureStyle), 1);
                            }
                        }
                    }

                };
                api.getData = function () {
                    var measureStyles = [];
                    for (var i = 0; i < ctrl.measureStyles.length ; i++) {
                        var measureStyle = ctrl.measureStyles[i];
                        measureStyles.push({
                            MeasureName: measureStyle.MeasureName,
                            Rules: measureStyle.Rules
                        });
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
                Analytic_AnalyticService.editMeasureStyle(measureStyle, onMeasureStyleUpdated, selectedMeasure);
            }

            function getMeasureNames(measureStyle) {
                var measures = [];
                var measureFields = context.getMeasures();
                for (var x = 0; x < measureFields.length; x++) {
                    var currentMeasureField = measureFields[x];
                    if ((measureStyle != undefined && measureStyle.MeasureName == currentMeasureField.Name) || UtilsService.getItemByVal(ctrl.measureStyles, currentMeasureField.Name, "MeasureName") == undefined) {
                        measures.push(currentMeasureField);
                    }
                }
                return measures;
            }
        }
    }

    app.directive('vrAnalyticMeasurestyleGridEditor', MeasureStyleGridEditorDirective);

})(app);