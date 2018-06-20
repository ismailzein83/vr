(function (appControllers) {
    'use strict';

    ResetPasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService', 'VR_Sec_SecurityAPIService'];

    function ResetPasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService, VR_Sec_SecurityAPIService) {
        var userId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                userId = parameters.userId;
            }
        }

        function defineScope() {
            $scope.save = function () {
                var resetPasswordInput = {
                    UserId: userId,
                    Password: $scope.password
                };

                return VR_Sec_UserAPIService.ResetPassword(resetPasswordInput).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                        if ($scope.onPasswordReset && typeof $scope.onPasswordReset == 'function') {
                            $scope.onPasswordReset(response);
                        }

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.hasSaveResetPasswordPermission = function () {
                return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
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
            $scope.isloading = true;

            VR_Sec_UserAPIService.GetUserbyId(userId).then(function (user) {
                loadPasswordHint(user.SecurityProviderId).then(function () {
                    setTitle();
                    $scope.isloading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                    $scope.isloading = false;
                });
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.isloading = false;
            });
        }

        function setTitle() {
            $scope.title = 'Reset Password';
        }
        function loadPasswordHint(securityProviderId) {
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo(securityProviderId).then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
            });
        }

    }

    appControllers.controller('VR_Sec_ResetPasswordEditorController', ResetPasswordEditorController);

})(appControllers);
