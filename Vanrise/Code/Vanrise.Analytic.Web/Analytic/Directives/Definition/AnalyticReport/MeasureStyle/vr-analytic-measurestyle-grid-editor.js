(function (app) {

    'use strict';

    MeasureStyleGridEditorDirective.$inject = ['Analytic_AnalyticService', 'UtilsService', 'VR_Analytic_MeasureStyleRuleAPIService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function MeasureStyleGridEditorDirective(Analytic_AnalyticService, UtilsService, VR_Analytic_MeasureStyleRuleAPIService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
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

            var context = {};
            var gridAPI;
            var counter = 0;
            var analyticTableId;
            var statusDefinitionBeId;
            var recommendedId;
            var analyticTableMeasureStyles;
            var allMeasures = [];
            var isLoadedFromKpis = false;

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

                $scope.onClearClicked = function (dataItem) {
                    var newEmtyMeasure = {
                        MeasureName: dataItem.MeasureName,
                        MeasureTitle: dataItem.MeasureTitle,
                        Entity: {
                            MeasureStyleRuleDetail: {
                                MeasureTitle: dataItem.MeasureTitle,
                                MeasureDescription: undefined,
                                MeasureName: dataItem.MeasureName,
                                RecommendedRecordFilterDescription: undefined,
                                Rules: []
                            },
                            MeasureStyleRule: {
                                MeasureName: dataItem.MeasureName,
                                RecommendedStyleRule: undefined,
                                Rules: []
                            }
                        }
                    };
                    if (isLoadedFromKpis)
                        newEmtyMeasure.isKpiMeasureStyle = true;
                    ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = newEmtyMeasure;

                };
                ctrl.onMeasureClicked = function (dataItem) {
                    if (dataItem.Entity != undefined && (dataItem.Entity.MeasureStyleRule.Rules.length > 0 || dataItem.Entity.MeasureStyleRule.RecommendedStyleRule != undefined)) {
                        var isEditMode = true;
                        var onMeasureStyleUpdated = function (measureStyleObj, isLoadedFromKpis) {
                            ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = {
                                MeasureName: dataItem.MeasureName,
                                isKpiMeasureStyle: isLoadedFromKpis,
                                Entity: measureStyleObj,
                                MeasureTitle: dataItem.MeasureTitle,
                            };
                        };
                        var measureNames = getMeasureNames(dataItem);
                        var measureName = dataItem.Entity.MeasureStyleRuleDetail.MeasureName;
                        var selectedMeasure = UtilsService.getItemByVal(measureNames, dataItem.Entity.MeasureStyleRuleDetail.MeasureName, "Name");
                        Analytic_AnalyticService.editMeasureStyle(dataItem, onMeasureStyleUpdated, selectedMeasure, context, analyticTableId, isEditMode, measureName, statusDefinitionBeId, recommendedId, isLoadedFromKpis);
                    }
                    else {
                        var onMeasureStyleAdded = function (measureStyleObj, isLoadedFromKpis) {
                            ctrl.measureStyles[ctrl.measureStyles.indexOf(dataItem)] = {
                                MeasureName: dataItem.MeasureName,
                                isKpiMeasureStyle: isLoadedFromKpis,
                                Entity: measureStyleObj,
                                MeasureTitle: dataItem.MeasureTitle,
                            };
                        };
                        var measureName = dataItem.MeasureName;
                        Analytic_AnalyticService.addMeasureStyle(onMeasureStyleAdded, context, analyticTableId, measureName, statusDefinitionBeId, recommendedId, isLoadedFromKpis);
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var loadMeasureStylesGridPromiseDeferred = UtilsService.createPromiseDeferred();
                    var promises = [];
                    var measureStyleRuleEditorRuntime;
                    var measureStyleRuleEditorRuntimePromiseDeferred = UtilsService.createPromiseDeferred();

                    if (payload != undefined) {

                        isLoadedFromKpis = payload.isLoadedFromKpis;
                        analyticTableId = payload.analyticTableId;
                        var analyticTablePromiseDeferred = VR_Analytic_AnalyticTableAPIService.GetTableById(analyticTableId).then(function (response) {
                            if (response != undefined) {
                                statusDefinitionBeId = response.Settings.StatusDefinitionBEId;
                                recommendedId = response.Settings.RecommendedStatusDefinitionId;
                                if (response.MeasureStyles != null)
                                    analyticTableMeasureStyles = response.MeasureStyles.MeasureStyleRules;
                            }
                        });
                        promises.push(analyticTablePromiseDeferred);
                        var input = {
                            TableIds: [analyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                        };
                        var analyticItemConfigPromise = VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                            console.log(response);
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    var measureData = response[i];
                                    var measure = {
                                        FieldType: measureData.Config.FieldType,
                                        Name: measureData.Name,
                                        Title: measureData.Title,
                                        AnalyticItemConfigId: measureData.AnalyticItemConfigId
                                    };
                                    allMeasures.push(measure);
                                };
                            }
                        });
                        promises.push(analyticItemConfigPromise);
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            var measureNames = allMeasures;
                            context.getMeasure = function (name) {
                                console.log(name);
                                var measureFields = allMeasures;
                                console.log(measureFields);
                                var measure = UtilsService.getItemByVal(measureFields, name, "Name");
                                console.log(measure);
                                return measure;
                            };
                            ctrl.measureFields = getMeasureNames();

                            ctrl.descriptons = [];
                            var filter;
                            if (payload.measureStyles != undefined && payload.measureStyles.length > 0) {
                                var measureStyleRules = [];

                                for (var i = 0; i < payload.measureStyles.length; i++) {
                                    var measureStyle = payload.measureStyles[i];
                                    measureStyleRules.push(measureStyle);
                                }
                                if (analyticTableMeasureStyles != undefined && analyticTableMeasureStyles.length > 0) {
                                    for (var i = 0; i < analyticTableMeasureStyles.length; i++) {
                                        var measureRule = analyticTableMeasureStyles[i];
                                        if (UtilsService.getItemByVal(measureStyleRules, measureRule.MeasureName, 'MeasureName') == null) {
                                            measureStyleRules.push(measureRule);
                                        }
                                    }
                                }

                                var filter = {
                                    AnalyticTableId: analyticTableId,
                                    MeasureStyleRules: measureStyleRules
                                };
                                ctrl.measureStyles.length = 0;

                            }
                            else {
                                filter = {
                                    AnalyticTableId: analyticTableId,
                                    MeasureStyleRules: analyticTableMeasureStyles
                                };
                            }
                            VR_Analytic_MeasureStyleRuleAPIService.GetMeasureStyleRuleEditorRuntime(filter).then(function (response) {
                                if (response != undefined && response.MeasureStyleRulesRuntime != undefined) {
                                    measureStyleRuleEditorRuntime = response;
                                }
                                measureStyleRuleEditorRuntimePromiseDeferred.resolve();
                            });


                            measureStyleRuleEditorRuntimePromiseDeferred.promise.then(function () {
                                for (var i = 0; i < measureNames.length; i++) {
                                    var isKpiMeasureStyle = false;
                                    var measure = measureNames[i];
                                    var detail = undefined;
                                    if (measureStyleRuleEditorRuntime != undefined) {
                                        detail = UtilsService.getItemByVal(measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime, measureNames[i].Name, "MeasureStyleRule.MeasureName");
                                    }
                                    if (measureStyleRules != undefined && measureStyleRules.length > 0 && UtilsService.getItemByVal(measureStyleRules, measure.Name, "MeasureName") != undefined)
                                        isKpiMeasureStyle = true;


                                    ctrl.measureStyles.push({
                                        MeasureName: measure.Name,
                                        MeasureTitle: measure.Title,
                                        isKpiMeasureStyle: isKpiMeasureStyle,
                                        Entity: detail
                                    });
                                }
                                loadMeasureStylesGridPromiseDeferred.resolve();
                            });
                        });

                    }
                    return loadMeasureStylesGridPromiseDeferred.promise;
                };
                //api.reloadMeasures = function () {
                //    ctrl.measureFields = getMeasureNames();
                //    var measureFields = allMeasures;
                //    if (ctrl.measureStyles.length > 0) {
                //        for (var i = 0; i < ctrl.measureStyles.length; i++) {
                //            var measureStyle = ctrl.measureStyles[i];
                //            if (UtilsService.getItemByVal(measureFields, measureStyle.MeasureStyleRule.MeasureName, "Name") == undefined) {
                //                ctrl.measureStyles.splice(ctrl.measureStyles.indexOf(measureStyle), 1);
                //            }
                //        }
                //    }
                //};
                api.getData = function () {
                    var measureStyles = [];
                    if (isLoadedFromKpis) {
                        for (var i = 0; i < ctrl.measureStyles.length; i++) {
                            var measureStyle = ctrl.measureStyles[i];
                            if (measureStyle.Entity != undefined && measureStyle.isKpiMeasureStyle) {
                                measureStyles.push(measureStyle.Entity.MeasureStyleRule);
                            }
                        }
                        return measureStyles;
                    }
                    else {
                        for (var i = 0; i < ctrl.measureStyles.length; i++) {
                            var measureStyle = ctrl.measureStyles[i];
                            if (measureStyle.Entity != undefined) {
                                measureStyles.push(measureStyle.Entity.MeasureStyleRule);
                            }
                        }
                        return measureStyles;
                    }

                };
                return api;
            }

            function getMeasureNames(measureStyle) {
                var measures = [];
                for (var x = 0; x < allMeasures.length; x++) {
                    var currentMeasureField = allMeasures[x];
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