(function (app) {

    'use strict';

    GaugeWidgetDefinition.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function GaugeWidgetDefinition(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var gaugeWidgetDefinitionEditor = new GaugeWidgetDefinitionEditor(ctrl, $scope, $attrs);
                gaugeWidgetDefinitionEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },

            templateUrl: function (element, attrs) {
                return '/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/History/Widgets/Gauge/Templates/GaugeWidgetDefinition.html';
            }
        };

        function GaugeWidgetDefinitionEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var analyticTableSelectorAPI;
            var analyticTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var measuresSelectorAPI;
            var measuresSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var analyticTableMeasureSelectedPromiseDeferred;

            var entity;

            var analyticTableDimensions = [];

            var context = {};

            var tableIds;

            var filterObject;

            var measureId;

            var configId;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedMeasures = [];
                $scope.scopeModel.onMeasureSelectorReady = function (api) {
                    measuresSelectorAPI = api;
                    measuresSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var measures;
                    var promises = [];
                    var entity;
                    if (payload != undefined) {
                        tableIds = payload.tableIds;

                        if (tableIds != undefined && tableIds.length > 0) {
                            var getDimensionsPromise = VR_Analytic_AnalyticItemConfigAPIService.GetDimensions(tableIds[0]).then(function (response) {
                                if (response != undefined)
                                    for (var i = 0; i < response.length; i++) {
                                        var dimension = {
                                            FieldName: response[i].Name,
                                            FieldTitle: response[i].Title,
                                            Type: response[i].Config.FieldType,
                                        };
                                        analyticTableDimensions.push(dimension);
                                    }
                                context.getFields = function () {
                                    return analyticTableDimensions;
                                };
                                if (payload.widgetEntity != undefined) {
                                    if (payload.widgetEntity.Measures != undefined && payload.widgetEntity.Measures.length > 0) {
                                        measureId = payload.widgetEntity.Measures[0].MeasureName;
                                    }
                                    $scope.scopeModel.maxValue = payload.widgetEntity.Maximum;
                                    $scope.scopeModel.minValue = payload.widgetEntity.Minimum;
                                    $scope.scopeModel.autoRefresh = payload.widgetEntity.AutoRefresh;
                                    $scope.scopeModel.autoRefreshInterval = payload.widgetEntity.AutoRefreshInterval;
                                    configId = payload.configId;
                                }

                                var loadMeasureSelectorPromise = loadMeasureSelector();
                                promises.push(loadMeasureSelectorPromise);

                            });
                            promises.push(getDimensionsPromise);
                        }

                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticGaugeWidget,Vanrise.Analytic.MainExtensions",
                        AnalyticTableId: tableIds[0],
                        Measures: [getMeasure($scope.scopeModel.selectedMeasure)],
                        Maximum: $scope.scopeModel.maxValue,
                        Minimum: $scope.scopeModel.minValue,
                        AutoRefresh: $scope.scopeModel.autoRefresh,
                        AutoRefreshInterval: $scope.scopeModel.autoRefreshInterval,
                        ConfigId: configId,
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function loadMeasureSelector() {
                    var measureSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    measuresSelectorReadyPromiseDeferred.promise.then(function () {
                        var measureSelectorPayload = {
                            filter: {
                                TableIds: tableIds
                            }
                        };
                        if (measureId != undefined) {
                            measureSelectorPayload.selectedIds = measureId;
                        }
                        VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, measureSelectorPayload, measureSelectorLoadPromiseDeferred);
                    });
                    return measureSelectorLoadPromiseDeferred.promise;
                }
                function getMeasure(measure) {
                    return {
                        MeasureName: measure.Name,
                        Title: measure.Title,
                    };
                }

            }
        }
    }

    app.directive('vrAnalyticWidgetsGaugeDefinition', GaugeWidgetDefinition);

})(app);
