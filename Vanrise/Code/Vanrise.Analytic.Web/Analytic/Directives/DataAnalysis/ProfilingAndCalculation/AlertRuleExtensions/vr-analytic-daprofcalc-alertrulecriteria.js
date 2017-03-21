(function (app) {

    'use strict';

    DAProfCalcAlertRuleCriteriaDirective.$inject = ["VR_Analytic_DAProfCalcOutputSettingsAPIService", "UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_Analytic_DAProfCalcOutputFieldTypeEnum'];

    function DAProfCalcAlertRuleCriteriaDirective(VR_Analytic_DAProfCalcOutputSettingsAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VR_Analytic_DAProfCalcOutputFieldTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onCriteriaSelectionChanged: "=",
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

            var daProfCalcOutputItemDefinitionId;
            var dataAnalysisItemOutputFields;

            var dataAnalysisItemDefinitionSelectorAPI;
            var dataAnalysisItemDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();
            var dataAnalysisItemDefinitionSelectionChangedDeferred;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var groupingOutputFiledsAPI;
            var groupingOutputFiledsReadyDeferred = UtilsService.createPromiseDeferred();

            var dataAnalysisRecordFilterDirectiveAPI;
            var dataAnalysisRecordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                var promises = [dataAnalysisItemDefinitionSelectoReadyDeferred.promise, recordFilterDirectiveReadyDeferred.promise, dataAnalysisRecordFilterDirectiveReadyDeferred.promise, groupingOutputFiledsReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisItemDefinitionSelectorAPI = api;
                    dataAnalysisItemDefinitionSelectoReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataAnalysisItemDefinitionSelectionChanged = function (selectedItem) {
                    daProfCalcOutputItemDefinitionId = dataAnalysisItemDefinitionSelectorAPI.getSelectedIds();

                    if (daProfCalcOutputItemDefinitionId != undefined) {
                        VR_Analytic_DAProfCalcOutputSettingsAPIService.GetOutputFields(daProfCalcOutputItemDefinitionId).then(function (response) {

                            dataAnalysisItemOutputFields = response;
                            if (dataAnalysisItemDefinitionSelectionChangedDeferred != undefined) {
                                dataAnalysisItemDefinitionSelectionChangedDeferred.resolve();
                            }
                            else {
                                if (ctrl.onCriteriaSelectionChanged != undefined && typeof (ctrl.onCriteriaSelectionChanged) == 'function') {
                                    ctrl.onCriteriaSelectionChanged(selectedItem);
                                }
                                var recordFilterDirectivePayload = {};
                                recordFilterDirectivePayload.context = buildRecordFilterContext(dataAnalysisItemOutputFields);
                                var setLoader = function (value) {
                                    $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                                };

                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader);

                                $scope.scopeModel.isDataanalysisitemdefinitionSelected = true;
                            }
                        });

                        if (dataAnalysisItemDefinitionSelectionChangedDeferred == undefined) {
                            var filter = { DAProfCalcOutputFieldType: VR_Analytic_DAProfCalcOutputFieldTypeEnum.GroupingField.value };
                            var groupingOutputFieldsPayload = { dataAnalysisItemDefinitionId: daProfCalcOutputItemDefinitionId, filter: filter };
                            var setGroupingOutputFieldsLoader = function (value) {
                                $scope.scopeModel.isGroupingOutputFieldsLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, groupingOutputFiledsAPI, groupingOutputFieldsPayload, setGroupingOutputFieldsLoader);
                        }
                    }
                };

                $scope.scopeModel.validateRecordFilter = function () {
                    if (recordFilterDirectiveAPI != undefined) {
                        var filterObj = recordFilterDirectiveAPI.getData().filterObj;
                        if (filterObj != undefined && filterObj != {})
                            return null;
                        else
                            return 'Record Filter is required';
                    }
                    return null;
                };

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalaysisRecordFilterDirectiveReady = function (api) {
                    dataAnalysisRecordFilterDirectiveAPI = api;
                    dataAnalysisRecordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalysisItemOutputFieldsSelectorReady = function (api) {
                    groupingOutputFiledsAPI = api;
                    groupingOutputFiledsReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    daProfCalcOutputItemDefinitionId = undefined;
                    $scope.scopeModel.isDataanalysisitemdefinitionSelected = false;

                    var promises = [];
                    var dataAnalysisDefinitionId;
                    var criteria;

                    if (payload != undefined) {
                        dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                        $scope.scopeModel.rawRecordFilterLabel = payload.rawRecordFilterLabel;
                        criteria = payload.criteria;

                        if (criteria != undefined) {
                            daProfCalcOutputItemDefinitionId = criteria.DAProfCalcOutputItemDefinitionId;
                        }
                    }

                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);

                    var dataAnalysisRecordFilterDirectiveLoadPromise = getDataAnalysisRecordFilterDirectiveLoadPromise();
                    promises.push(dataAnalysisRecordFilterDirectiveLoadPromise);

                    //Loading Record Filter Directive
                    if (daProfCalcOutputItemDefinitionId != undefined) {
                        dataAnalysisItemDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                        promises.push(recordFilterDirectiveLoadPromise);

                        var groupingOutputFieldsLoadPromise = getGroupingOutputFieldsPromise();
                        promises.push(groupingOutputFieldsLoadPromise);

                        UtilsService.waitMultiplePromises([recordFilterDirectiveLoadPromise, groupingOutputFieldsLoadPromise]).then(function () {
                            $scope.scopeModel.isDataanalysisitemdefinitionSelected = true;
                        });
                    }

                    function getDataAnalysisItemDefinitionSelectorLoadPromise() {

                        var dataAnalysisItemDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorPayload = {};
                        dataAnalysisItemDefinitionSelectorPayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                        if (criteria != undefined) {
                            dataAnalysisItemDefinitionSelectorPayload.selectedIds = criteria.DAProfCalcOutputItemDefinitionId
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisItemDefinitionSelectorAPI, dataAnalysisItemDefinitionSelectorPayload, dataAnalysisItemDefinitionSelectorLoadDeferred);

                        return dataAnalysisItemDefinitionSelectorLoadDeferred.promise;
                    };
                    function getRecordFilterDirectiveLoadPromise() {
                        var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {

                            dataAnalysisItemDefinitionSelectionChangedDeferred.promise.then(function () {
                                dataAnalysisItemDefinitionSelectionChangedDeferred = undefined;
                                var recordFilterDirectivePayload = {};
                                recordFilterDirectivePayload.context = buildRecordFilterContext(dataAnalysisItemOutputFields);
                                if (criteria != undefined) {
                                    recordFilterDirectivePayload.FilterGroup = criteria.FilterGroup;
                                }

                                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                            });
                        });
                        return recordFilterDirectiveLoadDeferred.promise;
                    };

                    function getGroupingOutputFieldsPromise() {

                        var groupingOutputFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                        groupingOutputFiledsReadyDeferred.promise.then(function () {

                            var filter = { DAProfCalcOutputFieldType: VR_Analytic_DAProfCalcOutputFieldTypeEnum.GroupingField.value };
                            var groupingOutputFieldsPayload = { dataAnalysisItemDefinitionId: daProfCalcOutputItemDefinitionId, filter: filter, selectedIds: criteria.GroupingFieldNames };
                            VRUIUtilsService.callDirectiveLoad(groupingOutputFiledsAPI, groupingOutputFieldsPayload, groupingOutputFieldsLoadDeferred);
                        });
                        return groupingOutputFieldsLoadDeferred.promise;
                    };

                    function getDataAnalysisRecordFilterDirectiveLoadPromise() {
                        var dataAnalysisRecordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        VR_Analytic_DAProfCalcOutputSettingsAPIService.GetInputFields(dataAnalysisDefinitionId).then(function (response) {
                            var dataAnalysisRecordFilterDirectivePayload = {};

                            dataAnalysisRecordFilterDirectivePayload.context = buildRecordFilterContext(response);
                            if (criteria != undefined) {
                                dataAnalysisRecordFilterDirectivePayload.FilterGroup = criteria.DataAnalysisFilterGroup;
                            }

                            VRUIUtilsService.callDirectiveLoad(dataAnalysisRecordFilterDirectiveAPI, dataAnalysisRecordFilterDirectivePayload, dataAnalysisRecordFilterDirectiveLoadDeferred);
                        });

                        return dataAnalysisRecordFilterDirectiveLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleCriteria, Vanrise.Analytic.Entities",
                        DAProfCalcOutputItemDefinitionId: dataAnalysisItemDefinitionSelectorAPI.getSelectedIds(),
                        FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                        DataAnalysisFilterGroup: dataAnalysisRecordFilterDirectiveAPI.getData().filterObj,
                        GroupingFieldNames: groupingOutputFiledsAPI.getSelectedIds()
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