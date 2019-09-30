'use strict';

app.directive('vrSecSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var securitySettingsEditor = new SecuritySettingsEditor(ctrl, $scope, $attrs);
                securitySettingsEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/SecuritySettingsTemplate.html"
        };

        function SecuritySettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSettingsAPI;
            var mailMessageTemplateSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var securityProviderSettings;

            //var passwordSettingsAPI;
            //var passwordSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var apiSettingsAPI;
            var apiSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var passwordComplexityAPI;
            var passwordComplexityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mailMessageTemplateSelectorAPI;
            var mailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var loggingModuleFilterAPI;
            var loggingModuleFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            $scope.scopeModel.onPasswordComplexitySelectorReady = function (api) {
                passwordComplexityAPI = api;
                passwordComplexityReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onMailMessageTemplateDirectiveReady = function (api) {
                mailMessageTemplateSelectorAPI = api;
                mailMessageTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onMailMessageTemplateSettingsReady = function (api) {
                mailMessageTemplateSettingsAPI = api;
                mailMessageTemplateSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAPISettingsReady = function (api) {
                apiSettingsAPI = api;
                apiSettingsReadyPromiseDeferred.resolve();
            };


            $scope.scopeModel.passwordValueCustomValidation = function () {
                if (parseInt($scope.scopeModel.passwordLength) >= parseInt($scope.scopeModel.maxPasswordLength))
                    return "Min password length should be less then Max password length.";
                return null;
            };

            $scope.scopeModel.passwordAgeCustomValidation = function () {
                if (!$scope.scopeModel.passwordAgeInDays)
                    return null;
                if (!$scope.scopeModel.expirationDaysToNotify)
                    return null;
                if (parseInt($scope.scopeModel.expirationDaysToNotify) >= parseInt($scope.scopeModel.passwordAgeInDays))
                    return "Expiration days to notify should be less then password age in days.";
                return null;
            };

            $scope.scopeModel.validateLockInterval = function () {
                if ($scope.scopeModel.maxUserLoginTries > 0)
                    return true;
                return false;
            };

            $scope.scopeModel.onLoggingModuleFilterReady = function (api) {
                loggingModuleFilterAPI = api;
                loggingModuleFilterReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onEnableLoggingValueChanged = function () {
                if ($scope.scopeModel.enableLogging) {
                    loggingModuleFilterReadyPromiseDeferred.promise.then(function () {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingLoggingModuleFilterDirective = value;
                        };
                        var payload = {};
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, loggingModuleFilterAPI, payload, setLoader);
                    });
                }
                else {
                    resetLoggingSwitches();
                    loggingModuleFilterAPI = undefined;
                    loggingModuleFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                }
            };

            function initializeController() {
                passwordComplexityReadyPromiseDeferred.promise.then(function () {
                    defineAPI();
                });
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var mailMessageTemplateSettingsPayload;
                    var passwordSettingsPayload;
                    var apiSettingsPayload;
                    var loggingModuleFilter;
                    if (payload != undefined && payload.data != undefined) {
                        mailMessageTemplateSettingsPayload = payload.data.MailMessageTemplateSettings;
                        passwordSettingsPayload = payload.data.PasswordSettings;
                        securityProviderSettings = payload.data.SecurityProviderSettings;
                        var securityProviderId = payload.data.SecurityProviderSettings;
                        apiSettingsPayload = payload.data.APISettings;
                        mailMessageTemplateSettingsPayload.SendEmailNewUser = payload.data.SendEmailNewUser;
                        mailMessageTemplateSettingsPayload.SendEmailOnResetPasswordByAdmin = payload.data.SendEmailOnResetPasswordByAdmin;
                        $scope.scopeModel.sessionExpirationInMinutes = payload.data.SessionExpirationInMinutes;
                        if (passwordSettingsPayload != undefined) {
                            $scope.scopeModel.passwordLength = passwordSettingsPayload.PasswordLength;
                            $scope.scopeModel.maxPasswordLength = passwordSettingsPayload.MaxPasswordLength;
                            $scope.scopeModel.maxUserLoginTries = passwordSettingsPayload.MaximumUserLoginTries == 0 ? undefined : passwordSettingsPayload.MaximumUserLoginTries;
                            $scope.scopeModel.maxUserPasswordHistoryCount = passwordSettingsPayload.UserPasswordHistoryCount == 0 ? undefined : passwordSettingsPayload.UserPasswordHistoryCount;
                            $scope.scopeModel.lockInterval = passwordSettingsPayload.FailedInterval;
                            $scope.scopeModel.lockFor = passwordSettingsPayload.MinutesToLock == 0 ? undefined : passwordSettingsPayload.MinutesToLock;
                            $scope.scopeModel.notificationMailTemplateId = passwordSettingsPayload.NotificationMailTemplateId;
                            $scope.scopeModel.sendNotification = passwordSettingsPayload.NotificationMailTemplateId != undefined;
                            $scope.scopeModel.passwordAgeInDays = passwordSettingsPayload.PasswordAgeInDays;
                            $scope.scopeModel.expirationDaysToNotify = passwordSettingsPayload.PasswordExpirationDaysToNotify;
                        }
                        var logSettings = payload.data.LogSettings;
                        if (logSettings != undefined) {
                            $scope.scopeModel.enableLogging = logSettings.EnableLogging;
                            $scope.scopeModel.enableParametersLogging = logSettings.EnableParametersLogging;
                            $scope.scopeModel.enableRequestHeaderLogging = logSettings.EnableRequestHeaderLogging;
                            $scope.scopeModel.enableRequestBodyLogging = logSettings.EnableRequestBodyLogging;
                            $scope.scopeModel.enableResponseHeaderLogging = logSettings.EnableResponseHeaderLogging;
                            $scope.scopeModel.enableResponseBodyLogging = logSettings.EnableResponseBodyLogging;
                            loggingModuleFilter = logSettings.Filter;
                        }
                    }


                    //Loading Mail Message Template Settings
                    var mailMessageTemplateSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSettingsAPI, mailMessageTemplateSettingsPayload, mailMessageTemplateSettingsLoadPromiseDeferred);
                        });

                    promises.push(mailMessageTemplateSettingsLoadPromiseDeferred.promise);


                    var apiSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    apiSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(apiSettingsAPI, apiSettingsPayload, apiSettingsLoadPromiseDeferred);
                        });
                    promises.push(apiSettingsLoadPromiseDeferred.promise);



                    var passwordComplexityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var passwordComplexityPayload = {
                        selectedIds: passwordSettingsPayload != undefined && passwordSettingsPayload.PasswordComplexity || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(passwordComplexityAPI, passwordComplexityPayload, passwordComplexityLoadPromiseDeferred);
                    promises.push(passwordComplexityLoadPromiseDeferred.promise);

                    var mailMessageTemplateSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSelectorReadyDeferred.promise.then(function () {
                        var mailMessageTemplateSelectorPayload = {
                            filter: {
                                VRMailMessageTypeId: '3b45aed6-1094-48f4-b3e5-099858909949'
                            },
                            selectedIds: $scope.scopeModel.notificationMailTemplateId
                        };
                        VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSelectorAPI, mailMessageTemplateSelectorPayload, mailMessageTemplateSelectorLoadPromiseDeferred);
                    });
                    promises.push(mailMessageTemplateSelectorLoadPromiseDeferred.promise);

                    //Loading Logging Module Filter
                    if ($scope.scopeModel.enableLogging) {
                        var loggingModuleFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        loggingModuleFilterReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                moduleFilterSettings: loggingModuleFilter
                            };
                            VRUIUtilsService.callDirectiveLoad(loggingModuleFilterAPI, payload, loggingModuleFilterLoadPromiseDeferred);
                        });
                        promises.push(loggingModuleFilterLoadPromiseDeferred.promise);
                    }

                    return UtilsService.waitMultiplePromises(promises);


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities",
                        MailMessageTemplateSettings: mailMessageTemplateSettingsAPI.getData(),
                        SecurityProviderSettings: securityProviderSettings, //securityProviderSettings remained due to backward compatibility
                        PasswordSettings: {
                            PasswordLength: $scope.scopeModel.passwordLength,
                            MaxPasswordLength: $scope.scopeModel.maxPasswordLength,
                            PasswordComplexity: passwordComplexityAPI.getSelectedIds(),
                            MaximumUserLoginTries: $scope.scopeModel.maxUserLoginTries,
                            UserPasswordHistoryCount: $scope.scopeModel.maxUserPasswordHistoryCount,
                            FailedInterval: $scope.scopeModel.lockInterval,
                            MinutesToLock: $scope.scopeModel.lockFor,
                            NotificationMailTemplateId: $scope.scopeModel.sendNotification ? mailMessageTemplateSelectorAPI.getSelectedIds() : undefined,
                            PasswordAgeInDays: $scope.scopeModel.passwordAgeInDays,
                            PasswordExpirationDaysToNotify: $scope.scopeModel.expirationDaysToNotify
                        },
                        APISettings: apiSettingsAPI.getData(),
                        SendEmailNewUser: mailMessageTemplateSettingsAPI.getSendEmailNewUser(),
                        SendEmailOnResetPasswordByAdmin: mailMessageTemplateSettingsAPI.getSendEmailOnResetPasswordByAdmin(),
                        SessionExpirationInMinutes: $scope.scopeModel.sessionExpirationInMinutes,
                        LogSettings: {
                            EnableLogging: $scope.scopeModel.enableLogging,
                            EnableParametersLogging: $scope.scopeModel.enableParametersLogging,
                            EnableRequestHeaderLogging: $scope.scopeModel.enableRequestHeaderLogging,
                            EnableRequestBodyLogging: $scope.scopeModel.enableRequestBodyLogging,
                            EnableResponseHeaderLogging: $scope.scopeModel.enableResponseHeaderLogging,
                            EnableResponseBodyLogging: $scope.scopeModel.enableResponseBodyLogging,
                            Filter: $scope.scopeModel.enableLogging ? loggingModuleFilterAPI.getData() : undefined
                        }
                    };
                };



                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function resetLoggingSwitches() {
                $scope.scopeModel.enableParametersLogging = false;
                $scope.scopeModel.enableRequestHeaderLogging = false;
                $scope.scopeModel.enableRequestBodyLogging = false;
                $scope.scopeModel.enableResponseHeaderLogging = false;
                $scope.scopeModel.enableResponseBodyLogging = false;
            }
        }
    }]);