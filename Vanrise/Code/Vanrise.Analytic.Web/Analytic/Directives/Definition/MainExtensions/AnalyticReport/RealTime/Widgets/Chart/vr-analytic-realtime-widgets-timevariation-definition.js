(function (app) {

    'use strict';

    WidgetsChartDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_ChartDefinitionTypeEnum'];

    function WidgetsChartDefinition(UtilsService, VRUIUtilsService, VR_ChartDefinitionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var widgetsChart = new WidgetsChart($scope, ctrl, $attrs);
                widgetsChart.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/RealTime/Widgets/Chart/Templates/TimeVariationWidgetDefinitionTemplate.html"

        };
        function WidgetsChart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var measureSelectorAPI;
            var measureReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.measures = [];

                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
 
                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        var selectedMeasureIds;
                        if (payload.widgetEntity != undefined) {
                            selectedMeasureIds = [];
                            if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Measures.length; i++) {
                                    var measure = payload.widgetEntity.Measures[i];
                                    selectedMeasureIds.push(measure.MeasureName);
                                    $scope.scopeModel.measures.push({
                                        Name: measure.MeasureName,
                                        Title: measure.Title,
                                    });
                                }
                            }
                        }
                        var promises = [];

                        var loadMeasureDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        measureReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds },
                                selectedIds: selectedMeasureIds
                            };

                            VRUIUtilsService.callDirectiveLoad(measureSelectorAPI, payloadFilterDirective, loadMeasureDirectivePromiseDeferred);
                        });
                        promises.push(loadMeasureDirectivePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }


                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.RealTimeReport.Widgets.TimeVariationChartWidget, Vanrise.Analytic.MainExtensions ",
                        Measures: getMeasures(),
                    }
                    return data;
                }


                function getMeasures() {
                    var measures;
                    if ($scope.scopeModel.measures != undefined && $scope.scopeModel.measures.length > 0) {
                        measures = [];
                        for (var i = 0; i < $scope.scopeModel.measures.length; i++) {
                            var measure = $scope.scopeModel.measures[i];
                            measures.push({
                                MeasureName: measure.Name,
                                Title: measure.Title,
                            });
                        }
                    }
                    console.log(measures);
                    return measures;
                }
            }
        }
    }

    app.directive('vrAnalyticRealtimeWidgetsTimevariationDefinition', WidgetsChartDefinition);

})(app);