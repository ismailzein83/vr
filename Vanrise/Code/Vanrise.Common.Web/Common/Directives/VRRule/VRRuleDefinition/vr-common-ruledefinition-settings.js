'use strict';

app.directive('vrCommonRuledefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRRuleDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRRule/VRRuleDefinition/Templates/VRRuleDefinitionSettingsTemplate.html'
        };

        function VRRuleDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrRuleDefinitionExtendedSettingsDirectiveAPI;
            var vrRuleDefinitionExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRRuleDefinitionExtendedSettingsDirectiveReady = function (api) {
                    vrRuleDefinitionExtendedSettingsDirectiveAPI = api;
                    vrRuleDefinitionExtendedSettingsDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrRuleDefinitionSettings, vrRuleDefinitionExtendedSettings;

                    if (payload != undefined) {
                        var vrRuleDefinitionEntity = payload.componentType;

                        if (vrRuleDefinitionEntity != undefined) {
                            $scope.scopeModel.name = vrRuleDefinitionEntity.Name;
                            vrRuleDefinitionSettings = vrRuleDefinitionEntity.Settings;
                        }

                        if (vrRuleDefinitionSettings != undefined) {
                            vrRuleDefinitionExtendedSettings = vrRuleDefinitionSettings.VRRuleDefinitionExtendedSettings;
                        }
                    }

                    //Loading ParentBEDefinitionRuntimeSelectorSettings selector
                    var vrRuleDefinitionExtendedSettingsDirectiveLoadPromise = getVRRuleDefinitionExtendedSettingsDirectiveLoadPromise();
                    promises.push(vrRuleDefinitionExtendedSettingsDirectiveLoadPromise);


                    function getVRRuleDefinitionExtendedSettingsDirectiveLoadPromise() {
                        var vrRuleDefinitionExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        vrRuleDefinitionExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                            var vrRuleDefinitionExtendedSettingsDirectivePayload = {
                                vrRuleDefinitionExtendedSettings: vrRuleDefinitionExtendedSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(vrRuleDefinitionExtendedSettingsDirectiveAPI, vrRuleDefinitionExtendedSettingsDirectivePayload, vrRuleDefinitionExtendedSettingsDirectiveLoadDeferred);
                        });

                        return vrRuleDefinitionExtendedSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Entities.VRRuleDefinitionSettings, Vanrise.Entities",
                            VRRuleDefinitionExtendedSettings: vrRuleDefinitionExtendedSettingsDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
