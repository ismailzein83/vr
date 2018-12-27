(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_AnalyticService', 'UtilsService', 'VR_Analytic_MeasureStyleRuleAPIService', 'VR_Analytic_AnalyticTableAPIService'];

    function MeasureStyleGridEditorDirective(Analytic_AnalyticService, UtilsService, VR_Analytic_MeasureStyleRuleAPIService, VR_Analytic_AnalyticTableAPIService) {
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
            var statusDefinitionBeId;
            var recommendedId;

            function initializeController() {
                ctrl.measureStyles = [];
                ctrl.measureFields = [];
                ctrl.showRecommendedColumn = function () {
                    return recommendedId != undefined;
                };
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };



                $scope.onResetClicked = function (dataItem) {
                    ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = {
                        MeasureName: dataItem.MeasureName
                    };
                };
                ctrl.saveMeasureStyle = function (dataItem) {
                   
                        if (dataItem.Entity != undefined) {
                            var isEditMode = true;
                            var onMeasureStyleUpdated = function (measureStyleObj) {
                                ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = {
                                    MeasureName: dataItem.MeasureName,
                                    Entity: measureStyleObj
                                };
                            };
                            var measureNames = getMeasureNames(dataItem);
                            var measureName = dataItem.Entity.MeasureStyleRuleDetail.MeasureName;
                            var selectedMeasure = UtilsService.getItemByVal(measureNames, dataItem.Entity.MeasureStyleRuleDetail.MeasureName, "Name");
                            Analytic_AnalyticService.editMeasureStyle(dataItem, onMeasureStyleUpdated, selectedMeasure, context, analyticTableId, isEditMode, measureName, statusDefinitionBeId, recommendedId);
                        }
                        else {
                            var onMeasureStyleAdded = function (measureStyleObj) {
                                ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = {
                                    MeasureName: dataItem.MeasureName,
                                    Entity: measureStyleObj
                                };
                            };
                            var measureName = dataItem.MeasureName;
                            Analytic_AnalyticService.addMeasureStyle(onMeasureStyleAdded, context, analyticTableId, measureName, statusDefinitionBeId, recommendedId);
                        }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var measureStyleRuleEditorRuntime;
                    var measureStyleRuleEditorRuntimePromiseDeferred = UtilsService.createPromiseDeferred();
                    if (payload != undefined) {
                        statusDefinitionBeId = payload.statusDefinitionBeId;
                        recommendedId = payload.recommendedId;
                        analyticTableId = payload.analyticTableId;
                        context = payload.context;
                        var measureNames = context.getMeasures();
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
                            VR_Analytic_MeasureStyleRuleAPIService.GetMeasureStyleRuleEditorRuntime(filter).then(function (response) {
                                if (response != undefined && response.MeasureStyleRulesRuntime != undefined) {
                                    measureStyleRuleEditorRuntime = response;
                                    measureStyleRuleEditorRuntimePromiseDeferred.resolve();

                                }

                            });
                        }
                        else
                            measureStyleRuleEditorRuntimePromiseDeferred.resolve();
                        measureStyleRuleEditorRuntimePromiseDeferred.promise.then(function () {
                            for (var i = 0; i < measureNames.length; i++) {
                                var measure = measureNames[i];
                                var detail;
                                if (measureStyleRuleEditorRuntime != undefined) {
                                    detail = UtilsService.getItemByVal(measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime, measureNames[i].Name, "MeasureStyleRule.MeasureName");
                                }
                                if (detail != undefined) {
                                    ctrl.measureStyles.push({
                                        MeasureName: measure.Name,
                                        Entity: detail
                                    });
                                } else {
                                    ctrl.measureStyles.push({
                                        MeasureName: measure.Name
                                    });
                                }
                            }
                        });
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
                        if (measureStyle.Entity != undefined) {
                            measureStyles.push(measureStyle.Entity.MeasureStyleRule);

                        }
                    }
                    return measureStyles;
                };
                return api;
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