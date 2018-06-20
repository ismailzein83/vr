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

            var securityProviderSettingsAPI;
            var securityProviderSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //var passwordSettingsAPI;
            //var passwordSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var apiSettingsAPI;
            var apiSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var passwordComplexityAPI;
            var passwordComplexityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mailMessageTemplateSelectorAPI;
            var mailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();


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

            $scope.scopeModel.onSecurityProviderSettingsReady = function (api) {
                securityProviderSettingsAPI = api;
                securityProviderSettingsReadyPromiseDeferred.resolve();
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
                    var securityProviderSettingsPayload;
                    var apiSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        mailMessageTemplateSettingsPayload = payload.data.MailMessageTemplateSettings;
                        passwordSettingsPayload = payload.data.PasswordSettings;
                        securityProviderSettingsPayload = payload.data.SecurityProviderSettings;
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
                    }


                    //Loading Mail Message Template Settings
                    var mailMessageTemplateSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSettingsAPI, mailMessageTemplateSettingsPayload, mailMessageTemplateSettingsLoadPromiseDeferred);
                        });

                    promises.push(mailMessageTemplateSettingsLoadPromiseDeferred.promise);

                    //Loading Security Provider Settings
                    var securityProviderSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    securityProviderSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(securityProviderSettingsAPI, securityProviderSettingsPayload, securityProviderSettingsLoadPromiseDeferred);
                        });

                    promises.push(securityProviderSettingsLoadPromiseDeferred.promise);

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



                    return UtilsService.waitMultiplePromises(promises);


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities",
                        MailMessageTemplateSettings: mailMessageTemplateSettingsAPI.getData(),
                        SecurityProviderSettings: securityProviderSettingsAPI.getData(),
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
                        SessionExpirationInMinutes: $scope.scopeModel.sessionExpirationInMinutes
                    };
                };



                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);