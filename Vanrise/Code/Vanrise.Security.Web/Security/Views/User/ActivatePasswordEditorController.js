(function (appControllers) {
    'use strict';

    ActivatePasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService', 'VR_Sec_SecurityAPIService', 'SecurityService'];

    function ActivatePasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService, VR_Sec_SecurityAPIService, SecurityService) {
        var email;
        var userObj;
        var tempPassword;
        var securityProviderId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                email = parameters.email;
                tempPassword = parameters.tempPassword;
                securityProviderId = parameters.securityProviderId;
            }
        }

        function defineScope() {
            $scope.passwordHint = "";
            $scope.save = function () {
                var activatePasswordInput = {
                    Email: email,
                    Password: $scope.password,
                    TempPassword : tempPassword
                };

                return VR_Sec_UserAPIService.ActivatePassword(activatePasswordInput).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                        if ($scope.onPasswordActivated && typeof $scope.onPasswordActivated == 'function') {
                            $scope.onPasswordActivated($scope.password);
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
            $scope.title = 'Activate Password';
        }

        function loadPasswordHint() {
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo(securityProviderId).then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
            });
        }
    }

    appControllers.controller('VR_Sec_ActivatePasswordEditorController', ActivatePasswordEditorController);

})(appControllers);
