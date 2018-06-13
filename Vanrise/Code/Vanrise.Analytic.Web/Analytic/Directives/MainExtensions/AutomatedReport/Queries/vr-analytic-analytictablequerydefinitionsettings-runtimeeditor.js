"use strict";
app.directive("vrAnalyticAnalytictablequerydefinitionsettingsRuntimeeditor", ["VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService", "UtilsService", "VRUIUtilsService",
function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new RuntimeEditor($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: "ctrlrutnime",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticTableQueryDefinitionSettingsRuntimeEditorTemplate.html"
    };

    function RuntimeEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dimensionsSelectorAPI;
        var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var measuresSelectorAPI;
        var measuresSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;
                timePeriodSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                dimensionsSelectorAPI = api;
                dimensionsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMeasureSelectorReady = function (api) {
                measuresSelectorAPI = api;
                measuresSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {

                }
                
                var loadTimePeriodSelectorPromise = loadTimePeriodSelector();
                var loadDimensionsSelectorPromise = loadDimensionsSelector();
                var loadMeasuresSelectorPromise = loadMeasuresSelector();

                promises.push(loadTimePeriodSelectorPromise);
                promises.push(loadDimensionsSelectorPromise);
                promises.push(loadMeasuresSelectorPromise);


                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodSelectorPayload = {
                            timePeriod: entity != undefined ? entity.TimePeriod : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodSelectorPayload, timePeriodSelectorLoadDeferred);
                    });
                    return timePeriodSelectorLoadDeferred.promise;
                }

                function loadDimensionsSelector() {
                    var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dimensionsSelectorReadyDeferred.promise.then(function () {
                        var dimensionsSelectorPayload = {
                            filter: 
                                {
                                    TableIds: analyticTableId
                                }
                            ,
                            selectedIds: undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, dimensionsSelectorPayload, dimensionsSelectorLoadDeferred);
                    });
                    return dimensionsSelectorLoadDeferred.promise;
                }

                function loadMeasuresSelector() {
                    var measuresSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    measuresSelectorReadyDeferred.promise.then(function () {
                        var measuresSelectorPayload = {
                            filter:
                                {
                                    TableIds: analyticTableId
                                }
                            ,
                            selectedIds: undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, measuresSelectorPayload, measuresSelectorLoadDeferred);
                    });
                    return measuresSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                return {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.AnalyticTableQuerySettings,Vanrise.Analytic.MainExtensions',
                    TimePeriod: timePeriodSelectorAPI.getData(),
                    Dimensions: getDimensions(),
                    Measures: getMeasures()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {
                };
            return currentContext;
        }

        function getDimensions() {
            if (dimensionsSelectorAPI != undefined && dimensionsSelectorAPI.getSelectedIds != undefined && typeof (dimensionsSelectorAPI.getSelectedIds) == "function") {
                var dimensionNames = dimensionsSelectorAPI.getSelectedIds();
                var dimensions = [];
                for (var i = 0; i < dimensionNames.length; i++) {
                    dimensions.push({ DimensionName: dimensionNames[i] });
                }
                return dimensions;
            }
        }

        function getMeasures() {
            if (measuresSelectorAPI != undefined && measuresSelectorAPI.getSelectedIds != undefined && typeof (measuresSelectorAPI.getSelectedIds) == "function") {
                var measureNames = measuresSelectorAPI.getSelectedIds();
                var measures = [];
                for (var i = 0; i < measureNames.length; i++) {
                    measures.push({ MeasureName: measureNames[i] });
                }
                return measures;
            }
        }
    }

    return directiveDefinitionObject;
}
]);