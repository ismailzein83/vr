(function (appControllers) {

    'use strict';

    LoginController.$inject = ['$scope', 'VR_Sec_SecurityAPIService', 'SecurityService', 'VRNotificationService', 'VR_Sec_UserService', 'UISettingsService'];

    function LoginController($scope, VR_Sec_SecurityAPIService, SecurityService, VRNotificationService, VR_Sec_UserService, UISettingsService) {
        defineScope();
        load();

        function defineScope() {

            $scope.Login = login;

            $scope.forgotPassword = function () {
                VR_Sec_UserService.forgotPassword($scope.email);
            };
        }

        function load() {
        }

        function login() {
            var credentialsObject = {
                Email: $scope.email,
                Password: $scope.password
            };
            return authenticate(credentialsObject);
        }

        function onValidationNeeded(userObj) {
            var onPasswordActivated = function (password) {
                var credentialsObject = {
                    Email: $scope.email,
                    Password: password
                };

                authenticate(credentialsObject);
            };
            VR_Sec_UserService.activatePassword($scope.email, userObj, $scope.password, onPasswordActivated);
        }

        function authenticate(credentialsObject) {
            return VR_Sec_SecurityAPIService.Authenticate(credentialsObject).then(function (response) {
                if (VRNotificationService.notifyOnUserAuthenticated(response, onValidationNeeded)) {

                    var userInfo = JSON.stringify(response.AuthenticationObject);
                    SecurityService.createAccessCookie(userInfo);
                      UISettingsService.loadUISettings().then(function () {
                          if ($scope.redirectURL != undefined && $scope.redirectURL != '' && $scope.redirectURL.indexOf('default') ==-1 &&  $scope.redirectURL.indexOf('#') >-1) {
                                window.location.href = $scope.redirectURL;
                            }
                            else if (UISettingsService.getDefaultPageURl() != undefined)
                                window.location.href = UISettingsService.getDefaultPageURl();
                            else
                                window.location.href = '/';
                      });
                }
            });
        }
    }

    appControllers.controller('VR_Sec_LoginController', LoginController);

})(appControllers);
