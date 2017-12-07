'use strict';

app.directive('vrSecPasswordsettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var passwordSettings = new PasswordSettings(ctrl, $scope, $attrs);
                passwordSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/PasswordSettingsTemplate.html"
        };

        function PasswordSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var passwordComplexityAPI;
            var passwordComplexityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mailMessageTemplateSelectorAPI;
            var mailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onPasswordComplexitySelectorReady = function (api) {
                passwordComplexityAPI = api;
                passwordComplexityReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.passwordValueCustomValidation = function () {
                if ($scope.scopeModel.passwordLength >= $scope.scopeModel.maxPasswordLength)
                    return "Min password length should be less then Max password length.";
                return null;
            };

            $scope.scopeModel.onMailMessageTemplateDirectiveReady = function (api) {
                mailMessageTemplateSelectorAPI = api;
                mailMessageTemplateSelectorReadyDeferred.resolve();
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
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.passwordLength = payload.PasswordLength;
                        $scope.scopeModel.maxPasswordLength = payload.MaxPasswordLength;
                        $scope.scopeModel.maxUserLoginTries = payload.MaximumUserLoginTries == 0 ? undefined : payload.MaximumUserLoginTries;
                        $scope.scopeModel.maxUserPasswordHistoryCount = payload.UserPasswordHistoryCount == 0 ? undefined : payload.UserPasswordHistoryCount;
                        $scope.scopeModel.lockInterval = payload.FailedInterval;
                        $scope.scopeModel.lockFor = payload.MinutesToLock == 0 ? undefined : payload.MinutesToLock;
                        $scope.scopeModel.notificationMailTemplateId = payload.NotificationMailTemplateId;
                        $scope.scopeModel.sendNotification = payload.NotificationMailTemplateId != undefined;
                    }

                    var promises = [];

                    function loadPasswordSection() {

                        var passwordComplexityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        var passwordComplexityPayload = {
                            selectedIds: payload != undefined && payload.PasswordComplexity || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(passwordComplexityAPI, passwordComplexityPayload, passwordComplexityLoadPromiseDeferred);
                        return passwordComplexityLoadPromiseDeferred.promise;
                    }

                    function loadMailMessageTypeSelector() {
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
                        return mailMessageTemplateSelectorLoadPromiseDeferred.promise;
                    }

                    promises.push(loadPasswordSection());
                    promises.push(loadMailMessageTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        PasswordLength: $scope.scopeModel.passwordLength,
                        MaxPasswordLength: $scope.scopeModel.maxPasswordLength,
                        PasswordComplexity: passwordComplexityAPI.getSelectedIds(),
                        MaximumUserLoginTries: $scope.scopeModel.maxUserLoginTries,
                        UserPasswordHistoryCount: $scope.scopeModel.maxUserPasswordHistoryCount,
                        FailedInterval: $scope.scopeModel.lockInterval,
                        MinutesToLock: $scope.scopeModel.lockFor,
                        NotificationMailTemplateId: $scope.scopeModel.sendNotification ? mailMessageTemplateSelectorAPI.getSelectedIds() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);