"use strict";

app.directive("vrAnalyticGaugeRuntime", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService','VRTimerService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService, VRTimerService) {

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
            var query;
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
                             query = getQuery(payload);
                            if (definitionSettings.AutoRefresh) {
                                if ($scope.jobIds) {
                                    VRTimerService.unregisterJobByIds($scope.jobIds);
                                    $scope.jobIds.length = 0;
                                }
                            }
                            var filteredRecordsPromise = getFilteredRecords(query).then(function (response) {
                                measureValue = response;
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

                                if (definitionSettings.AutoRefresh)
                                    registerAutoRefreshJob(definitionSettings.AutoRefreshInterval);
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
                    rangesObject: ranges
                };
                xAxisDefinition = {
                    titlePath: "Measure"
                };
                seriesDefinitions.push({
                    title: (measures != undefined && measures[0] != undefined) ? measures[0].Title : "",
                    valuePath: (measures != undefined && measures[0] != undefined) ? measures[0].Title : "",
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

            function registerAutoRefreshJob(autoRefreshInterval) {
                VRTimerService.registerJob(onTimerElapsed, $scope, autoRefreshInterval);
            }
            function getQuery(payload) {
                var queryFinalized = {
                    MeasureFields: UtilsService.getPropValuesFromArray(measures, 'MeasureName'),
                    TimePeriod: payload.TimePeriod,
                    TableId: payload.TableId,
                    FilterGroup: payload.FilterGroup,
                    FromTime: payload.FromTime,
                    ToTime: payload.ToTime,
                };
                return queryFinalized;
            }

            function onTimerElapsed(){
               return getFilteredRecords(query).then(function (response) {
                    measureValue = response;
                    gaugeAPI.updateValue(measureValue);
                });
            }
            function getFilteredRecords(query) {
                var promiseDeferred = UtilsService.createPromiseDeferred();
                var dataRetrievalInput = {
                    DataRetrievalResultType: 0,
                    IsSortDescending: false,
                    ResultKey: null,
                    SortByColumnName: 'MeasureValues.' + measures[0].MeasureName + '.Value',
                    Query: query
                };
                 VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                    .then(function (response) {
                        if (response != undefined && response.Data != undefined && response.Data.length > 0) {
                            measureValue = eval("response.Data[0].MeasureValues." + measures[0].MeasureName + ".ModifiedValue");
                            promiseDeferred.resolve(measureValue);
                        }
                    });

                return promiseDeferred.promise;
            }

        };
        return directiveDefinitionObject;
    }

]); 
