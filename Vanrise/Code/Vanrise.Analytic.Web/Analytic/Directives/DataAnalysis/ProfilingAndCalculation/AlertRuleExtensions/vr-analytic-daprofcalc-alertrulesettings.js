
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

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        alertTypeSettings = payload.alertTypeSettings;
                        alertExtendedSettings = payload.alertExtendedSettings;

                        var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadCriteriaSectionPromiseDeferred.promise);

                        criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                            var criteriapayload = { dataAnalysisDefinitionId: alertTypeSettings.DataAnalysisDefinitionId };

                            if (alertExtendedSettings != undefined) {
                                criteriapayload.criteria = { DAProfCalcOutputItemDefinitionId: alertExtendedSettings.OutputItemDefinitionId, FilterGroup: alertExtendedSettings.FilterGroup };
                            }

                            VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriapayload, loadCriteriaSectionPromiseDeferred);
                        });

                        var loadVRActionManagementLoadDeferred = loadVRActionManagement();
                        promises.push(loadVRActionManagementLoadDeferred.promise);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var criteria = criteriaDirectiveAPI.getData();
                    return {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleSettings,Vanrise.Analytic.Entities",
                        FilterGroup: criteria.FilterGroup,
                        OutputItemDefinitionId: criteria.DAProfCalcOutputItemDefinitionId,
                        Actions: vRActionManagementAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function loadVRActionManagement() {
                var vRActionManagementLoadDeferred = UtilsService.createPromiseDeferred();
                vRActionManagementReadyDeferred.promise.then(function () {
                    var vrActionPayload = { extensionType: alertTypeSettings.VRActionExtensionType, actions: alertExtendedSettings != undefined ? alertExtendedSettings.Actions : undefined, isRequired: true };
                    VRUIUtilsService.callDirectiveLoad(vRActionManagementAPI, vrActionPayload, vRActionManagementLoadDeferred);
                });
                return vRActionManagementLoadDeferred;
            };
        }
    }]);