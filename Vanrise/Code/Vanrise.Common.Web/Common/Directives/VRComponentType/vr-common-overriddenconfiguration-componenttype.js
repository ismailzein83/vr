(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRComponentTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VRCommon_VRComponentTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
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
            templateUrl: '/Client/Modules/Common/Directives/VRComponentType/Templates/OverriddenConfigurationComponentType.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var componenttypeSettings;
            var selectedIds;
            var componentTypeEntity;
            var componentTypeSelectorApi;
            var componentTypePromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.selectedSetingsTypeConfig;
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadOverriddenSettingsEditor();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.componentTypeSelectorSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.name = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenSettings = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                        }
                    }
                };
                $scope.scopeModel.isSettingsOverridden = false;
                $scope.scopeModel.onSettingsEditorReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onComponentTypeSelectorReady = function (api) {
                    componentTypeSelectorApi = api;
                    componentTypePromiseDeferred.resolve();
                };
                componentTypePromiseDeferred.promise.then(function () {
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
                        var extendedSettings = payload.extendedSettings;
                        selectedIds = extendedSettings.VRComponentTypeId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.name = extendedSettings.OverriddenName;
                        $scope.scopeModel.isSettingsOverridden = (extendedSettings.OverriddenSettings != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadOverriddenSettingsEditor());
                        }
                    }


                    promises.push(loadComponentTypeSelector());

                    function loadComponentTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return componentTypeSelectorApi.load(payloadSelector);
                    }
                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData().Settings; }
                    var componentTypeOverriddenConfiguration = {};
                    componentTypeOverriddenConfiguration.$type = "Vanrise.Common.Business.VRComponentTypeOverriddenConfiguration ,Vanrise.Common.Business";
                    componentTypeOverriddenConfiguration.ConfigId = '';
                    componentTypeOverriddenConfiguration.VRComponentTypeId = componentTypeSelectorApi.getSelectedIds();
                    componentTypeOverriddenConfiguration.OverriddenName = $scope.scopeModel.name;
                    componentTypeOverriddenConfiguration.OverriddenSettings = settings;
                    return componentTypeOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                if (overriddenSettings == undefined) {
                    getVRComponentTypeExtensionConfigs().then(
                        function () {
                            getVRComponentType().then(function () {

                                componenttypeSettings = componentTypeEntity.Settings;
                                loadSettings();
                            }).catch(function (error) {
                                loadSettingDirectivePromiseDeferred.reject();
                            });
                        }
                        ).catch(function (error) {
                            loadSettingDirectivePromiseDeferred.reject();
                        });
                }
                else {
                    getVRComponentTypeExtensionConfigs().then(function () {
                        componenttypeSettings = overriddenSettings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });

                }

                function loadSettings() {
                    
                    $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.componentTypeSettingConfigs, componenttypeSettings.VRComponentTypeConfigId, "ExtensionConfigurationId");
                  
                    settingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                componentType: {
                                    VRComponentTypeId: componentTypeSelectorApi.getSelectedIds(),
                                    Name: $scope.scopeModel.name,
                                    Settings: componenttypeSettings
                                },
                                extensionConfigId: componenttypeSettings.VRComponentTypeConfigId
                            };
                            
                            VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                        });
                }
                return loadSettingDirectivePromiseDeferred.promise;
            }

            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                settingsAPI = undefined;
            }

            function getVRComponentTypeExtensionConfigs() {
                return VRCommon_VRComponentTypeAPIService.GetVRComponentTypeExtensionConfigs().then(function (response) {
                    if (response) {

                        $scope.scopeModel.componentTypeSettingConfigs = response;
                    }
                });
            }
            function getVRComponentType() {
                return VRCommon_VRComponentTypeAPIService.GetVRComponentType(componentTypeSelectorApi.getSelectedIds()).then(function (response) {
                    componentTypeEntity = response;
                });
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonOverriddenconfigurationComponenttype', OverriddenSettings);

})(app);
