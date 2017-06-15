(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Notification_VRAlertRuleTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_Notification_VRAlertRuleTypeAPIService) {

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
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/OverriddenAlertRuleTypeDefinition.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var ruleTypeSettings;
            var selectedIds;
            var ruleTypeEntityDefinitionEntity;
            var ruleTypeSelectorApi;
            var ruleTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;

            var viewPermissionAPI;
            var viewPermissionReadyDeferred;

            var addPermissionAPI;
            var addPermissionReadyDeferred;

            var editPermissionAPI;
            var editPermissionReadyDeferred;

            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.ruleTypeSelectedValue;
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadOverriddenSettingsEditor();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.ruleTypeSelectorSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.title = "";
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
                $scope.scopeModel.onRuleTypeSelectorReady = function (api) {
                    ruleTypeSelectorApi = api;
                    ruleTypeSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;

                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, viewPermissionAPI, undefined, setLoader, viewPermissionReadyDeferred);
                    //viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, addPermissionAPI, undefined, setLoader, addPermissionReadyDeferred);
                };
                $scope.scopeModel.onEditRequiredPermissionReady = function (api) {
                    editPermissionAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, editPermissionAPI, undefined, setLoader, editPermissionReadyDeferred);
                };

                ruleTypeSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.RuleTypeId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.title = extendedSettings.OverriddenName;
                        $scope.scopeModel.isSettingsOverridden = (extendedSettings.OverriddenSettings != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadOverriddenSettingsEditor());
                        }
                    }


                    promises.push(loadRuleTypeSelector());

                    function loadRuleTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return ruleTypeSelectorApi.load(payloadSelector);
                    }
                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined) {
                        settings = settingsAPI.getData();
                        settings.Security = {
                            ViewPermission: viewPermissionAPI.getData(),
                            AddPermission: addPermissionAPI.getData(),
                            EditPermission: editPermissionAPI.getData()
                        }
                    }
                    var ruleTypeOverriddenConfiguration = {};
                    ruleTypeOverriddenConfiguration.$type = "Vanrise.Notification.Business.VRAlertRuleTypeOverriddenConfiguration ,Vanrise.Notification.Business";
                    ruleTypeOverriddenConfiguration.ConfigId = '';
                    ruleTypeOverriddenConfiguration.RuleTypeId = ruleTypeSelectorApi.getSelectedIds();
                    ruleTypeOverriddenConfiguration.OverriddenName = $scope.scopeModel.title;
                    ruleTypeOverriddenConfiguration.OverriddenSettings = settings;
                    return ruleTypeOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();
                addPermissionReadyDeferred = UtilsService.createPromiseDeferred();
                editPermissionReadyDeferred = UtilsService.createPromiseDeferred();

                if (overriddenSettings == undefined) {
                    getVRAlertRuleTypeSettingsExtensionConfigs().then(
                        function () {
                            getVRAlertRuleType().then(function () {
                                ruleTypeSettings = ruleTypeEntityDefinitionEntity.Settings;
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
                    getVRAlertRuleTypeSettingsExtensionConfigs().then(function () {
                        ruleTypeSettings = overriddenSettings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });

                }

                function loadSettings() {
                    var promises = [];
                    $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.ruleTypeSettingConfigs, ruleTypeSettings.ConfigId, "ExtensionConfigurationId");
                    settingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                settings: ruleTypeSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                        });
                    promises.push(loadSettingDirectivePromiseDeferred.promise);
                    promises.push(loadViewRequiredPermission());
                    promises.push(loadAddRequiredPermission());
                    promises.push(loadEditRequiredPermission());
                   
                    return UtilsService.waitMultiplePromises(promises);
                }

                function loadViewRequiredPermission() {
                    var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    viewPermissionReadyDeferred.promise.then(function () {
                        var dataPayload = {
                            data: ruleTypeSettings && ruleTypeSettings.Security && ruleTypeSettings.Security.ViewPermission || undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                    });
                    return viewSettingPermissionLoadDeferred.promise;
                }

                function loadAddRequiredPermission() {
                    var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    addPermissionReadyDeferred.promise.then(function () {
                        var dataPayload = {
                            data: ruleTypeSettings && ruleTypeSettings.Security && ruleTypeSettings.Security.AddPermission || undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(addPermissionAPI, dataPayload, addPermissionLoadDeferred);
                    });
                    return addPermissionLoadDeferred.promise;
                }

                function loadEditRequiredPermission() {
                    var editPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                    editPermissionReadyDeferred.promise.then(function () {
                        var dataPayload = {
                            data: ruleTypeSettings && ruleTypeSettings.Security && ruleTypeSettings.Security.EditPermission || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(editPermissionAPI, dataPayload, editPermissionLoadDeferred);
                    });
                    return editPermissionLoadDeferred.promise;
                }


               

                return loadSettingDirectivePromiseDeferred.promise;
            }

            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                settingsAPI = undefined;
            }

            function getVRAlertRuleTypeSettingsExtensionConfigs() {

                return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleTypeSettingsExtensionConfigs().then(function (response) {

                    if (response) {

                        $scope.scopeModel.ruleTypeSettingConfigs = response;
                    }
                });
            }
            function getVRAlertRuleType() {
                return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(ruleTypeSelectorApi.getSelectedIds()).then(function (response) {
                    ruleTypeEntityDefinitionEntity = response;
                });
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrNotificationOverriddenconfigurationAlertruletype', OverriddenSettings);

})(app);
