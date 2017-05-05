(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_GenericRuleDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var overriddenSettingsDirective = new OverriddenSettingsDirective(ctrl, $scope, $attrs);
                overriddenSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRuleDefinition/Templates/OverriddenGenericRuleDefinition.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenCriteriaDefinition;
            var overriddenObjects;
            var overriddenSettingsDefinition;
            var overriddenSecurity;
            var filter;
            var extendedSettings;
            var genericRuleDefinitionCriteriaDefinition;
            var genericRuleDefinitionObjects;
            var genericRuleDefinitionSettingsDefinition;
            var genericRuleDefinitionSecurity;
            var selectedIds;
            var genericRuleEntityDefinitionEntity;
            var genericRuleDefinitionSelectorApi;
            var genericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden==true)
                    {
                        loadOverriddenSettingsEditor();
                     }
                    else{
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.genericRuleDefinitionSelectorSelectionChanged = function () {
                    if (selectedIds != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.title = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenCriteriaDefinition = undefined;
                            overriddenObjects = undefined;
                            overriddenSettingsDefinition = undefined;
                            overriddenSecurity = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                        }
                    }
                       
                };
                $scope.scopeModel.isSettingsOverridden = false;
                $scope.scopeModel.onGenericRuleDefinfitionSettingsReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onGenericRuleDefinitionSelectorReady = function (api) {
                    genericRuleDefinitionSelectorApi = api;
                    genericRuleDefinitionSelectorPromiseDeferred.resolve();
                };
                genericRuleDefinitionSelectorPromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    var promises = [];
                    if (payload) {
                        extendedSettings = payload.extendedSettings;
                        selectedIds = extendedSettings.RuleDefinitionId;
                        overriddenCriteriaDefinition = extendedSettings.OverriddenCriteriaDefinition;
                        overriddenObjects = extendedSettings.OverriddenObjects;
                        overriddenSettingsDefinition = extendedSettings.OverriddenSettingsDefinition;
                        overriddenSecurity = extendedSettings.OverriddenSecurity;
                        $scope.scopeModel.title = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.isSettingsOverridden = (overriddenCriteriaDefinition != undefined || overriddenObjects != undefined || overriddenSettingsDefinition != undefined || overriddenSecurity != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden)
                        {
                            promises.push(loadOverriddenSettingsEditor());
                        }
                    }

                    promises.push(loadGenericRuleDefinitionSelector());

                    function loadGenericRuleDefinitionSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return genericRuleDefinitionSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var genericRuleDefinitionOverriddenConfiguration = {};
                    genericRuleDefinitionOverriddenConfiguration.$type = "Vanrise.GenericData.Business.GenericRuleDefinitionOverriddenConfiguration ,Vanrise.GenericData.Business";
                    genericRuleDefinitionOverriddenConfiguration.ConfigId = '';
                    genericRuleDefinitionOverriddenConfiguration.RuleDefinitionId = genericRuleDefinitionSelectorApi.getSelectedIds();
                    genericRuleDefinitionOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.title;
                    if (settings != undefined) {
                        genericRuleDefinitionOverriddenConfiguration.OverriddenCriteriaDefinition = settings.criteriaDefinition;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenObjects = settings.objects;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenSettingsDefinition = settings.settingsDefinition;
                        genericRuleDefinitionOverriddenConfiguration.OverriddenSecurity = settings.security;
                    }

                    return genericRuleDefinitionOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                 
                    if (overriddenCriteriaDefinition == undefined && overriddenObjects == undefined && overriddenSettingsDefinition == undefined && overriddenSecurity == undefined)
                    {
                       
                        getGenericRuleDefinition().then(function () {
                            genericRuleDefinitionCriteriaDefinition = genericRuleEntityDefinitionEntity.CriteriaDefinition;
                            genericRuleDefinitionObjects = genericRuleEntityDefinitionEntity.Objects;
                            genericRuleDefinitionSettingsDefinition = genericRuleEntityDefinitionEntity.SettingsDefinition;
                            genericRuleDefinitionSecurity = genericRuleEntityDefinitionEntity.Security;
                            loadSettings();
                        }).catch(function (error) {
                            loadSettingDirectivePromiseDeferred.reject();
                        });
                    }
                    else {
                        genericRuleDefinitionCriteriaDefinition = overriddenCriteriaDefinition;
                        genericRuleDefinitionObjects = overriddenObjects;
                        genericRuleDefinitionSettingsDefinition = overriddenSettingsDefinition;
                        genericRuleDefinitionSecurity = overriddenSecurity;
                            loadSettings();
                    }
                
                function loadSettings() {

                        $scope.scopeModel.showDirectiveSettings = true;
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {

                                    objects: genericRuleDefinitionObjects,
                                    criteriaDefinition: genericRuleDefinitionCriteriaDefinition,
                                    settingsDefinition: genericRuleDefinitionSettingsDefinition,
                                    security: genericRuleDefinitionSecurity
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });

                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.showDirectiveSettings = false;
                settingsAPI = undefined;
            }
            function getGenericRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionSelectorApi.getSelectedIds()).then(function (genericRuleEntityDefinition) {
                    genericRuleEntityDefinitionEntity = genericRuleEntityDefinition;
                });
            }
        }


        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataOverriddenconfigurationGenericruledefinition', OverriddenSettings);

})(app);
