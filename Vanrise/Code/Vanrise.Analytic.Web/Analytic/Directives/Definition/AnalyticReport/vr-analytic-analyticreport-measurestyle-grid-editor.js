(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_RecordSearchService'];

    function MeasureStyleGridEditorDirective(Analytic_RecordSearchService) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/Templates/MeasureStyleGridEditorTemplate.html"
        };

        function MeasureStyleGridEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var measureFields;
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
                    Analytic_RecordSearchService.addDataRecordMeasureStyle(onMeasureStyleAdded, getMeasureNames(null));
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
                        measureFields = payload.measureFields;
                    }
                    ctrl.measureStyles.length = 0;
                    if (query) {
                        if (query.measureStyles && query.measureStyles.length > 0) {
                            for (var y = 0; y < query.measureStyles.length; y++) {
                                counter++;
                                var currentMeasureStyle = query.measureStyles[y];
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

                Analytic_RecordSearchService.editDataRecordMeasureStyle(measureStyle, getMeasureNames(measureStyle), onMeasureStyleUpdated);
            }

            function getMeasureNames(measureStyle) {
                var measureNames = [];
                for (var x = 0; x < measureFields.length; x++) {
                    var currentMeasureField = measureFields[x];
                    if (measureStyle == null || measureStyle.Title != currentMeasureField.Title) {
                        measureNames.push(currentMeasureField.Title.toLowerCase());
                    }
                }
                return measureNames;
            }
        }
    }

    app.directive('vrAnalyticAnalyticreportMeasurestyleGridEditor', MeasureStyleGridEditorDirective);

})(app);