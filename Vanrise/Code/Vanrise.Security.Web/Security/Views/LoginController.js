(function (appControllers) {

    'use strict';

    LoginController.$inject = ['$scope', 'VR_Sec_SecurityAPIService', 'SecurityService', 'VRNotificationService'];

    function LoginController($scope, VR_Sec_SecurityAPIService, SecurityService, VRNotificationService) {
        defineScope();
        load();

        function defineScope() {

            $scope.Login = login;
        }

        function load() {
        }

        function login() {
            var credentialsObject = {
                Email: $scope.email,
                Password: $scope.password
            };

            return VR_Sec_SecurityAPIService.Authenticate(credentialsObject).then(function (response) {
                if (VRNotificationService.notifyOnUserAuthenticated(response)) {
                    var userInfo = JSON.stringify(response.AuthenticationObject);
                    SecurityService.createAccessCookie(userInfo);
                    window.location.href = '/';
                }
            });
        }
    }

    appControllers.controller('VR_Sec_LoginController', LoginController);

})(appControllers);
