'use strict';

app.directive('vrAccountbalanceNotificationtypeSettings', ['UtilsService', 'VRUIUtilsService', 'VR_AccountBalance_AccountBalanceNotificationTypeAPIService',
    function (UtilsService, VRUIUtilsService, VR_AccountBalance_AccountBalanceNotificationTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBalanceNotificationTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationTypeSettingsTemplate.html'
        };

        function AccountBalanceNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

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
                    var directivePayload = {};

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var accountColumnHeader;
                    var accountBalanceNotificationTypeExtendedSettings;

                    if (payload != undefined) {
                        accountColumnHeader = payload.AccountColumnHeader;
                        accountBalanceNotificationTypeExtendedSettings = payload.AccountBalanceNotificationTypeExtendedSettings;
                    }

                    $scope.scopeModel.accountColumnHeader = accountColumnHeader;

                    var loadAccountBalanceNotificationTypeExtendedSettingsConfigs = getAccountBalanceNotificationTypeExtendedSettingsConfigs();
                    promises.push(loadAccountBalanceNotificationTypeExtendedSettingsConfigs);

                    if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getAccountBalanceNotificationTypeExtendedSettingsConfigs() {
                        return VR_AccountBalance_AccountBalanceNotificationTypeAPIService.GetAccountBalanceNotificationTypeExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, accountBalanceNotificationTypeExtendedSettings.ConfigId, 'ExtensionConfigurationId');
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
                                accountBalanceNotificationTypeExtendedSettings: accountBalanceNotificationTypeExtendedSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var extendedSettings;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        extendedSettings = directiveAPI.getData();
                        if (extendedSettings != undefined) {
                            extendedSettings.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }

                    var obj = {
                        $type: 'Vanrise.AccountBalance.Business.AccountBalanceNotificationTypeSettings, Vanrise.AccountBalance.Business',
                        AccountBalanceNotificationTypeExtendedSettings: extendedSettings,
                        AccountColumnHeader: $scope.scopeModel.accountColumnHeader
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);