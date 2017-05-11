(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailMessageTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VRCommon_VRMailMessageTypeAPIService) {

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
            templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/OverriddenConfigurationVRMailType.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var extendedSettings;
            var mailMessageTypeSettings;
            var selectedIds;
            var mailMessageTypeEntity;
            var mailMessageTypeSelectorApi;
            var mailMessageTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
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
                }
                $scope.scopeModel.mailMessageTypeSelectorSelectionChanged = function (value) {
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
                $scope.scopeModel.onObjectDirectiveReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeSelectorApi = api;
                    mailMessageTypeSelectorPromiseDeferred.resolve();
                };
                mailMessageTypeSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.VRMailMessageTypeId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.name = extendedSettings.OverriddenName;
                        $scope.scopeModel.isSettingsOverridden = (overriddenSettings != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadOverriddenSettingsEditor());
                        }
                    }

                    promises.push(loadMailMessageTypeSelector());

                    function loadMailMessageTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return mailMessageTypeSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings={ Objects:settingsAPI.getData()}; }
                    var mailMessageTypeOverriddenConfiguration = {};
                    mailMessageTypeOverriddenConfiguration.$type = " Vanrise.Common.Business.VRMailMessageTypeOverriddenConfiguration , Vanrise.Common.Business";
                    mailMessageTypeOverriddenConfiguration.ConfigId = '';
                    mailMessageTypeOverriddenConfiguration.VRMailMessageTypeId = mailMessageTypeSelectorApi.getSelectedIds();
                    mailMessageTypeOverriddenConfiguration.OverriddenName = $scope.scopeModel.name;
                    mailMessageTypeOverriddenConfiguration.OverriddenSettings = settings;
                    return mailMessageTypeOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                if (overriddenSettings == undefined) {

                    getMailMessageType().then(function () {
                        mailMessageTypeSettings = mailMessageTypeEntity.Settings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    mailMessageTypeSettings = overriddenSettings;
                    loadSettings();
                }

                function loadSettings() {

                    $scope.scopeModel.showDirectiveSettings = true;
                    settingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {

                                objects: mailMessageTypeSettings.Objects
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
            function getMailMessageType() {
                return VRCommon_VRMailMessageTypeAPIService.GetMailMessageType(mailMessageTypeSelectorApi.getSelectedIds()).then(function (mailMessageType) {
                    mailMessageTypeEntity = mailMessageType;
                });
            }
        }


        return directiveDefinitionObject;
    } 

    app.directive('vrCommonOverriddenconfigurationMailtype', OverriddenSettings);

})(app);
