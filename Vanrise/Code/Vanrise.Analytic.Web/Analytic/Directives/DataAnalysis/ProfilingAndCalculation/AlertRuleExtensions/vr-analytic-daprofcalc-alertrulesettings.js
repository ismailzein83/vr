
'use strict';

app.directive('vrAnalyticDaprofcalcAlertrulesettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var vRActionManagementAPI;
            var vRActionManagementReadyDeferred = UtilsService.createPromiseDeferred();

            var alertExtendedSettings;
            var alertTypeSettings;
            var vrAlertRuleTypeId;

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onVRActionManagementDirectiveReady = function (api) {
                    vRActionManagementAPI = api;
                    vRActionManagementReadyDeferred.resolve();
                };

                $scope.scopeModel.onCriteriaSelectionChanged = function (daProfCalcOutputItemDefinition) {
                    $scope.scopeModel.selectedAnalysisTypeId = daProfCalcOutputItemDefinition.DataAnalysisItemDefinitionId;
                    loadVRActionManagement($scope.scopeModel.selectedAnalysisTypeId, undefined);
                };
                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        alertTypeSettings = payload.alertTypeSettings;
                        alertExtendedSettings = payload.alertExtendedSettings;
                        vrAlertRuleTypeId = payload.vrAlertRuleTypeId;

                        var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadCriteriaSectionPromiseDeferred.promise);

                        criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                            var criteriapayload = { dataAnalysisDefinitionId: alertTypeSettings.DataAnalysisDefinitionId, rawRecordFilterLabel: alertTypeSettings.RawRecordFilterLabel };

                            if (alertExtendedSettings != undefined) {
                                criteriapayload.criteria = {
                                    DAProfCalcOutputItemDefinitionId: alertExtendedSettings.OutputItemDefinitionId, FilterGroup: alertExtendedSettings.FilterGroup, DataAnalysisFilterGroup: alertExtendedSettings.DataAnalysisFilterGroup, GroupingFieldNames: alertExtendedSettings.GroupingFieldNames
                                };
                                $scope.scopeModel.selectedAnalysisTypeId = alertExtendedSettings.OutputItemDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriapayload, loadCriteriaSectionPromiseDeferred);
                        });

                        if (alertExtendedSettings != undefined) {
                            var loadVRActionManagementLoadDeferred = loadVRActionManagement(alertExtendedSettings.OutputItemDefinitionId, alertExtendedSettings.Actions);
                            promises.push(loadVRActionManagementLoadDeferred.promise);
                        }
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
                        GroupingFieldNames:criteria.GroupingFieldNames,
                        Actions: vRActionManagementAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function loadVRActionManagement(analysisTypeId, actions) {
                var vRActionManagementLoadDeferred = UtilsService.createPromiseDeferred();
                vRActionManagementReadyDeferred.promise.then(function () {
                    var vrActionPayload = {
                        extensionType: alertTypeSettings.VRActionExtensionType,
                        actions: actions,
                        isRequired: true,
                        vrAlertRuleTypeId: vrAlertRuleTypeId,
                        vrActionTargetType: buildDAProfCalcTargetType(analysisTypeId)
                    };
                    VRUIUtilsService.callDirectiveLoad(vRActionManagementAPI, vrActionPayload, vRActionManagementLoadDeferred);
                });
                return vRActionManagementLoadDeferred;
            };

            function buildDAProfCalcTargetType(selectedAnalysisTypeId) {
                return {
                    $type: "Vanrise.Analytic.Business.DAProfCalcActionTargetType,Vanrise.Analytic.Business",
                    DataAnalysisItemDefinitionId: selectedAnalysisTypeId
                };
            };
        }
    }]);