(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_SettingsAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VRCommon_SettingsAPIService) {

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
            templateUrl: "/Client/Modules/Common/Directives/Settings/Templates/OverriddenConfigurationSettings.html"
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenData;
            var filter;
            var extendedSettings;
            var settings;
            var selectedIds;
            var settingEntity;
            var settingSelectorApi;
            var settingSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadOverriddenSettingsEditor();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.settingsSelectorSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                            console.log("test1");
                        }
                        else {
                            $scope.scopeModel.name = "";
                            $scope.scopeModel.category = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenData = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                            console.log("test2");
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
                $scope.scopeModel.onSettingsSelectorReady = function (api) {
                    settingSelectorApi = api;
                    settingSelectorPromiseDeferred.resolve();
                };
                settingSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.SettingId;
                        overriddenData = extendedSettings.OverriddenData;
                        $scope.scopeModel.category = extendedSettings.OverriddenCategory;
                        $scope.scopeModel.name = extendedSettings.OverriddenName;
                        $scope.scopeModel.isSettingsOverridden = (overriddenData != undefined) ? true : false;
                    }
                    var loadSettingSelectorPromise = loadSettingSelector();


                    function loadSettingSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return settingSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined;
                    });

                    if ($scope.scopeModel.isSettingsOverridden) {
                        var settingsloadPromiseDeferred = UtilsService.createPromiseDeferred();
                        loadSettingSelectorPromise.then(function () {
                            loadOverriddenSettingsEditor().then(function () { settingsloadPromiseDeferred.resolve(); });
                        });
                        promises.push(settingsloadPromiseDeferred.promise);
                    }
                    else {

                        promises.push(loadSettingSelectorPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var settingsOverriddenConfiguration = {};
                    settingsOverriddenConfiguration.$type = " Vanrise.Common.Business.SettingOverriddenConfiguration , Vanrise.Common.Business";
                    settingsOverriddenConfiguration.ConfigId = '';
                    settingsOverriddenConfiguration.SettingId = settingSelectorApi.getSelectedIds();
                    settingsOverriddenConfiguration.OverriddenName = $scope.scopeModel.name;
                    settingsOverriddenConfiguration.OverriddenCategory = $scope.scopeModel.category;
                    settingsOverriddenConfiguration.OverriddenData = settings;

                    return settingsOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                if (overriddenData == undefined) {
                    getSetting().then(function () {
                        settings = settingEntity.Data;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                        
                    
                        
                }
                else {
                    getSetting().then(function () {
                        settings = overriddenData;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                   
                }

                function loadSettings() {

                    $scope.scopeModel.showDirectiveSettings = true;
                    settingReadyPromiseDeferred.promise
                       .then(function () {
                           var directivePayload = {
                               data: settings
                           };
                           VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                       }).catch(function (error) {
                           loadSettingDirectivePromiseDeferred.reject();
                       });
                    
                       

                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.showDirectiveSettings = false;
                settingsAPI = undefined;
            }
            function getSetting() {
                return VRCommon_SettingsAPIService.GetSetting(settingSelectorApi.getSelectedIds()).then(function (response) {
                    $scope.settingEditor = response.Settings.Editor;
                    settingEntity = response;
                });
            }
        }


        return directiveDefinitionObject;
    }
   
    app.directive('vrCommonOverriddenconfigurationSettings', OverriddenSettings);

})(app);
