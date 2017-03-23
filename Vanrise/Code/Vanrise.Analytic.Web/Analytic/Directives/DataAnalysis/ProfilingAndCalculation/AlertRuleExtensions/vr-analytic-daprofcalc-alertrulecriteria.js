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

            var groupingOutputFiledsAPI;
            var groupingOutputFiledsReadyDeferred = UtilsService.createPromiseDeferred();

            var dataAnalysisRecordFilterDirectiveAPI;
            var dataAnalysisRecordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                var promises = [dataAnalysisItemDefinitionSelectoReadyDeferred.promise, dataAnalysisRecordFilterDirectiveReadyDeferred.promise, groupingOutputFiledsReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisItemDefinitionSelectorAPI = api;
                    dataAnalysisItemDefinitionSelectoReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataAnalysisItemDefinitionSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        $scope.scopeModel.isDataanalysisitemdefinitionSelected = true;

                        if (dataAnalysisItemDefinitionSelectionChangedDeferred != undefined) {
                            dataAnalysisItemDefinitionSelectionChangedDeferred.resolve();
                        }
                        else {
                            if (ctrl.onCriteriaSelectionChanged != undefined && typeof (ctrl.onCriteriaSelectionChanged) == 'function') {
                                ctrl.onCriteriaSelectionChanged(selectedItem);
                            }

                            getGroupingOutputFieldsPromise();

                            function getGroupingOutputFieldsPromise() {
                                var groupingOutputFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                                var filter = { DAProfCalcOutputFieldType: VR_Analytic_DAProfCalcOutputFieldTypeEnum.GroupingField.value };
                                var groupingOutputFieldsPayload = { dataAnalysisItemDefinitionId: selectedItem.DataAnalysisItemDefinitionId, filter: filter };
                                VRUIUtilsService.callDirectiveLoad(groupingOutputFiledsAPI, groupingOutputFieldsPayload, groupingOutputFieldsLoadDeferred);

                                return groupingOutputFieldsLoadDeferred.promise;
                            };
                        }
                    }
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

                    var dataAnalysisRecordFilterDirectiveLoadPromise = getDataAnalysisRecordFilterDirectiveLoadPromise();
                    promises.push(dataAnalysisRecordFilterDirectiveLoadPromise);

                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);

                    //Loading Record Filter Directive
                    if (daProfCalcOutputItemDefinitionId != undefined) {
                        var groupingOutputFieldsLoadPromise = getGroupingOutputFieldsPromise();
                        promises.push(groupingOutputFieldsLoadPromise);
                    }

                    function getDataAnalysisItemDefinitionSelectorLoadPromise() {
                        if (daProfCalcOutputItemDefinitionId != undefined)
                            dataAnalysisItemDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorPayload = {};
                        dataAnalysisItemDefinitionSelectorPayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                        if (criteria != undefined) {
                            dataAnalysisItemDefinitionSelectorPayload.selectedIds = daProfCalcOutputItemDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisItemDefinitionSelectorAPI, dataAnalysisItemDefinitionSelectorPayload, dataAnalysisItemDefinitionSelectorLoadDeferred);

                        return dataAnalysisItemDefinitionSelectorLoadDeferred.promise;
                    };

                    function getGroupingOutputFieldsPromise() {
                        var groupingOutputFieldsLoadDeferred = UtilsService.createPromiseDeferred();

                        dataAnalysisItemDefinitionSelectionChangedDeferred.promise.then(function () {
                            dataAnalysisItemDefinitionSelectionChangedDeferred = undefined;

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