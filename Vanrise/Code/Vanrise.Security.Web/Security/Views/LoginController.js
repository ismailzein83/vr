(function (appControllers) {

    'use strict';

    LoginController.$inject = ['$scope', 'VR_Sec_SecurityAPIService', 'SecurityService', 'VRNotificationService', 'VR_Sec_UserService'];

    function LoginController($scope, VR_Sec_SecurityAPIService, SecurityService, VRNotificationService, VR_Sec_UserService) {
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
                    if ($scope.redirectURL != undefined && $scope.redirectURL != '')
                        window.location.href = $scope.redirectURL;
                    else
                        window.location.href = '/';
                }
            });
        }
    }

    appControllers.controller('VR_Sec_LoginController', LoginController);

})(appControllers);
