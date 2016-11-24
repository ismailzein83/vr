
'use strict';

app.directive('vrNotificationVrbalancealertruleSettings', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/VR_Notification/Directives/MainExtensions/VRBalanceAlertRule/Templates/VRBalanceAlertRuleSettingsTemplate.html'
        };

        function AlertRuleSettings($scope, ctrl, $attrs) {

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var vrAlertRuleSettingsDirectiveAPI;
            var vrAlertRuleSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var criteriaDefinitionFields;
            var criteriaFieldsValues;

            var extensionType;

            var alertExtendedSettings;
            var alertTypeSettings;

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onVRAlertRuleSettingsDirectiveReady = function (api) {
                    vrAlertRuleSettingsDirectiveAPI = api;
                    vrAlertRuleSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined) {
                        criteriaDefinitionFields = payload.alertTypeSettings.CriteriaDefinition.Fields;
                        extensionType = payload.alertTypeSettings.VRActionExtensionType;

                        alertExtendedSettings = payload.alertExtendedSettings;
                        alertTypeSettings = payload.alertTypeSettings;
                    }

                    if (alertExtendedSettings != undefined && alertExtendedSettings.Criteria != undefined) {
                        criteriaFieldsValues = alertExtendedSettings.Criteria.FieldsValues;
                    }

                    var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCriteriaSectionPromiseDeferred.promise);


                    criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            criteriaDefinitionFields: criteriaDefinitionFields,
                            criteriaFieldsValues: criteriaFieldsValues
                        };
                        VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, payload, loadCriteriaSectionPromiseDeferred);
                    });


                    var loadRuleSettingsSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadRuleSettingsSectionPromiseDeferred.promise);


                    vrAlertRuleSettingsDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            settings: alertExtendedSettings,
                            alertTypeSettings: alertTypeSettings
                        };
                        VRUIUtilsService.callDirectiveLoad(vrAlertRuleSettingsDirectiveAPI, payload, loadRuleSettingsSectionPromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Notification.Entities.VRBalanceAlertRuleSettings,Vanrise.Notification.Entities",
                        Criteria: criteriaDirectiveAPI.getData(),
                        ThresholdActions: vrAlertRuleSettingsDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

