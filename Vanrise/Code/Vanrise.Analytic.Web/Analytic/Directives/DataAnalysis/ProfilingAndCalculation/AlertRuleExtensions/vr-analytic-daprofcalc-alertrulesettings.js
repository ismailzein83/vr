
'use strict';

app.directive('vrAnalyticDaprofcalcAlertrulesettings', ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_DAProfCalcOutputSettingsAPIService', 'VR_Analytic_DAProfCalcNotificationAPIService', 'VR_Analytic_DataAnalysisItemDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_Analytic_DAProfCalcOutputSettingsAPIService, VR_Analytic_DAProfCalcNotificationAPIService, VR_Analytic_DataAnalysisItemDefinitionAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
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
            var daProfCalcParameters;

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var criteriaDirectiveSelectionChangeDeferred;

            var dataRecordAlertRuleSettingsAPI;
            var dataRecordAlertRuleSettingReadyDeferred = UtilsService.createPromiseDeferred();

            var parametersEditorDirectiveAPI;
            var parametersEditorDirectiveReadyDeferred;

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

                $scope.scopeModel.onParametersEditorDirectiveReady = function (api) {
                    parametersEditorDirectiveAPI = api;
                    parametersEditorDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onCriteriaSelectionChanged = function (daProfCalcOutputItemDefinition, selectedGroupingFields) {
                    $scope.scopeModel.isDataRecordAlertRuleSettingsDirectiveLoading = true;
                    $scope.scopeModel.isParameterSettingsDirectiveLoading = true;
                    analysisTypeId = $scope.scopeModel.selectedAnalysisTypeId = daProfCalcOutputItemDefinition.DataAnalysisItemDefinitionId;

                    var loadDataRecordAlertRuleSettingsDeferred = UtilsService.createPromiseDeferred();
                    var selectionChangedPromises = [loadDataRecordAlertRuleSettingsDeferred.promise];

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
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordAlertRuleSettingsAPI, dataRecordAlertRuleSettingsPayload, setLoader, undefined).then(function () {

                            });
                        });
                    });

                    var loadParametersEditorPromise = getParametersEditorPromise(analysisTypeId).then(function () {
                        $scope.scopeModel.isParameterSettingsDirectiveLoading = false;
                    });
                    selectionChangedPromises.push(loadParametersEditorPromise);

                    return UtilsService.waitMultiplePromises(selectionChangedPromises);
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
                    var daProfCalcOutputItemDefinitionId;
                    var parameterValues;

                    if (payload != undefined) {
                        vrAlertRuleTypeId = payload.vrAlertRuleTypeId;
                        alertTypeSettings = payload.alertTypeSettings;
                        alertExtendedSettings = payload.alertExtendedSettings;


                        if (alertExtendedSettings != undefined) {
                            analysisTypeId = alertExtendedSettings.OutputItemDefinitionId;
                            parameterValues = alertExtendedSettings.ParameterValues;
                        }
                    }

                    var criteriaSectionLoadPromise = getCriteriaSectionLoadPromise();
                    promises.push(criteriaSectionLoadPromise);

                    if (analysisTypeId != undefined) {
                        var loadParametersEditorPromise = getParametersEditorPromise(analysisTypeId, parameterValues);
                        promises.push(loadParametersEditorPromise);
                    }

                    if (alertExtendedSettings != undefined) {
                        var dataRecordAlertRuleSettingsLoadPromise = getDataRecordAlertRuleSettingsLoadPromise();
                        promises.push(dataRecordAlertRuleSettingsLoadPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var criteria = criteriaDirectiveAPI.getData();

                    var parameterValues = {};
                    if (parametersEditorDirectiveAPI != undefined)
                        parametersEditorDirectiveAPI.setData(parameterValues);

                    return {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleSettings,Vanrise.Analytic.Entities",
                        FilterGroup: criteria.FilterGroup, // To be checked
                        DataAnalysisFilterGroup: criteria.DataAnalysisFilterGroup,
                        OutputItemDefinitionId: criteria.DAProfCalcOutputItemDefinitionId,
                        GroupingFieldNames: criteria.GroupingFieldNames,
                        Settings: dataRecordAlertRuleSettingsAPI.getData(),
                        MinNotificationInterval: criteria.MinNotificationInterval,
                        DAProfCalcAnalysisPeriod: criteria.DAProfCalcAnalysisPeriod,
                        DAProfCalcAlertRuleFilter: criteria.DAProfCalcAlertRuleFilter,
                        ParameterValues: parameterValues
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function getCriteriaSectionLoadPromise() {
                var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();

                criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                    var criteriapayload = {
                        dataAnalysisDefinitionId: alertTypeSettings.DataAnalysisDefinitionId,
                        rawRecordFilterLabel: alertTypeSettings.RawRecordFilterLabel,
                        daProfCalcAlertRuleFilterDefinition: alertTypeSettings.DAProfCalcAlertRuleFilterDefinition
                    };

                    if (alertExtendedSettings != undefined) {
                        criteriapayload.criteria = {
                            DAProfCalcOutputItemDefinitionId: alertExtendedSettings.OutputItemDefinitionId,
                            MinNotificationInterval: alertExtendedSettings.MinNotificationInterval,
                            DataAnalysisFilterGroup: alertExtendedSettings.DataAnalysisFilterGroup,
                            GroupingFieldNames: alertExtendedSettings.GroupingFieldNames,
                            DAProfCalcAnalysisPeriod: alertExtendedSettings.DAProfCalcAnalysisPeriod,
                            DAProfCalcAlertRuleFilter: alertExtendedSettings.DAProfCalcAlertRuleFilter,
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
                var availableDataRecordFieldNames = groupingFields != undefined ? groupingFields : getDefaultSelectedGroupingFields(outputFields);

                return {
                    recordfields: buildRecordFields(outputFields),
                    vrActionTargetType: buildDAProfCalcTargetType(analysisTypeId, availableDataRecordFieldNames),
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
                        Type: field.Type,
                        IsRequired: field.IsRequired,
                        IsSelected: field.IsSelected
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

            function getDefaultSelectedGroupingFields(fields) {
                var defaultSelectedFields = [];
                for (var i = 0; i < fields.length; i++) {
                    var field = fields[i];
                    if (field.IsSelected)
                        defaultSelectedFields.push(field.Name);
                };

                return defaultSelectedFields.length > 0 ? defaultSelectedFields : undefined;
            }

            function getParametersEditorPromise(dataAnalysisItemDefinitionId, parameterValues) {
                var loadParametersEditorPromiseDeferred = UtilsService.createPromiseDeferred();

                getParametersEditorDefinitionSettingPromise(dataAnalysisItemDefinitionId).then(function () {
                    if ($scope.scopeModel.hasOverriddenParameters) {
                        parametersEditorDirectiveReadyDeferred.promise.then(function () {
                            var parametersEditorPayload = {
                                selectedValues: parameterValues,
                                dataRecordTypeId: daProfCalcParameters.ParametersRecordTypeId,
                                definitionSettings: daProfCalcParameters.OverriddenParametersEditorDefinitionSetting,
                                runtimeEditor: daProfCalcParameters.OverriddenParametersEditorDefinitionSetting.RuntimeEditor
                            };
                            VRUIUtilsService.callDirectiveLoad(parametersEditorDirectiveAPI, parametersEditorPayload, loadParametersEditorPromiseDeferred);
                        });
                    }
                    else {
                        loadParametersEditorPromiseDeferred.resolve();
                    }
                });

                return loadParametersEditorPromiseDeferred.promise;
            }

            function getParametersEditorDefinitionSettingPromise(dataAnalysisItemDefinitionId) {
                return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId).then(function (response) {
                    $scope.scopeModel.hasOverriddenParameters = false;
                    daProfCalcParameters = undefined;

                    if (response != undefined) {
                        if (response.Settings != undefined) {
                            daProfCalcParameters = response.Settings.DAProfCalcParameters;
                        }
                    }

                    if (daProfCalcParameters != undefined && daProfCalcParameters.OverriddenParametersEditorDefinitionSetting != undefined) {
                        parametersEditorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                        $scope.scopeModel.hasOverriddenParameters = true;
                    }
                    else {
                        parametersEditorDirectiveAPI = undefined;
                    }
                });
            }
        }
    }]);