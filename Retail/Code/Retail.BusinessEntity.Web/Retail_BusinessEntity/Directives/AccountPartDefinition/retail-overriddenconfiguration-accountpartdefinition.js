(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountPartDefinitionAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, Retail_BE_AccountPartDefinitionAPIService) {

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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountPartDefinition/Templates/OverriddenConfigurationAccountPartDefinition.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var extendedSettings;
            var accountPartDefinitionSettings;
            var selectedIds;
            var accountPartDefinitionEntity;
            var accountPartDefinitionSelectorApi;
            var accountPartDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var settingsAPI;
            var settingReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.loadSettings = function () {
                    loadOverriddenSettingsEditor();
                };
                $scope.scopeModel.accountPartDefinitionSelectorSelectionChanged = function () {
                    if (selectedIds != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.accountPartDefinitionTitle = "";
                            $scope.scopeModel.showSettings = false;
                            overriddenSettings = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                            loadOverriddenSettingsEditor();
                        }
                    }

                };
                $scope.scopeModel.showSettings = false;
                $scope.scopeModel.accountPartDefinitionDirectiveReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onAccountPartDefinitionSelectorReady = function (api) {
                    accountPartDefinitionSelectorApi = api;
                    accountPartDefinitionSelectorPromiseDeferred.resolve();
                };
                accountPartDefinitionSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.AccountPartDefinitionId;
                        overriddenSettings = extendedSettings.OverriddenSettings;
                        $scope.scopeModel.accountPartDefinitionTitle = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.showSettings = (overriddenSettings != undefined) ? true : false;
                        promises.push(loadOverriddenSettingsEditor());
                    }

                    promises.push(loadAccountPartDefinitionSelector());

                    function loadAccountPartDefinitionSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return accountPartDefinitionSelectorApi.load(payloadSelector);
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
                    var accountPartDefinitionOverriddenConfiguration = {};
                    accountPartDefinitionOverriddenConfiguration.$type = "Retail.BusinessEntity.Business.AccountPartDefinitionOverriddenConfiguration ,Retail.BusinessEntity.Business";
                    accountPartDefinitionOverriddenConfiguration.ConfigId = '';
                    accountPartDefinitionOverriddenConfiguration.AccountPartDefinitionId = accountPartDefinitionSelectorApi.getSelectedIds();
                    accountPartDefinitionOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.accountPartDefinitionTitle;
                    accountPartDefinitionOverriddenConfiguration.OverriddenSettings = settings;
                    return accountPartDefinitionOverriddenConfiguration;
                }

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                if ($scope.scopeModel.showSettings == true) {

                    settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    if (overriddenSettings==undefined) {

                        getAccountPartDefinition().then(function () {
                            accountPartDefinitionSettings = accountPartDefinitionEntity.Settings;
                            loadSettings();
                        }).catch(function (error) {
                            loadSettingDirectivePromiseDeferred.reject();
                        });
                    }
                    else {
                        accountPartDefinitionSettings = overriddenSettings;
                        loadSettings();
                    }
                }
                else {
                    settingsAPI = undefined;
                    $scope.scopeModel.showDirectiveSettings = false;
                    loadSettingDirectivePromiseDeferred.resolve();
                }
                function loadSettings() {

                    if (accountPartDefinitionSettings!=undefined) {

                        $scope.scopeModel.showDirectiveSettings = true;
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {

                                    partDefinitionSettings: accountPartDefinitionSettings
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });

                    }
                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function getAccountPartDefinition() {
                return Retail_BE_AccountPartDefinitionAPIService.GetAccountPartDefinition(accountPartDefinitionSelectorApi.getSelectedIds()).then(function (accountPartDefinition) {
                    accountPartDefinitionEntity = accountPartDefinition;
                });
            }
        }


        return directiveDefinitionObject;
    }

    app.directive('retailOverriddenconfigurationAccountpartdefinition', OverriddenSettings);

})(app);
