(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRObjectTypeDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VRCommon_VRObjectTypeDefinitionAPIService) {

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
            templateUrl: '/Client/Modules/Common/Directives/VRObjectTypeDefinition/Templates/OverriddenConfigurationObjectTypeDefinition.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var extendedSettings;
            var objectTypeDefinitionSettings;
            var selectedIds;
            var objectTypeDefinitionEntity;
            var objectTypeDefinitionSelectorApi;
            var objectTypeDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true)
                    { loadOverriddenSettingsEditor(); }
                    else
                    { hideOverriddenSettingsEditor(); }
                };
                $scope.scopeModel.ObjectTypeDefinitionSelectorSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.name = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenSettings = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                            settingsAPI = undefined;
                        }
                    }

                };
                $scope.scopeModel.isSettingsOverridden = false;
                $scope.scopeModel.onObjectTypeDefinitionSettingsReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onObjectTypeDefinitionSelectorReady = function (api) {
                    objectTypeDefinitionSelectorApi = api;
                    objectTypeDefinitionSelectorPromiseDeferred.resolve();
                };
                objectTypeDefinitionSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.VRObjectTypeDefinitionId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.name = extendedSettings.OverriddenName;
                        $scope.scopeModel.isSettingsOverridden = (overriddenSettings != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadOverriddenSettingsEditor());
                        }
                    }

                    promises.push(loadObjectTypeDefinitionSelector());

                    function loadObjectTypeDefinitionSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return objectTypeDefinitionSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData();}
                    var objectTypeDefinitionOverriddenConfiguration = {};
                    objectTypeDefinitionOverriddenConfiguration.$type = " Vanrise.Common.Business.VRObjectTypeDefinitionOverriddenConfiguration , Vanrise.Common.Business";
                    objectTypeDefinitionOverriddenConfiguration.ConfigId = '';
                    objectTypeDefinitionOverriddenConfiguration.VRObjectTypeDefinitionId = objectTypeDefinitionSelectorApi.getSelectedIds();
                    objectTypeDefinitionOverriddenConfiguration.OverriddenName = $scope.scopeModel.name;
                    objectTypeDefinitionOverriddenConfiguration.OverriddenSettings = settings;
                    return objectTypeDefinitionOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                if (overriddenSettings == undefined) {

                    getVRObjectTypeDefinition().then(function () {
                        objectTypeDefinitionSettings = objectTypeDefinitionEntity.Settings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    objectTypeDefinitionSettings = overriddenSettings;
                    loadSettings();
                }

                function loadSettings() {

                    $scope.scopeModel.showDirectiveSettings = true;
                    settingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {

                                vrObjectTypeDefinitionSettings: objectTypeDefinitionSettings
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
            function getVRObjectTypeDefinition() {
                return VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinition(objectTypeDefinitionSelectorApi.getSelectedIds()).then(function (objectTypedefinition) {
                    objectTypeDefinitionEntity = objectTypedefinition;
                });
            }
        }


        return directiveDefinitionObject;
    }

    app.directive('vrCommonOverriddenconfigurationObjecttypedefinition', OverriddenSettings);

})(app);
