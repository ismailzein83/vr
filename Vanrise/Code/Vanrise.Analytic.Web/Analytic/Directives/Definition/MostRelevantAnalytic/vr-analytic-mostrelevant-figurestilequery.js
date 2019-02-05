(function (app) {

    'use strict';

    MostRelevantAnalyticFiguresTileQuery.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function MostRelevantAnalyticFiguresTileQuery(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mostRelevantAnalyticFiguresTileQuery = new MostRelevantAnalyticFiguresTileQuery(ctrl, $scope, $attrs);
                mostRelevantAnalyticFiguresTileQuery.initializeController();
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
                return '/Client/Modules/Analytic/Directives/Definition/MostRelevantAnalytic/Templates/MostRelevantFiguresTileQueryTemplate.html';
            }
        };

        function MostRelevantAnalyticFiguresTileQuery(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var analyticTableSelectorAPI;
            var analyticTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var measuresSelectorAPI;
            var measuresSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dimensionsSelectorAPI;
            var dimensionsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var advancedOrderOptionDirectiveAPI;
            var advancedOrderOptionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var periodSelectorAPI;
            var periodSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var analyticTableMeasureSelectedPromiseDeferred;
            var analyticTableDimesnionSelectedPromiseDeferred;
            var analyticTableRecordFilterSelectedPromiseDeferred;
            var advancedOrderOptionDirectiveSelectedPromiseDeferred;

            var entity;

            var analyticTableDimensions = [];

            var context = {};

            var analyitcTableId;

            var filterObject;

            var measureNames;

            var dimensionName;

            var timePeriod;

            var configId;

            var dimensionId;

            var advancedOrderOptions;


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

                $scope.scopeModel.orderOptionsDirectiveReady = function (api) {
                    advancedOrderOptionDirectiveAPI = api;
                    advancedOrderOptionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDimensionSelectorReady = function (api) {
                    dimensionsSelectorAPI = api;
                    dimensionsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onPeriodSelectorReady = function (api) {
                    periodSelectorAPI = api;
                    periodSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onMeasureSelectionChanged = function () {
                    var selectedMeasures = $scope.scopeModel.selectedMeasures;
                    if (selectedMeasures != undefined && selectedMeasures.length > 0) {
                        var onlySelectedMeasureIds = [];
                        for (var i = 0; i < $scope.scopeModel.selectedMeasures.length; i++) {
                            var selectedMeasure = $scope.scopeModel.selectedMeasures[i];
                            onlySelectedMeasureIds.push(selectedMeasure.AnalyticItemConfigId)
                        }

                        if (onlySelectedMeasureIds != undefined) {
                            var advancedDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            var setAdvancedOptionsLoader = function (value) { $scope.isLoadingAdvancedOptions = value; };
                            var advancedOrderOptionDirectivePayload = {
                                tableIds: [analyticTableSelectorAPI.getSelectedIds()],
                                onlySelectedMeasureIds: onlySelectedMeasureIds
                            };
                            if (advancedOrderOptions != undefined)
                                advancedOrderOptionDirectivePayload.advancedOrderOptions = advancedOrderOptions;
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, advancedOrderOptionDirectiveAPI, advancedOrderOptionDirectivePayload, setAdvancedOptionsLoader, advancedOrderOptionDirectiveSelectedPromiseDeferred);
                        }
                    }
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


                            var setDimensionsLoader = function (value) { $scope.isLoadingDimensions = value; };
                            var dimesionsSelectorPayload = {
                                filter: {
                                    TableIds: [analyticTableSelectorAPI.getSelectedIds()]
                                }
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dimensionsSelectorAPI, dimesionsSelectorPayload, setDimensionsLoader, analyticTableDimesnionSelectedPromiseDeferred);

                            var setRecordFilterLoader = function (value) { $scope.isLoadingRecordFilter = value; };
                            var recordFilterPayload = {
                                context: context,
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

                        if (measureIds != undefined) {
                            var measureNamesInput = {
                                AnalyticTableId: analyitcTableId,
                                MeasureIds: measureIds
                            };
                            var measureNamesPromiseDeferrred = VR_Analytic_AnalyticItemConfigAPIService.GetMeasuresNames(measureNamesInput).then(function (response) {
                                if (response != undefined)
                                    measureNames = response;
                            });
                            promises.push(measureNamesPromiseDeferrred);
                        }

                        dimensionId = payload.dimensionId;

                        if (dimensionId != undefined) {
                            var dimensionNameInput = {
                                AnalyticTableId: analyitcTableId,
                                DimensionId: dimensionId
                            };
                            var dimensionNamePromiseDeferrred = VR_Analytic_AnalyticItemConfigAPIService.GetDimensionName(dimensionNameInput).then(function (response) {
                                if (response != undefined)
                                    dimensionName = response;
                            });
                            promises.push(dimensionNamePromiseDeferrred);
                        }
                        advancedOrderOptions = payload.advancedOrderOptions;
                        timePeriod = payload.timePeriod;
                        configId = payload.configId;
                    }
                    var loadAnalyitcTableSectionPromise = loadAnalyticTableSectionSection();
                    promises.push(loadAnalyitcTableSectionPromise);

                    var loadPeriodSelectorPromise = loadPeriodSelector();
                    promises.push(loadPeriodSelectorPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var measuresIds = [];
                    var selectedMeasures = $scope.scopeModel.selectedMeasures;
                    for (var i = 0; i < selectedMeasures.length; i++) {
                        var measure = selectedMeasures[i];
                        measuresIds.push(measure.AnalyticItemConfigId);
                    }
                    var dimensionId;
                    var selectedDimension = $scope.scopeModel.selectedDimension;
                    dimensionId = selectedDimension.AnalyticItemConfigId;
                    return {
                        $type: "Vanrise.Analytic.Business.MostRelevantAnalyticDimensionFiguresTileQuery,Vanrise.Analytic.Business",
                        AnalyticTableId: analyticTableSelectorAPI.getSelectedIds(),
                        Measures: measuresIds,
                        TimePeriod: periodSelectorAPI.getData(),
                        RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                        ConfigId: configId,
                        AdvancedOrderOptions: advancedOrderOptionDirectiveAPI.getData(),
                        DimensionId: dimensionId
                    }
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
                        analyticTableDimesnionSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                       
                    }

                    analyticTableReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(analyticTableSelectorAPI, analyticTableSelectorPayload, loadAnalyticTableSelectorPromiseDeferred);
                    });



                    if (analyitcTableId != undefined) {
                        var loadMeasureSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadMeasureSelectorPromiseDeferred.promise);

                        var loadAdvamcedOrderOptionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadAdvamcedOrderOptionDirectivePromiseDeferred.promise);


                        var loadDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadDimensionSelectorPromiseDeferred.promise);

                        var loadRecordFilterPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadRecordFilterPromiseDeferred.promise);

                        UtilsService.waitMultiplePromises([measuresSelectorReadyPromiseDeferred.promise, recordFilterDirectiveReadyPromiseDeferred.promise, analyticTableMeasureSelectedPromiseDeferred.promise, analyticTableRecordFilterSelectedPromiseDeferred.promise, dimensionsSelectorReadyPromiseDeferred.promise, analyticTableDimesnionSelectedPromiseDeferred.promise]).then(function () {
                            var measuresSelectorPayload = {
                                filter: {
                                    TableIds: [analyticTableSelectorAPI.getSelectedIds()]
                                }
                            };
                            if (measureNames != undefined && measureNames.length > 0)
                                measuresSelectorPayload.selectedIds = measureNames;
                            advancedOrderOptionDirectiveSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                            VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, measuresSelectorPayload, loadMeasureSelectorPromiseDeferred);
                            analyticTableMeasureSelectedPromiseDeferred = undefined;

                            UtilsService.waitMultiplePromises([advancedOrderOptionReadyPromiseDeferred.promise, advancedOrderOptionDirectiveSelectedPromiseDeferred.promise]).then(function () {
                                var selectedMeasures = $scope.scopeModel.selectedMeasures;
                                if (selectedMeasures != undefined && selectedMeasures.length > 0) {
                                    var onlySelectedMeasureIds = [];
                                    for (var i = 0; i < $scope.scopeModel.selectedMeasures.length; i++) {
                                        var selectedMeasure = $scope.scopeModel.selectedMeasures[i];
                                        onlySelectedMeasureIds.push(selectedMeasure.AnalyticItemConfigId)
                                    }
                                }
                                var advancedOrderOptionDirectivePayload = {
                                    tableIds: [analyticTableSelectorAPI.getSelectedIds()],
                                    onlySelectedMeasureIds: onlySelectedMeasureIds
                                };
                                if (advancedOrderOptions != undefined)
                                    advancedOrderOptionDirectivePayload.advancedOrderOptions = advancedOrderOptions;
                                VRUIUtilsService.callDirectiveLoad(advancedOrderOptionDirectiveAPI, advancedOrderOptionDirectivePayload, loadAdvamcedOrderOptionDirectivePromiseDeferred);
                                advancedOrderOptionDirectiveSelectedPromiseDeferred = undefined;
                            });
                            
                          

                            var dimensionsSelectorPayload = {
                                filter: {
                                    TableIds: [analyticTableSelectorAPI.getSelectedIds()]
                                }
                            };
                            if (dimensionName != undefined)
                                dimensionsSelectorPayload.selectedIds = dimensionName;
                            VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, dimensionsSelectorPayload, loadDimensionSelectorPromiseDeferred);
                            analyticTableDimesnionSelectedPromiseDeferred = undefined;


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

                function loadPeriodSelector() {
                    var loadPeriodSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    periodSelectorReadyPromiseDeferred.promise.then(function () {
                        var periodSelectorPayload;
                        if (timePeriod != undefined)
                            periodSelectorPayload = {
                                timePeriod: timePeriod
                            };
                        VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, loadPeriodSelectorPromiseDeferred);
                    });
                    return loadPeriodSelectorPromiseDeferred.promise;
                }

            }
        }
    }

    app.directive('vrAnalyticMostrelevantFigurestilequery', MostRelevantAnalyticFiguresTileQuery);

})(app);
