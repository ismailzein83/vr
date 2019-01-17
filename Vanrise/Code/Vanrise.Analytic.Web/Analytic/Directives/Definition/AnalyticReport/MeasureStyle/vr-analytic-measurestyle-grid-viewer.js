(function (app) {

    'use strict';

    MeasureStyleGridViewerDirective.$inject = ['Analytic_AnalyticService', 'UtilsService', 'VR_Analytic_MeasureStyleRuleAPIService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function MeasureStyleGridViewerDirective(Analytic_AnalyticService, UtilsService, VR_Analytic_MeasureStyleRuleAPIService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var measureStyleGridViewer = new MeasureStyleGridViewer($scope, ctrl, $attrs);
                measureStyleGridViewer.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/MeasureStyle/Templates/MeasureStyleGridViewerTemplate.html"
        };

        function MeasureStyleGridViewer($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var gridAPI;
            var analyticTableId;
            var measureStyleRuleEditorRuntime;
            var measures = [];
            var highlightedId;
            var statusBeDefinitionId;

            function initializeController() {
                ctrl.mergedMeasureStyles = [];
                ctrl.showRecommended = false;
                ctrl.showRules = true;
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        analyticTableId = payload.analyticTableId;
                        highlightedId = payload.highlightedId;
                        ctrl.showRecommended = highlightedId != undefined;
                        statusBeDefinitionId = payload.statusBeDefinitionId;
                    }
                    if (analyticTableId != undefined) {

                        var input = {
                            TableIds: [analyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                        };
                        var analyticItemConfigPromise = VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    var measureData = response[i];
                                    var measure = {
                                        FieldType: measureData.Config.FieldType,
                                        Name: measureData.Name,
                                        Title: measureData.Title,
                                        AnalyticItemConfigId: measureData.AnalyticItemConfigId,
                                        Config : measureData.Config
                                    };
                                    measures.push(measure);
                                };
                            }
                        });
                        promises.push(analyticItemConfigPromise);

                        var analyticTbaleMergedMeasureStyleRules = VR_Analytic_AnalyticTableAPIService.GetAnalyticTableMergedMeasureStylesEditorRuntime(analyticTableId).then(function (response) {
                            if (response != undefined) {
                                measureStyleRuleEditorRuntime = response;
                            }
                            if (response == undefined || response.MeasureStyleRulesRuntime == undefined || response.MeasureStyleRulesRuntime.length == 0) {
                                
                                ctrl.showRules = false;
                                ctrl.showRecommended = false;
                            }
                            
                        });
                        promises.push(analyticTbaleMergedMeasureStyleRules);
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        for (var i = 0; i < measures.length; i++) {
                            var measure = measures[i];
                            var detail = undefined;
                            if (measureStyleRuleEditorRuntime != undefined && measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime != undefined) {
                                detail = UtilsService.getItemByVal(measureStyleRuleEditorRuntime.MeasureStyleRulesRuntime, measures[i].Name, "MeasureStyleRule.MeasureName");
                            }
                            if (measure.Config != undefined && !measure.Config.HideDescriptionInLegend) {
                                ctrl.mergedMeasureStyles.push({
                                    MeasureName: measure.Title,
                                    MeasureDescription: measure.Config.Description,
                                    Entity: detail
                                });
                                
                            } 
                        }
                    });
                };
                
                api.getData = function () {
                   
                };
                return api;
            }

            
        }
    }

    app.directive('vrAnalyticMeasurestyleGridViewer', MeasureStyleGridViewerDirective);

})(app);