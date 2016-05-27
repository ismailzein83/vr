(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_AnalyticService'];

    function MeasureStyleGridEditorDirective(Analytic_AnalyticService) {
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
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addMeasureStyle = function () {
                    var onMeasureStyleAdded = function (measureStyleObj) {
                        counter++;
                        measureStyleObj.id = counter;
                        ctrl.measureStyles.push(measureStyleObj);
                    }
                    Analytic_AnalyticService.addMeasureStyle(onMeasureStyleAdded, getMeasureNames());
                }

                ctrl.removeMeasureStyle = function (measureStyle) {
                    ctrl.measureStyles.splice(ctrl.measureStyles.indexOf(measureStyle), 1);
                }

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                    {
                        context = payload.context;
                        ctrl.measureStyles.length = 0;
                        if (payload.measureStyles && payload.measureStyles.length > 0) {
                            for (var y = 0; y < payload.measureStyles.length; y++) {
                                counter++;
                                var currentMeasureStyle = payload.measureStyles[y];
                                currentMeasureStyle.id = counter;
                                ctrl.measureStyles.push(currentMeasureStyle);
                            }
                        }
                    }
                };

                api.getData = function () {
                    return ctrl.measureStyles;
                }
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
                    for (var x = 0; x < ctrl.measureStyles.length; x++) {
                        var currentMeasureStyle = ctrl.measureStyles[x];
                        if (currentMeasureStyle.id == measureStyle.id) {
                            ctrl.measureStyles.splice(x, 1);
                            measureStyleObj.id = measureStyle.id;
                            ctrl.measureStyles.push(measureStyleObj);
                            ctrl.measureStyles.sort(function (a, b) {
                                return b.id - a.id;
                            });
                            break;
                        }
                    }
                }

                Analytic_AnalyticService.editMeasureStyle(measureStyle,  onMeasureStyleUpdated,getMeasureNames(measureStyle));
            }

            function getMeasureNames(measureStyle) {
                var measures = [];
                var measureFields = context.getMeasures();
                for (var x = 0; x < measureFields.length; x++) {
                    var currentMeasureField = measureFields[x];
                    if (measureStyle == undefined || measureStyle.Name != currentMeasureField.Name) {
                        measures.push(currentMeasureField);
                    }
                }
                return measures;
            }
        }
    }

    app.directive('vrAnalyticMeasurestyleGridEditor', MeasureStyleGridEditorDirective);

})(app);