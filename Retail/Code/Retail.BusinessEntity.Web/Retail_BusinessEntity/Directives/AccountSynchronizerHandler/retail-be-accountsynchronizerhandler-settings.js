(function (app) {

    'use strict';

    AccountSynchronizerHandlerSettingsDirective.$inject = ['Retail_BE_AccountSynchronizerHandlerAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AccountSynchronizerHandlerSettingsDirective(Retail_BE_AccountSynchronizerHandlerAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var accSyncCtrl = this;
                var ctor = new AccountSynchronizerHandlerSettingsCtor($scope, accSyncCtrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "accSyncCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountSynchronizerHandler/Templates/AccountSynchronizerHandlerSettingsTemplate.html"
        };

        function AccountSynchronizerHandlerSettingsCtor($scope, accSyncCtrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var accountSynchronizerHandlerSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountSynchronizerHandlerSettings = payload.Settings;
                    }

                    var accountSynchronizerHandlerSettingsTemplateConfigsLoadPromise = getAccountSynchronizerHandlerSettingsTemplateConfigs();
                    promises.push(accountSynchronizerHandlerSettingsTemplateConfigsLoadPromise);

                    if (accountSynchronizerHandlerSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getAccountSynchronizerHandlerSettingsTemplateConfigs() {
                        return Retail_BE_AccountSynchronizerHandlerAPIService.GetAccountSynchronizerInsertHandlerConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (accountSynchronizerHandlerSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, accountSynchronizerHandlerSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                Settings: accountSynchronizerHandlerSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (accSyncCtrl.onReady != null) {
                    accSyncCtrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountsynchronizerhandlerSettings', AccountSynchronizerHandlerSettingsDirective);

})(app);