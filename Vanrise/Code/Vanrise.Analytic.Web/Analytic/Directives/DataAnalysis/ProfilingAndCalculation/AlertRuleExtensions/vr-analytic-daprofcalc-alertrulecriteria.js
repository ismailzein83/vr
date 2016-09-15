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
            var outputFields;

            var dataAnalysisItemDefinitionSelectorAPI;
            var dataAnalysisItemDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();
            var dataAnalysisItemDefinitionSelectionChangedDeferred;

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
                   
                    console.log(daProfCalcOutputItemDefinitionId);

                    if (daProfCalcOutputItemDefinitionId != undefined) {
                        VR_Analytic_DAProfCalcOutputSettingsAPIService.GetOutputFields(daProfCalcOutputItemDefinitionId).then(function (response) {

                            outputFields = response;

                            if (dataAnalysisItemDefinitionSelectionChangedDeferred != undefined) {
                                dataAnalysisItemDefinitionSelectionChangedDeferred.resolve();
                            }
                            else {
                                var recordFilterDirectivePayload = {};
                                recordFilterDirectivePayload.context = buildContext();
                                var setLoader = function (value) {
                                    $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                                };

                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader);

                                $scope.scopeModel.isDataanalysisitemdefinitionSelected = true;
                            }
                        });
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

                    daProfCalcOutputItemDefinitionId = undefined;
                    $scope.scopeModel.isDataanalysisitemdefinitionSelected = false;

                    var promises = [];
                    var dataAnalysisDefinitionId;
                    var criteria;

                    if (payload != undefined) {
                        dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                        criteria = payload.criteria;

                        if (criteria != undefined) {
                            daProfCalcOutputItemDefinitionId = criteria.DAProfCalcOutputItemDefinitionId;
                        }
                    }

                        
                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);

                    //Loading Record Filter Directive
                    if (daProfCalcOutputItemDefinitionId != undefined) {
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

                        if (dataAnalysisItemDefinitionSelectionChangedDeferred == undefined)
                            dataAnalysisItemDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {

                            dataAnalysisItemDefinitionSelectionChangedDeferred.promise.then(function () {
                                dataAnalysisItemDefinitionSelectionChangedDeferred = undefined;

                                var recordFilterDirectivePayload = {};
                                recordFilterDirectivePayload.context = buildContext();
                                if (criteria != undefined) {
                                    recordFilterDirectivePayload.FilterGroup = criteria.FilterGroup;
                                }

                                console.log(recordFilterDirectivePayload);

                                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                            });
                        })

                        return recordFilterDirectiveLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isDataanalysisitemdefinitionSelected = true;
                        });
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
                }
                return context;
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertrulecriteria', DAProfCalcAlertRuleCriteriaDirective);

})(app);