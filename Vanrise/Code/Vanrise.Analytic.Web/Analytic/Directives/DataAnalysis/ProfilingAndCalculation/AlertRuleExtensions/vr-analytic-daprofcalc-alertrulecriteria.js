﻿(function (app) {

    'use strict';

    DAProfCalcAlertRuleCriteriaDirective.$inject = ["VR_Analytic_DAProfCalcOutputSettingsAPIService", "UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_Analytic_DAProfCalcOutputFieldTypeEnum'];

    function DAProfCalcAlertRuleCriteriaDirective(VR_Analytic_DAProfCalcOutputSettingsAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VR_Analytic_DAProfCalcOutputFieldTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onCriteriaSelectionChanged: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcAlertRuleCriteria = new DAProfCalcAlertRuleCriteria($scope, ctrl, $attrs);
                daProfCalcAlertRuleCriteria.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleCriteriaTemplate.html"
        };
        function DAProfCalcAlertRuleCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectedOutputFields = [];
            var daProfCalcOutputItemDefinitionId;
            var dataAnalysisItemOutputFields;

            var dataAnalysisRecordFilterDirectiveAPI;
            var dataAnalysisRecordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var dataAnalysisItemDefinitionSelectorAPI;
            var dataAnalysisItemDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();
            var dataAnalysisItemDefinitionSelectionChangedDeferred;

            var groupingOutputFiledsAPI;
            var groupingOutputFiledsReadyDeferred = UtilsService.createPromiseDeferred();

            var dataAnalysisItemObj;

            function initializeController() {
                var promises = [dataAnalysisItemDefinitionSelectoReadyDeferred.promise, dataAnalysisRecordFilterDirectiveReadyDeferred.promise, groupingOutputFiledsReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisItemDefinitionSelectorAPI = api;
                    dataAnalysisItemDefinitionSelectoReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataAnalaysisRecordFilterDirectiveReady = function (api) {
                    dataAnalysisRecordFilterDirectiveAPI = api;
                    dataAnalysisRecordFilterDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataAnalysisItemOutputFieldsSelectorReady = function (api) {
                    groupingOutputFiledsAPI = api;
                    groupingOutputFiledsReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectionChanged = function (selectedItem) {
                    dataAnalysisItemObj = selectedItem;
                    if (selectedItem != undefined) {
                        $scope.scopeModel.isDataAnalysisItemDefinitionSelected = true;

                        if (dataAnalysisItemDefinitionSelectionChangedDeferred != undefined) {
                            dataAnalysisItemDefinitionSelectionChangedDeferred.resolve();
                        }
                        else {
                            if (ctrl.onCriteriaSelectionChanged != undefined && typeof (ctrl.onCriteriaSelectionChanged) == 'function') {
                                ctrl.onCriteriaSelectionChanged(selectedItem, undefined);
                            }

                            getGroupingOutputFieldsPromise();

                            function getGroupingOutputFieldsPromise() {

                                var filter = { DAProfCalcOutputFieldType: VR_Analytic_DAProfCalcOutputFieldTypeEnum.GroupingField.value };
                                var groupingOutputFieldsPayload = { dataAnalysisItemDefinitionId: selectedItem.DataAnalysisItemDefinitionId, filter: filter };

                                var setLoader = function (value) {
                                    $scope.scopeModel.isOutputFieldsSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, groupingOutputFiledsAPI, groupingOutputFieldsPayload, setLoader, undefined);
                            };
                        }
                    }
                };

                $scope.scopeModel.onSelectDataAnalysisItemOutputFields = function (selectedItem) {
                    if (ctrl.onCriteriaSelectionChanged != undefined && typeof (ctrl.onCriteriaSelectionChanged) == 'function') {
                        selectedOutputFields.push(selectedItem.Name);
                        ctrl.onCriteriaSelectionChanged(dataAnalysisItemObj, selectedOutputFields);
                    }
                };

                $scope.scopeModel.onDeselectDataAnalysisItemOutputFields = function (deselectedItem) {
                    if (ctrl.onCriteriaSelectionChanged != undefined && typeof (ctrl.onCriteriaSelectionChanged) == 'function') {
                        selectedOutputFields.splice(selectedOutputFields.indexOf(deselectedItem.Name), 1);
                        ctrl.onCriteriaSelectionChanged(dataAnalysisItemObj, selectedOutputFields);
                    }
                };

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isDataAnalysisItemDefinitionSelected = false;

                    var promises = [];
                    var dataAnalysisDefinitionId;
                    var criteria;

                    if (payload != undefined) {
                        dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                        $scope.scopeModel.rawRecordFilterLabel = payload.rawRecordFilterLabel;
                        criteria = payload.criteria;

                        if (criteria != undefined) {
                            daProfCalcOutputItemDefinitionId = criteria.DAProfCalcOutputItemDefinitionId;
                            $scope.scopeModel.minNotificationInterval = criteria.MinNotificationInterval;

                            if (criteria.GroupingFieldNames != undefined) {
                                for (var x = 0; x < criteria.GroupingFieldNames.length; x++) {
                                    selectedOutputFields.push(criteria.GroupingFieldNames[x]);
                                }
                            }
                        }
                        else {
                            $scope.scopeModel.minNotificationInterval = '0.01:00:00';
                        }
                    }

                    //Loading Data Analysis Record Filter Directive
                    var dataAnalysisRecordFilterDirectiveLoadPromise = getDataAnalysisRecordFilterDirectiveLoadPromise();
                    promises.push(dataAnalysisRecordFilterDirectiveLoadPromise);

                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);

                    //Loading Grouping Output Fields Selector
                    if (daProfCalcOutputItemDefinitionId != undefined) {
                        var groupingOutputFieldsLoadPromise = getGroupingOutputFieldsPromise();
                        promises.push(groupingOutputFieldsLoadPromise);
                    }

                    function getDataAnalysisRecordFilterDirectiveLoadPromise() {
                        var dataAnalysisRecordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        VR_Analytic_DAProfCalcOutputSettingsAPIService.GetInputFields(dataAnalysisDefinitionId).then(function (response) {

                            var dataAnalysisRecordFilterDirectivePayload = {
                                context: buildRecordFilterContext(response),
                                criteria: criteria != undefined ? criteria.DataAnalysisFilterGroup : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataAnalysisRecordFilterDirectiveAPI, dataAnalysisRecordFilterDirectivePayload, dataAnalysisRecordFilterDirectiveLoadDeferred);
                        });

                        return dataAnalysisRecordFilterDirectiveLoadDeferred.promise;
                    };
                    function getDataAnalysisItemDefinitionSelectorLoadPromise() {
                        if (daProfCalcOutputItemDefinitionId != undefined)
                            dataAnalysisItemDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorPayload = {
                            dataAnalysisDefinitionId: dataAnalysisDefinitionId,
                            selectedIds: daProfCalcOutputItemDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisItemDefinitionSelectorAPI, dataAnalysisItemDefinitionSelectorPayload, dataAnalysisItemDefinitionSelectorLoadDeferred);

                        return dataAnalysisItemDefinitionSelectorLoadDeferred.promise;
                    };
                    function getGroupingOutputFieldsPromise() {
                        var groupingOutputFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                        dataAnalysisItemDefinitionSelectionChangedDeferred.promise.then(function () {
                            dataAnalysisItemDefinitionSelectionChangedDeferred = undefined;

                            var filter = { DAProfCalcOutputFieldType: VR_Analytic_DAProfCalcOutputFieldTypeEnum.GroupingField.value };
                            var groupingOutputFieldsPayload = {
                                dataAnalysisItemDefinitionId: daProfCalcOutputItemDefinitionId,
                                selectedIds: criteria.GroupingFieldNames,
                                filter: filter
                            };
                            VRUIUtilsService.callDirectiveLoad(groupingOutputFiledsAPI, groupingOutputFieldsPayload, groupingOutputFieldsLoadDeferred);
                        });

                        return groupingOutputFieldsLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        DAProfCalcOutputItemDefinitionId: dataAnalysisItemDefinitionSelectorAPI.getSelectedIds(),
                        DataAnalysisFilterGroup: dataAnalysisRecordFilterDirectiveAPI.getData().filterObj,
                        GroupingFieldNames: groupingOutputFiledsAPI.getSelectedIds(),
                        MinNotificationInterval: $scope.scopeModel.minNotificationInterval
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildRecordFilterContext(outputFields) {
                var context = {
                    getFields: function () {
                        var fields = [];

                        if (outputFields) {
                            for (var i = 0 ; i < outputFields.length; i++) {
                                var field = outputFields[i];
                                fields.push({
                                    FieldName: field.Name,
                                    FieldTitle: field.Title,
                                    Type: field.Type
                                })
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertrulecriteria', DAProfCalcAlertRuleCriteriaDirective);

})(app);