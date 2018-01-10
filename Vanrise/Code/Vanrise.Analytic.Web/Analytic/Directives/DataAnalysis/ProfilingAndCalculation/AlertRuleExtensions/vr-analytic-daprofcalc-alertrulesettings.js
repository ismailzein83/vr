
'use strict';

app.directive('vrAnalyticDaprofcalcAlertrulesettings', ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_DAProfCalcOutputSettingsAPIService', 'VR_Analytic_DAProfCalcNotificationAPIService',
    function (UtilsService, VRUIUtilsService, VR_Analytic_DAProfCalcOutputSettingsAPIService, VR_Analytic_DAProfCalcNotificationAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cstr = new AlertRuleSettings($scope, ctrl, $attrs);
                cstr.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleSettingsTemplate.html"
        };

        function AlertRuleSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var analysisTypeId;
            var outputFields;
            var alertExtendedSettings;
            var alertTypeSettings;
            var vrAlertRuleTypeId;
            var notificationTypeId;

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var criteriaDirectiveSelectionChangeDeferred;

            var dataRecordAlertRuleSettingsAPI;
            var dataRecordAlertRuleSettingReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordAlertRuleSettingsReady = function (api) {
                    dataRecordAlertRuleSettingsAPI = api;
                    dataRecordAlertRuleSettingReadyDeferred.resolve();
                };

                $scope.scopeModel.onCriteriaSelectionChanged = function (daProfCalcOutputItemDefinition, selectedGroupingFields) {
                    $scope.scopeModel.isDataRecordAlertRuleSettingsDirectiveLoading = true;

                    analysisTypeId = $scope.scopeModel.selectedAnalysisTypeId = daProfCalcOutputItemDefinition.DataAnalysisItemDefinitionId;

                    VR_Analytic_DAProfCalcNotificationAPIService.GetDAProfCalcNotificationTypeId(vrAlertRuleTypeId, analysisTypeId).then(function (response) {
                        notificationTypeId = response;
                        VR_Analytic_DAProfCalcOutputSettingsAPIService.GetOutputFields(analysisTypeId).then(function (response) {
                            outputFields = response;

                            var dataRecordAlertRuleSettingsPayload = {
                                settings: undefined,
                                context: buildContext(selectedGroupingFields)
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isDataRecordAlertRuleSettingsDirectiveLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordAlertRuleSettingsAPI, dataRecordAlertRuleSettingsPayload, setLoader, undefined);
                        });
                    });
                };

                $scope.scopeModel.hasSettingsData = function () {
                    return dataRecordAlertRuleSettingsAPI != undefined ? dataRecordAlertRuleSettingsAPI.hasData() : false;
                };

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        vrAlertRuleTypeId = payload.vrAlertRuleTypeId;
                        alertTypeSettings = payload.alertTypeSettings;
                        alertExtendedSettings = payload.alertExtendedSettings;

                        if (alertExtendedSettings != undefined) {
                            analysisTypeId = alertExtendedSettings.OutputItemDefinitionId;
                        }
                    }

                    var criteriaSectionLoadPromise = getCriteriaSectionLoadPromise();
                    promises.push(criteriaSectionLoadPromise);

                    if (alertExtendedSettings != undefined) {
                        var dataRecordAlertRuleSettingsLoadPromise = getDataRecordAlertRuleSettingsLoadPromise();
                        promises.push(dataRecordAlertRuleSettingsLoadPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var criteria = criteriaDirectiveAPI.getData();
                    return {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleSettings,Vanrise.Analytic.Entities",
                        FilterGroup: criteria.FilterGroup,
                        DataAnalysisFilterGroup: criteria.DataAnalysisFilterGroup,
                        OutputItemDefinitionId: criteria.DAProfCalcOutputItemDefinitionId,
                        GroupingFieldNames: criteria.GroupingFieldNames,
                        Settings: dataRecordAlertRuleSettingsAPI.getData(),
                        MinNotificationInterval: criteria.MinNotificationInterval,
                        DAProfCalcAnalysisPeriod: criteria.DAProfCalcAnalysisPeriod
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function getCriteriaSectionLoadPromise() {
                var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();

                criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                    var criteriapayload = { dataAnalysisDefinitionId: alertTypeSettings.DataAnalysisDefinitionId, rawRecordFilterLabel: alertTypeSettings.RawRecordFilterLabel };

                    if (alertExtendedSettings != undefined) {
                        criteriapayload.criteria = {
                            DAProfCalcOutputItemDefinitionId: alertExtendedSettings.OutputItemDefinitionId,
                            MinNotificationInterval: alertExtendedSettings.MinNotificationInterval,
                            DataAnalysisFilterGroup: alertExtendedSettings.DataAnalysisFilterGroup,
                            GroupingFieldNames: alertExtendedSettings.GroupingFieldNames,
                            DAProfCalcAnalysisPeriod: alertExtendedSettings.DAProfCalcAnalysisPeriod
                        };
                        $scope.scopeModel.selectedAnalysisTypeId = alertExtendedSettings.OutputItemDefinitionId;
                    }

                    VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriapayload, loadCriteriaSectionPromiseDeferred);
                });

                return loadCriteriaSectionPromiseDeferred.promise;
            };

            function getDataRecordAlertRuleSettingsLoadPromise() {
                var dataRecordAlertRuleSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                VR_Analytic_DAProfCalcNotificationAPIService.GetDAProfCalcNotificationTypeId(vrAlertRuleTypeId, analysisTypeId).then(function (response) {
                    notificationTypeId = response;
                    VR_Analytic_DAProfCalcOutputSettingsAPIService.GetOutputFields(analysisTypeId).then(function (response) {
                        outputFields = response;

                        dataRecordAlertRuleSettingReadyDeferred.promise.then(function () {

                            var payloadSettings;
                            var payloadContext;

                            if (alertExtendedSettings != undefined) {
                                payloadSettings = alertExtendedSettings.Settings;
                                payloadContext = buildContext(alertExtendedSettings.GroupingFieldNames);
                            }
                            else {
                                payloadContext = buildContext(undefined);
                            }

                            var dataRecordAlertRuleSettingsPayload = {
                                settings: payloadSettings,
                                context: payloadContext
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordAlertRuleSettingsAPI, dataRecordAlertRuleSettingsPayload, dataRecordAlertRuleSettingsLoadDeferred);
                        });
                    });
                });
                return dataRecordAlertRuleSettingsLoadDeferred.promise;
            };

            function buildContext(groupingFields) {
                return {
                    recordfields: buildRecordFields(outputFields),
                    vrActionTargetType: buildDAProfCalcTargetType(analysisTypeId, groupingFields),
                    notificationTypeId: notificationTypeId
                };
            };

            function buildRecordFields(fields) {
                var recordFields = [];
                for (var i = 0; i < fields.length; i++) {
                    var field = fields[i];
                    recordFields.push({
                        Name: field.Name,
                        Title: field.Title,
                        Type: field.Type
                    });
                };
                return recordFields;
            };

            function buildDAProfCalcTargetType(selectedAnalysisTypeId, groupingFields) {
                return {
                    $type: "Vanrise.Analytic.Business.DAProfCalcActionTargetType, Vanrise.Analytic.Business",
                    DataAnalysisItemDefinitionId: selectedAnalysisTypeId,
                    AvailableDataRecordFieldNames: groupingFields
                };
            };
        }
    }]);