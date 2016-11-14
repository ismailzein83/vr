(function (app) {

    'use strict';

    WidgetsChartDefinition.$inject = ["UtilsService", 'VRUIUtilsService','VR_ChartDefinitionTypeEnum'];

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
                $scope.scopeModel.chartTypes = [];
                $scope.scopeModel.selectedChartType;
                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureReadyDeferred.resolve();
                };
                loadChartTypes();
                defineAPI();
            }
            function loadChartTypes() {
                $scope.scopeModel.chartTypes = [];
                for (var m in VR_ChartDefinitionTypeEnum) {
                    $scope.scopeModel.chartTypes.push(VR_ChartDefinitionTypeEnum[m]);
                }
                $scope.scopeModel.selectedChartType = $scope.scopeModel.chartTypes[0];
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
 
                    if (payload != undefined && payload.tableIds != undefined) {
                        var tableIds = payload.tableIds;
                        var selectedMeasureIds;
                        if (payload.widgetEntity != undefined) {
                            $scope.scopeModel.selectedChartType = UtilsService.getItemByVal($scope.scopeModel.chartTypes, payload.widgetEntity.ChartType, "value");

                            selectedMeasureIds = [];
                            if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                for (var i = 0; i < payload.widgetEntity.Measures.length; i++) {
                                    var measure = payload.widgetEntity.Measures[i];
                                    selectedMeasureIds.push(measure.MeasureName);
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
                        ChartType: $scope.scopeModel.selectedChartType.value,
                    };
                    return data;
                }


                function getMeasures() {
                    var measures;
                    if (measureSelectorAPI != undefined && measureSelectorAPI.getSelectedIds() !=undefined) {
                        measures = [];
                        var selectedIds = measureSelectorAPI.getSelectedIds();
                        for (var i = 0; i < selectedIds.length; i++) {
                            var measure = selectedIds[i];
                            measures.push({ MeasureName: measure});
                        }
                    }
                    return measures;
                }


            }
        }
    }

    app.directive('vrAnalyticRealtimeWidgetsTimevariationDefinition', WidgetsChartDefinition);

})(app);