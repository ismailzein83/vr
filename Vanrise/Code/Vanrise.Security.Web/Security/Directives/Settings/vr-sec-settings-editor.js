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

            var passwordSettingsAPI;
            var passwordSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var apiSettingsAPI;
            var apiSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onMailMessageTemplateSettingsReady = function (api) {
                mailMessageTemplateSettingsAPI = api;
                mailMessageTemplateSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onPasswordSettingsReady = function (api) {
                passwordSettingsAPI = api;
                passwordSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onAPISettingsReady = function (api) {
                apiSettingsAPI = api;
                apiSettingsReadyPromiseDeferred.resolve();
            };
            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var mailMessageTemplateSettingsPayload;
                    var passwordSettingsPayload;
                    var apiSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        mailMessageTemplateSettingsPayload = payload.data.MailMessageTemplateSettings;
                        passwordSettingsPayload = payload.data.PasswordSettings;
                        apiSettingsPayload = payload.data.APISettings;
                        mailMessageTemplateSettingsPayload.SendEmailNewUser = payload.data.SendEmailNewUser;
                        mailMessageTemplateSettingsPayload.SendEmailOnResetPasswordByAdmin = payload.data.SendEmailOnResetPasswordByAdmin;
                        $scope.scopeModel.sessionExpirationInMinutes = payload.data.SessionExpirationInMinutes;
                    }


                    //Loading Mail Message Template Settings
                    var mailMessageTemplateSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSettingsAPI, mailMessageTemplateSettingsPayload, mailMessageTemplateSettingsLoadPromiseDeferred);
                        });

                    promises.push(mailMessageTemplateSettingsLoadPromiseDeferred.promise);

                    var passwordSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    passwordSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(passwordSettingsAPI, passwordSettingsPayload, passwordSettingsLoadPromiseDeferred);
                        });

                    promises.push(passwordSettingsLoadPromiseDeferred.promise);

                    var apiSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    apiSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(apiSettingsAPI, apiSettingsPayload, apiSettingsLoadPromiseDeferred);
                        });
                    promises.push(apiSettingsLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities",
                        MailMessageTemplateSettings: mailMessageTemplateSettingsAPI.getData(),
                        PasswordSettings: passwordSettingsAPI.getData(),
                        APISettings:apiSettingsAPI.getData(),
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