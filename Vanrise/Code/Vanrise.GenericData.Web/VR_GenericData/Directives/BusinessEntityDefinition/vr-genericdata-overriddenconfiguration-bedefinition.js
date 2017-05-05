(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {

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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/OverriddenBEDefinition.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var beDefinitionSettings;
            var selectedIds;
            var businessEntityDefinitionEntity;
            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.bEDefinitionSelectedValue;
                $scope.scopeModel.loadSettings = function () {
                    loadOverriddenSettingsEditor();
                };
                $scope.scopeModel.businessEntityDefinitionSelectorSelectionChanged = function (value) {
                    if (value != undefined)
                    {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.businessEntityTitle = "";
                            $scope.scopeModel.showSettings = false;
                            overriddenSettings = undefined;
                            loadOverriddenSettingsEditor();
                        }
                    }
                };
                $scope.scopeModel.showSettings = false;
                $scope.scopeModel.onSettingsEditorReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };
                beDefinitionSelectorPromiseDeferred.promise.then(function () {
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
                         selectedIds = extendedSettings.BusinessEntityDefinitionId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.businessEntityTitle = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.showSettings = (extendedSettings.OverriddenSettings != undefined) ? true : false;
                        promises.push(loadOverriddenSettingsEditor());
                    }
                   

                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                            var payloadSelector = {
                                selectedIds: selectedIds,
                                filter: filter
                            };
                            return beDefinitionSelectorApi.load(payloadSelector);
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
                    var beDefinitionOverriddenConfiguration = {};
                    beDefinitionOverriddenConfiguration.$type = "Vanrise.GenericData.Business.BEDefinitionOverriddenConfiguration ,Vanrise.GenericData.Business";
                    beDefinitionOverriddenConfiguration.ConfigId = '';
                    beDefinitionOverriddenConfiguration.BusinessEntityDefinitionId = beDefinitionSelectorApi.getSelectedIds();
                    beDefinitionOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.businessEntityTitle;
                    beDefinitionOverriddenConfiguration.OverriddenSettings = settings;
                    return beDefinitionOverriddenConfiguration;
                }

                return directiveAPI;
            }
            
            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                if ($scope.scopeModel.showSettings == true) {
                    settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    if (overriddenSettings == undefined) {
                        getBEDefinitionSettingConfigs().then(
                            function () {
                                getBusinessEntityDefinition().then(function () {

                                    beDefinitionSettings = businessEntityDefinitionEntity.Settings;
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
                        getBEDefinitionSettingConfigs().then(function () {
                            beDefinitionSettings = overriddenSettings;
                            loadSettings();
                        }).catch(function (error) {
                            loadSettingDirectivePromiseDeferred.reject();
                        });

                    }
                    
                }
                else {
                    $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                    settingsAPI = undefined;
                    loadSettingDirectivePromiseDeferred.resolve();
                }
                function loadSettings() {

                    if (beDefinitionSettings != undefined) {

                        $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.bEDefinitionSettingConfigs, beDefinitionSettings.ConfigId, "ExtensionConfigurationId");
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {
                                    businessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                                    businessEntityDefinitionSettings: beDefinitionSettings,
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });
                    }
                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function getBEDefinitionSettingConfigs() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDefinitionSettingConfigs().then(function (response) {
                    if (response) {

                        $scope.scopeModel.bEDefinitionSettingConfigs = response;
                    }
                });
            }
            function getBusinessEntityDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(beDefinitionSelectorApi.getSelectedIds()).then(function (businessEntityDefinition) {
                    businessEntityDefinitionEntity = businessEntityDefinition;
                });
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataOverriddenconfigurationBedefinition', OverriddenSettings);

})(app);
