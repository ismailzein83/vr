
'use strict';
                
app.directive('vrAnalyticRecordprofilingoutputsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recordProfilingOutputSettingsEditor = new RecordProfilingOutputSettingsEditor($scope, ctrl, $attrs);
                recordProfilingOutputSettingsEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/OutputDefinitions/Templates/RecordProfilingOutputSettingsEditorTemplate.html'
        };

    function RecordProfilingOutputSettingsEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var timeRangeFilterSelectiveAPI;
        var timeRangeFilterSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var groupingFieldsDirectiveAPI;
        var groupingFieldsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var aggregationFieldsDirectiveAPI;
        var aggregationFieldsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var calculationFieldsDirectiveAPI;
        var calculationFieldsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {
            var promises = [recordFilterDirectiveReadyDeferred.promise, groupingFieldsDirectiveReadyDeferred.promise,
                            aggregationFieldsDirectiveReadyDeferred.promise, calculationFieldsDirectiveReadyDeferred.promise];

            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onTimeRangeFilterSelectiveReady = function (api) {
                timeRangeFilterSelectiveAPI = api;
                timeRangeFilterSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onGroupingFieldsDirectiveReady = function (api) {
                groupingFieldsDirectiveAPI = api;
                groupingFieldsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onAggregationFieldsDirectiveReady = function (api) {
                aggregationFieldsDirectiveAPI = api;
                aggregationFieldsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onCalculationFieldsDirectiveReady = function (api) {
                calculationFieldsDirectiveAPI = api;
                calculationFieldsDirectiveReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises(promises).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var promises = [];

                var context;
                var recordTypeId;
                var recordFilter;
                var timeRangeFilter;
                var groupingFields;
                var aggregationFields;
                var calculationFields;

                if(payload != undefined){
                    context = payload.context;

                    if (payload.dataAnalysisItemDefinitionSettings != undefined) {
                        recordTypeId = payload.dataAnalysisItemDefinitionSettings.RecordTypeId;
                        recordFilter = payload.dataAnalysisItemDefinitionSettings.RecordFilter;
                        timeRangeFilter = payload.dataAnalysisItemDefinitionSettings.TimeRangeFilter;
                        groupingFields = payload.dataAnalysisItemDefinitionSettings.GroupingFields;
                        aggregationFields = payload.dataAnalysisItemDefinitionSettings.AggregationFields;
                        calculationFields = payload.dataAnalysisItemDefinitionSettings.CalculationFields;
                    }
                }

                //Loading Data Record Type Selector
                var DataRecordTypeSelectorLoadPromise = getDataRecordTypeSelectorLoadPromise();
                promises.push(DataRecordTypeSelectorLoadPromise);

                //Loading Record Filter Directive
                var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                promises.push(recordFilterDirectiveLoadPromise);

                //Loading Time Range Filter Selective
                var timeRangeFilterSelectiveLoadPromise = getTimeRangeFilterSelectiveLoadPromise();
                promises.push(timeRangeFilterSelectiveLoadPromise);

                //Loading Grouping Fields Directive
                var groupingFieldsDirectiveLoadPromise = getGroupingFieldsDirectiveLoadPromise();
                promises.push(groupingFieldsDirectiveLoadPromise);

                //Loading Aggregation Fields Directive
                var aggregationFieldsDirectiveLoadPromise = getAggregationFieldsDirectiveLoadPromise();
                promises.push(aggregationFieldsDirectiveLoadPromise);

                //Loading Calculation Fields Directive
                var calculationFieldsDirectiveLoadPromise = getCalculationFieldsDirectiveLoadPromise();
                promises.push(calculationFieldsDirectiveLoadPromise);


                function getDataRecordTypeSelectorLoadPromise() {
                    var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                        var dataRecordTypeSelectorPayload = {};
                        dataRecordTypeSelectorPayload.selectedIds = recordTypeId;
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                    });

                    return dataRecordTypeSelectorLoadDeferred.promise;
                }
                function getRecordFilterDirectiveLoadPromise() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        if (recordFilter != undefined) {
                            recordFilterDirectivePayload.FilterGroup = recordFilter;
                        }

                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                }
                function getTimeRangeFilterSelectiveLoadPromise() {
                    var timeRangeFilterSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    timeRangeFilterSelectiveReadyDeferred.promise.then(function () {
                        var timeRangeFilterSelectivePayload = {};
                        if (timeRangeFilter != undefined) {
                            timeRangeFilterSelectivePayload.timeRangeFilter = timeRangeFilter;
                        }

                        VRUIUtilsService.callDirectiveLoad(timeRangeFilterSelectiveAPI, timeRangeFilterSelectivePayload, timeRangeFilterSelectiveLoadDeferred);
                    });

                    return timeRangeFilterSelectiveLoadDeferred.promise;
                }
                function getGroupingFieldsDirectiveLoadPromise() {
                    var groupingFieldsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    groupingFieldsDirectiveReadyDeferred.promise.then(function () {
                        var groupingFieldsDirectivePayload = {};
                        groupingFieldsDirectivePayload.context = buildContext();
                        groupingFieldsDirectivePayload.groupingFields = groupingFields;

                        VRUIUtilsService.callDirectiveLoad(groupingFieldsDirectiveAPI, groupingFieldsDirectivePayload, groupingFieldsDirectiveLoadDeferred);
                    });

                    return groupingFieldsDirectiveLoadDeferred.promise;
                }
                function getAggregationFieldsDirectiveLoadPromise() {
                    var aggregationFieldsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    aggregationFieldsDirectiveReadyDeferred.promise.then(function () {
                        var aggregationFieldsDirectivePayload = {};
                        aggregationFieldsDirectivePayload.context = buildContext();
                        aggregationFieldsDirectivePayload.aggregationFields = aggregationFields;

                        VRUIUtilsService.callDirectiveLoad(aggregationFieldsDirectiveAPI, aggregationFieldsDirectivePayload, aggregationFieldsDirectiveLoadDeferred);
                    });

                    return aggregationFieldsDirectiveLoadDeferred.promise;
                }
                function getCalculationFieldsDirectiveLoadPromise() {
                    var calculationFieldsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    calculationFieldsDirectiveReadyDeferred.promise.then(function () {
                        var calculationFieldsDirectivePayload = {};
                        calculationFieldsDirectivePayload.context = buildContext();
                        calculationFieldsDirectivePayload.calculationFields = calculationFields;

                        VRUIUtilsService.callDirectiveLoad(calculationFieldsDirectiveAPI, calculationFieldsDirectivePayload, calculationFieldsDirectiveLoadDeferred);
                    });

                    return calculationFieldsDirectiveLoadDeferred.promise;
                }

                function buildContext() {
                    return context;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                var data = {
                    $type: "Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities",
                    RecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                    TimeRangeFilter: timeRangeFilterSelectiveAPI.getData(),
                    GroupingFields: groupingFieldsDirectiveAPI.getData(),
                    AggregationFields: aggregationFieldsDirectiveAPI.getData(),
                    CalculationFields: calculationFieldsDirectiveAPI.getData()
                };

                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

}]);
