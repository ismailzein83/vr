"use strict";

app.directive("vrAnalyticGaugeRuntime", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericGauge = new GenericGauge($scope, ctrl, $attrs);
                genericGauge.initializeController();
            },
            controllerAs: 'analyticchartctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytic/Directives/Runtime/Chart/Templates/AnalyticGauge.html";
            }
        };

        function GenericGauge($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gaugeAPI;
            var gaugeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var chartData;
            var chartDefinition;
            var seriesDefinitions = [];
            var xAxisDefinition;
            var yAxisDefinition;

            var maximum;
            var minimum;
            var directiveAPI = {};
            var ranges;
            var measureValue;
            var measures;

            var definitionSettings;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGaugeReady = function (api) {
                    gaugeAPI = api;
                    gaugeReadyPromiseDeferred.resolve();
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(getDirectiveAPI());


                function getDirectiveAPI() {


                    var analyticTableId;

                    directiveAPI.load = function (payload) {
                        var promises = [];
                        if (payload != undefined) {
                            definitionSettings = payload.Settings;
                            measures = definitionSettings.Measures;
                            analyticTableId = payload.TableId;
                            var query = getQuery(payload);
                            var dataRetrievalInput = {
                                DataRetrievalResultType: 0,
                                IsSortDescending: false,
                                ResultKey: null,
                                SortByColumnName: 'MeasureValues.' + measures[0].MeasureName + '.Value',
                                Query: query
                            };
                           
                            var filteredRecordsPromise = VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                                .then(function (response) {
                                  if(response != undefined && response.Data != undefined && response.Data.length > 0) 
                                    measureValue = eval("response.Data[0].MeasureValues." + measures[0].MeasureName + ".ModifiedValue");
                                });
                            promises.push(filteredRecordsPromise);
                            var input = {
                                MeasureName: measures[0].MeasureName,
                                AnalyticTableId: analyticTableId
                            };
                            var measureStyleRuleRangesPromise = VR_Analytic_AnalyticAPIService.GetMeasureStyleRulesRanges(input).then(function (response) {
                                if (response != undefined) {
                                    ranges = response;
                                }
                            });
                            promises.push(measureStyleRuleRangesPromise);
                        }
                        return UtilsService.waitMultiplePromises(promises).then(function () {
                            gaugeReadyPromiseDeferred.promise.then(function () {
                                prepareDataForGaugeDirective();
                                gaugeAPI.renderChart({ Measure: measureValue }, chartDefinition, seriesDefinitions, xAxisDefinition, yAxisDefinition);
                            });
                        });
                    };
                    return directiveAPI;
                }

            };
            function prepareDataForGaugeDirective() {
                var gaugeRanges = [];
                chartDefinition = {
                    type: "gauge",
                    title: "Measure",
                    rangesObject : ranges
                };
                xAxisDefinition = {
                    titlePath: "Measure"
                };
                seriesDefinitions.push({
                    title: (measures != undefined && measures[0] != undefined)?measures[0].MeasureName:"",
                    valuePath: (measures != undefined && measures[0] != undefined) ? measures[0].MeasureName : "",
                    tooltip: {
                        valueSuffix: ''
                    },
                });
                yAxisDefinition = {
                    min: definitionSettings.Minimum,
                    max: definitionSettings.Maximum,
                    title: 'as',
                    interval: 20// Math.round(measureValue / 2)
                };

            }

            function getQuery(payload) {
                var queryFinalized = {
                    MeasureFields: UtilsService.getPropValuesFromArray(measures, 'MeasureName'),
                    FromTime: payload.FromTime,
                    ToTime: payload.ToTime,
                    TableId: payload.TableId,
                    FilterGroup: payload.FilterGroup,
                };
                return queryFinalized;
            }

        };
        return directiveDefinitionObject;
    }

]); 
