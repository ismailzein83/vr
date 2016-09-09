(function (app) {

    'use strict';

    DAProfCalcAlertRuleCriteriaDirective.$inject = ["VR_Analytic_DAProfCalcOutputSettingsAPIService", "UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcAlertRuleCriteriaDirective(VR_Analytic_DAProfCalcOutputSettingsAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
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

            var dataAnalysisItemDefinitionSelectorAPI;
            var dataAnalysisItemDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                var promises = [dataAnalysisItemDefinitionSelectoReadyDeferred.promise, recordFilterDirectiveReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisItemDefinitionSelectorAPI = api;
                    dataAnalysisItemDefinitionSelectoReadyDeferred.resolve();
                }
                $scope.scopeModel.onDataAnalysisItemDefinitionSelectionChanged = function (api) {
                    daProfCalcOutputItemDefinitionId = dataAnalysisItemDefinitionSelectorAPI.getSelectedIds();
                    if (daProfCalcOutputItemDefinitionId != undefined) {

                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        var setLoader = function (value) {
                            $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader);
                    }
                }

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                })
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var dataAnalysisDefinitionId;
                    var criteria;

                    if (payload != undefined) {
                        dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                        criteria = payload.criteria;

                        if (criteria) {
                            daProfCalcOutputItemDefinitionId = criteria.DAProfCalcOutputItemDefinitionId;
                        }
                    }
                        

                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);

                    //Loading Record Filter Directive
                    if (daProfCalcOutputItemDefinitionId) {
                        var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                        promises.push(recordFilterDirectiveLoadPromise);
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
                    }
                    function getRecordFilterDirectiveLoadPromise() {
                        var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {
                            var recordFilterDirectivePayload = {};
                            recordFilterDirectivePayload.context = buildContext();
                            if (criteria != undefined) {
                                recordFilterDirectivePayload.FilterGroup = criteria.FilterGroup;
                            }

                            VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                        })

                        return recordFilterDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleCriteria, Vanrise.Analytic.Entities",
                        DAProfCalcOutputItemDefinitionId: dataAnalysisItemDefinitionSelectorAPI.getSelectedIds(),
                        FilterGroup: recordFilterDirectiveAPI.getData().filterObj
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        var fields = []

                        if (daProfCalcOutputItemDefinitionId) {
                            VR_Analytic_DAProfCalcOutputSettingsAPIService.GetOutputFields(daProfCalcOutputItemDefinitionId).then(function (response) {

                                var outputFields = response;
                                for (var i = 0 ; i < outputFields.length; i++) {
                                    var field = outputFields[i];
                                    fields.push({
                                        FieldName: field.Name,
                                        FieldTitle: field.Title,
                                        Type: field.Type
                                    })
                                }
                            });
                        }
                        return fields;
                    }
                }
                return context;
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertrulecriteria', DAProfCalcAlertRuleCriteriaDirective);

})(app);