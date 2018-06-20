(function (appControllers) {

    'use strict';

    LoginController.$inject = ['$rootScope', '$scope', 'VR_Sec_SecurityAPIService', 'SecurityService', 'VRNotificationService', 'VR_Sec_UserService', 'UISettingsService', 'UtilsService', 'VR_Sec_UserAPIService', 'VRLocalizationService', 'VR_Sec_SecurityProviderAPIService', 'VRUIUtilsService'];

    function LoginController($rootScope, $scope, VR_Sec_SecurityAPIService, SecurityService, VRNotificationService, VR_Sec_UserService, UISettingsService, UtilsService, VR_Sec_UserAPIService, VRLocalizationService, VR_Sec_SecurityProviderAPIService, VRUIUtilsService) {

        var securityProviderSelectorApi;
        var securityProviderSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        var authenticateUserEditorApi;

        defineScope();
        load();

        function defineScope() {
            $scope.showSecurityProviderSelector = true;

            $scope.onSecurityProviderSelectorReady = function (api) {
                securityProviderSelectorApi = api;
                securityProviderSelectorPromiseDeferred.resolve();
            };

            $scope.onAuthenticateUserEditorReady = function (api) {
                authenticateUserEditorApi = api;
                var setLoader = function (value) {
                    $scope.isLoadingAuthenticateUserEditor = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, authenticateUserEditorApi, undefined, setLoader);
            };

            $scope.Login = login;
            $rootScope.onValidationMessageShown = function (e) {
                var self = angular.element(e.currentTarget);
                var selfHeight = $(self).height();
                var TophasLable = $(self).parent().attr('label') != undefined ? 0 : (($(self).parents('.dropdown-container2').length > 0)) ? -10 : -15;
                var topVar = ($(self).parents('.dropdown-container2').length > 0) ? (selfHeight / 3) - 5 : (selfHeight / 3);
                var selfWidth = $(self).width();
                var selfOffset = $(self).offset();
                var elleft = selfOffset.left - $(window).scrollLeft() + $(self).width();
                var left = 0;
                var tooltip = self.parent().find('.tooltip-error');
                if (innerWidth - elleft < 100) {
                    elleft = elleft - (100 + $(self).width() + 10);
                    $(tooltip).addClass('tooltip-error-right');
                    $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft, width: 100 });
                }
                else {
                    $(tooltip).removeClass('tooltip-error-right');
                    $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft });
                }
                e.stopPropagation();
            };

            $scope.forgotPassword = function () {
                VR_Sec_UserService.forgotPassword($scope.email);
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function loadSecurityProviderSelector() {

                var securityProviderSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                securityProviderSelectorPromiseDeferred.promise.then(function () {
                    var securityProviderPayload = { selectfirstitem: true };
                    VRUIUtilsService.callDirectiveLoad(securityProviderSelectorApi, securityProviderPayload, securityProviderSelectorLoadDeferred);
                });
                return securityProviderSelectorLoadDeferred.promise.then(function () {
                    $scope.showSecurityProviderSelector = !securityProviderSelectorApi.hasSingleItem();
                });
            }

            return UtilsService.waitMultipleAsyncOperations([loadSecurityProviderSelector]).then(function () { }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function login(password) {
            var authenticateUserEditorPayload = authenticateUserEditorApi.getData();
            if (password != undefined)
                authenticateUserEditorPayload.Password = password;

            var reloginAfterPasswordActivation = function (passwordAfterActivation) {
                login(passwordAfterActivation);
            };

            var loginPromisedeferred = UtilsService.createPromiseDeferred();

            return SecurityService.authenticate(securityProviderSelectorApi.getSelectedIds(), authenticateUserEditorPayload, reloginAfterPasswordActivation).then(function () {
                var promises = [];
                promises.push(UISettingsService.loadUISettings());
                promises.push(getLoggedInUserLanguage());

                UtilsService.waitMultiplePromises(promises).then(function () {
                    if ($scope.redirectURL != undefined && $scope.redirectURL != '' && $scope.redirectURL.indexOf('default') == -1 && $scope.redirectURL.indexOf('#') > -1) {
                        window.location.href = $scope.redirectURL;
                    }
                    else if (UISettingsService.getDefaultPageURl() != undefined)
                        window.location.href = UISettingsService.getDefaultPageURl();
                    else
                        window.location.href = '/';

                    loginPromisedeferred.resolve();
                }).catch(function (error) {
                    loginPromisedeferred.reject(error);
                });
            }).catch(function (error) {
                loginPromisedeferred.reject(error);
            });

            function getLoggedInUserLanguage() {
                return VR_Sec_UserAPIService.GetLoggedInUserLanguageId().then(function (response) {
                    VRLocalizationService.createOrUpdateLanguageCookie(response);
                });
            }
            return loginPromisedeferred.promise;
        }
    }

    appControllers.controller('VR_Sec_LoginController', LoginController);

})(appControllers);
