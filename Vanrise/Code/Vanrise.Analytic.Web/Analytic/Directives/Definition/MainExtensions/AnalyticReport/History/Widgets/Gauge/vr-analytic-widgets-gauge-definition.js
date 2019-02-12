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

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var analyticTableMeasureSelectedPromiseDeferred;
            var analyticTableRecordFilterSelectedPromiseDeferred;

            var entity;

            var analyticTableDimensions = [];

            var context = {};

            var analyitcTableId;

            var filterObject;

            var measureNames;

            var configId;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedMeasures = [];
                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    analyticTableSelectorAPI = api;
                    analyticTableReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onMeasureSelectorReady = function (api) {
                    measuresSelectorAPI = api;
                    measuresSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onTableSelectionChanged = function () {
                    var selectedAnalyticTable = analyticTableSelectorAPI.getSelectedIds();
                    if (selectedAnalyticTable != undefined) {
                        VR_Analytic_AnalyticItemConfigAPIService.GetDimensions(selectedAnalyticTable).then(function (response) {
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
                            var setMeasuresLoader = function (value) { $scope.isLoadingMeasures = value; };
                            var measuresSelectorPayload = {
                                filter: {
                                    TableIds: [analyticTableSelectorAPI.getSelectedIds()]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, measuresSelectorAPI, measuresSelectorPayload, setMeasuresLoader, analyticTableMeasureSelectedPromiseDeferred);

                            var setRecordFilterLoader = function (value) { $scope.isLoadingRecordFilter = value; };
                            var recordFilterPayload = {
                                context: context
                                //filterObj: filterObject
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterPayload, setRecordFilterLoader, analyticTableRecordFilterSelectedPromiseDeferred);

                        });

                    }
                    else {
                        if (measuresSelectorAPI != undefined) {
                            measuresSelectorAPI.clearDataSource();
                            $scope.scopeModel.selectedMeasure = undefined;
                        }
                        if (recordFilterDirectiveAPI != undefined) {
                            //should implement something like cleardatasoutrce;
                        }
                    }
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

                        analyitcTableId = payload.analyticTableId;
                        filterObject = payload.filterObj;
                        var measureIds = payload.measures;
                        configId = payload.configId;
                    }
                    var loadAnalyitcTableSectionPromise = loadAnalyticTableSectionSection();
                    promises.push(loadAnalyitcTableSectionPromise);

                    var loadPeriodSelectorPromise = loadPeriodSelector();

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.History.Widgets.AnalyticGaugeWidget,Vanrise.Analytic.MainExtensions",
                        AnalyticTableId: analyticTableSelectorAPI.getSelectedIds(),
                        Measures: [measuresSelectorAPI.getSelectedIds()],
                        RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                        ConfigId: configId
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function loadAnalyticTableSectionSection() {
                    var loadAnalyticTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    var promises = [];
                    promises.push(loadAnalyticTableSelectorPromiseDeferred.promise);

                    var analyticTableSelectorPayload = {

                    };

                    if (analyitcTableId != undefined) {
                        analyticTableSelectorPayload.selectedIds = analyitcTableId;

                        analyticTableMeasureSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        analyticTableRecordFilterSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    }

                    analyticTableReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(analyticTableSelectorAPI, analyticTableSelectorPayload, loadAnalyticTableSelectorPromiseDeferred);
                    });



                    if (analyitcTableId != undefined) {
                        var loadMeasureSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadMeasureSelectorPromiseDeferred.promise);

                        var loadRecordFilterPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadRecordFilterPromiseDeferred.promise);

                        UtilsService.waitMultiplePromises([measuresSelectorReadyPromiseDeferred.promise, recordFilterDirectiveReadyPromiseDeferred.promise, analyticTableMeasureSelectedPromiseDeferred.promise, analyticTableRecordFilterSelectedPromiseDeferred.promise]).then(function () {
                            var measuresSelectorPayload = {
                                filter: {
                                    TableIds: [analyticTableSelectorAPI.getSelectedIds()]
                                }
                            };
                            if (measureNames != undefined && measureNames.length > 0)
                                measuresSelectorPayload.selectedIds = measureNames;

                            VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, measuresSelectorPayload, loadMeasureSelectorPromiseDeferred);
                            analyticTableMeasureSelectedPromiseDeferred = undefined;
                            context.getFields = function () {
                                return analyticTableDimensions;
                            };
                            var recordFilterSelectorPayload =
                            {
                                context: context
                            };
                            if (filterObject != undefined)
                                recordFilterSelectorPayload.FilterGroup = filterObject;

                            VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterSelectorPayload, loadRecordFilterPromiseDeferred);
                            analyticTableRecordFilterSelectedPromiseDeferred = undefined;

                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                }

            }
        }
    }

    app.directive('vrAnalyticWidgetsGaugeDefinition', GaugeWidgetDefinition);

})(app);
