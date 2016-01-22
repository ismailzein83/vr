(function (appControllers) {
    'use strict';

    ResetPasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService'];

    function ResetPasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService) {
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
            setTitle();
        }

        function setTitle()
        {
            $scope.title = 'Reset Password';
        }
    }

    appControllers.controller('VR_Sec_ResetPasswordEditorController', ResetPasswordEditorController);

})(appControllers);
