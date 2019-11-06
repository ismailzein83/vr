﻿(function (appControllers) {

    'use strict';

    DAProfCalcAggregationFieldController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DAProfCalcAggregationFieldController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var aggregationFieldEntity;
        var context;

        var recordAggregateSelectiveAPI;
        var recordAggregateSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var timeRangeFilterSelectiveAPI;
        var timeRangeFilterSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var timePeriodDirectiveAPI;
        var timePeriodDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                aggregationFieldEntity = parameters.daProfCalcAggregationField;
                context = parameters.context;
            }

            isEditMode = (aggregationFieldEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRecordAggregateSelectiveReady = function (api) {
                recordAggregateSelectiveAPI = api;
                recordAggregateSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onTimeRangeFilterSelectiveReady = function (api) {
                timeRangeFilterSelectiveAPI = api;
                timeRangeFilterSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onTimePeriodDirectiveReady = function (api) {
                timePeriodDirectiveAPI = api;
                timePeriodDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRecordAggregateSelective, loadTimeRangeFilterSelective,
                loadRecordFilterDirective, loadTimePeriodDirective]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((aggregationFieldEntity != undefined) ? aggregationFieldEntity.FieldName : null, 'Profiling and Calculation Aggregation Field') :
                    UtilsService.buildTitleForAddEditor('Profiling and Calculation Aggregation Field');
            }
            function loadStaticData() {
                if (aggregationFieldEntity == undefined) return;

                $scope.scopeModel.fieldName = aggregationFieldEntity.FieldName;
                $scope.scopeModel.fieldTitle = aggregationFieldEntity.FieldTitle;
            }
            function loadRecordAggregateSelective() {
                var recordAggregateSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                recordAggregateSelectiveReadyDeferred.promise.then(function () {
                    var recordAggregateSelectivePayload = {};
                    recordAggregateSelectivePayload.context = builContext();
                    if (aggregationFieldEntity != undefined) {
                        recordAggregateSelectivePayload.recordAggregate = aggregationFieldEntity.RecordAggregate;
                    }

                    VRUIUtilsService.callDirectiveLoad(recordAggregateSelectiveAPI, recordAggregateSelectivePayload, recordAggregateSelectiveLoadDeferred);
                });

                return recordAggregateSelectiveLoadDeferred.promise;
            }
            function loadTimeRangeFilterSelective() {
                var timeRangeFilterSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                timeRangeFilterSelectiveReadyDeferred.promise.then(function () {
                    var timeRangeFilterSelectivePayload = {};
                    if (aggregationFieldEntity != undefined) {
                        timeRangeFilterSelectivePayload.timeRangeFilter = aggregationFieldEntity.TimeRangeFilter;
                    }

                    VRUIUtilsService.callDirectiveLoad(timeRangeFilterSelectiveAPI, timeRangeFilterSelectivePayload, timeRangeFilterSelectiveLoadDeferred);
                });

                return timeRangeFilterSelectiveLoadDeferred.promise;
            }
            function loadRecordFilterDirective() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                recordFilterDirectiveReadyDeferred.promise.then(function () {
                    var recordFilterDirectivePayload = {};
                    recordFilterDirectivePayload.context = builContext();
                    if (aggregationFieldEntity != undefined && aggregationFieldEntity.RecordFilter != undefined) {
                        recordFilterDirectivePayload.FilterGroup = aggregationFieldEntity.RecordFilter;
                    }

                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                });

                return recordFilterDirectiveLoadDeferred.promise;
            }
            function loadTimePeriodDirective() {
                var timePeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                timePeriodDirectiveReadyDeferred.promise.then(function () {

                    var timePeriodDirectivePayload = {};
                    if (aggregationFieldEntity != undefined && aggregationFieldEntity.TimeFilter != undefined) {
                        timePeriodDirectivePayload.timePeriod = aggregationFieldEntity.TimeFilter.TimePeriod;

                        $scope.scopeModel.excludeFrom = aggregationFieldEntity.TimeFilter.ExcludeFrom;
                        $scope.scopeModel.excludeTo = aggregationFieldEntity.TimeFilter.ExcludeTo;
                    }
                    VRUIUtilsService.callDirectiveLoad(timePeriodDirectiveAPI, timePeriodDirectivePayload, timePeriodDirectiveLoadDeferred);
                });

                return timePeriodDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var aggregationFieldObject = buildAggregationFieldObjectFromScope();

            if ($scope.onDAProfCalcAggregationFieldAdded != undefined && typeof ($scope.onDAProfCalcAggregationFieldAdded) == 'function') {
                $scope.onDAProfCalcAggregationFieldAdded(aggregationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var aggregationFieldObject = buildAggregationFieldObjectFromScope();

            if ($scope.onDAProfCalcAggregationFieldUpdated != undefined && typeof ($scope.onDAProfCalcAggregationFieldUpdated) == 'function') {
                $scope.onDAProfCalcAggregationFieldUpdated(aggregationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function builContext() {
            return context;
        }
        function buildAggregationFieldObjectFromScope() {
            var timeFilter;
            var timePeriodData = timePeriodDirectiveAPI.getData();

            if (timePeriodData != undefined) {
                timeFilter = {
                    TimePeriod: timePeriodData,
                    ExcludeFrom: $scope.scopeModel.excludeFrom,
                    ExcludeTo: $scope.scopeModel.excludeTo
                };
            }

            return {
                FieldName: $scope.scopeModel.fieldName,
                FieldTitle: $scope.scopeModel.fieldTitle,
                RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                TimeFilter: timeFilter,
                TimeRangeFilter: timeRangeFilterSelectiveAPI.getData(),
                RecordAggregate: recordAggregateSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_Analytic_DAProfCalcAggregationFieldController', DAProfCalcAggregationFieldController);
})(appControllers);