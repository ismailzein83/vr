(function (appControllers) {
    'use strict';

    changeExpiredPasswordEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'VR_Sec_SecurityAPIService', 'SecurityService'];

    function changeExpiredPasswordEditorController($scope, VRNavigationService, VRNotificationService, VR_Sec_SecurityAPIService, SecurityService) {
        var email;
        var userObj;
        var oldPassword;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                email = parameters.email;
                oldPassword = parameters.oldPassword;
            }
        }

        function defineScope() {
            $scope.passwordHint = "";
            $scope.save = function () {
                var changeExpiredPasswordObject = {
                    Email: email,
                    Password: $scope.password,
                    OldPassword: oldPassword
                };

                return VR_Sec_SecurityAPIService.ChangeExpiredPassword(changeExpiredPasswordObject).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                        if ($scope.onExpiredPasswordChanged && typeof $scope.onExpiredPasswordChanged == 'function') {
                            $scope.onExpiredPasswordChanged($scope.password);
                        }

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.validatePasswords = function () {
                if ($scope.password != $scope.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };
        }

        function load() {
            return loadPasswordHint().then(function () {
                setTitle();
            });
            
        }

        function setTitle() {
            $scope.title = 'Change Expired Password';
        }

        function loadPasswordHint() {
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo().then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
            });
        }
    }

    appControllers.controller('VR_Sec_ChangeExpiredPasswordEditorController', changeExpiredPasswordEditorController);

})(appControllers);
