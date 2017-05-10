(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {

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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountType/Templates/OverriddenConfigurationAccountType.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var filter;
            var extendedSettings;
            var accountTypeSettings;
            var selectedIds;
            var accountTypeEntity;
            var accountTypeSelectorApi;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
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
                $scope.scopeModel.accountTypeSelectorSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.title = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenSettings = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.showDirectiveSettings = false;
                        }
                    }

                };
                $scope.scopeModel.isSettingsOverridden = false;
                $scope.scopeModel.onAccountTypeSettingsReady = function (api) {
                    settingsAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsAPI, undefined, setLoader, settingReadyPromiseDeferred);
                };
                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorApi = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };
                accountTypeSelectorPromiseDeferred.promise.then(function () {
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
                        selectedIds = extendedSettings.AccountTypeId;
                        overriddenSettings = extendedSettings.OverriddenSettings;  
                        $scope.scopeModel.title = extendedSettings.OverriddenTitle;
                        $scope.scopeModel.isSettingsOverridden = (overriddenSettings != undefined) ? true : false;
                        
                    }
                    var loadAccountTypeSelectorPromise = loadAccountTypeSelector();
                    

                    function loadAccountTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return accountTypeSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined
                    });

                    if ($scope.scopeModel.isSettingsOverridden) {
                      var  accountTypeSettingsloadPromiseDeferred = UtilsService.createPromiseDeferred();
                        loadAccountTypeSelectorPromise.then(function () {
                            loadOverriddenSettingsEditor().then(function () { accountTypeSettingsloadPromiseDeferred.resolve(); });
                        });
                        promises.push(accountTypeSettingsloadPromiseDeferred.promise);
                    }
                    else {
                       
                        promises.push(loadAccountTypeSelectorPromise);
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };
                directiveAPI.getData = function () {
                    var settings;
                    if (settingsAPI != undefined)
                    { settings = settingsAPI.getData(); }
                    var accountTypeOverriddenConfiguration = {};
                    accountTypeOverriddenConfiguration.$type = " Retail.BusinessEntity.Business.AccountTypeOverriddenConfiguration , Retail.BusinessEntity.Business";
                    accountTypeOverriddenConfiguration.ConfigId = '';
                    accountTypeOverriddenConfiguration.AccountTypeId = accountTypeSelectorApi.getSelectedIds();
                    accountTypeOverriddenConfiguration.OverriddenTitle = $scope.scopeModel.title;
                    accountTypeOverriddenConfiguration.OverriddenSettings = settings;
                      
                    return accountTypeOverriddenConfiguration;
                };

                return directiveAPI;
            }

            function loadOverriddenSettingsEditor() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                if (overriddenSettings == undefined) {

                    getAccountType().then(function () {
                        accountTypeSettings = accountTypeEntity.Settings;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    accountTypeSettings = overriddenSettings;
                    loadSettings();
                }

                function loadSettings() {
                 
                    var accountBEDefinitionId;
                    $scope.scopeModel.showDirectiveSettings = true;
                    getAccountType().then(function () {
                        accountBEDefinitionId = accountTypeEntity.AccountBEDefinitionId;
                        settingReadyPromiseDeferred.promise
                       .then(function () {
                           var directivePayload = {
                               AccountBEDefinitionId: accountBEDefinitionId,
                               accountTypeSettings: accountTypeSettings
                           };
                           VRUIUtilsService.callDirectiveLoad(settingsAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                       }).catch(function (error) {
                           loadSettingDirectivePromiseDeferred.reject();
                       });
                    });

                }
                return loadSettingDirectivePromiseDeferred.promise;
            }
            function hideOverriddenSettingsEditor() {
                $scope.scopeModel.showDirectiveSettings = false;
                settingsAPI = undefined;
            }
            function getAccountType() {
                return Retail_BE_AccountTypeAPIService.GetAccountType(accountTypeSelectorApi.getSelectedIds()).then(function (response) {
                    accountTypeEntity = response;
                });
            }
        }


        return directiveDefinitionObject;
    }

    app.directive('retailBeOverriddenconfigurationAccounttype', OverriddenSettings);

})(app);
