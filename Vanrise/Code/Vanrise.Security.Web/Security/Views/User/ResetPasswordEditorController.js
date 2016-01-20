(function (appControllers) {
    "use strict";

    ResetPasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService'];

    function ResetPasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService) {

        var userId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                userId = parameters.userId;
        }

        function defineScope() {

            $scope.ResetPassword = function () {

                var ResetPasswordInput = {
                    UserId: userId,
                    Password: $scope.txtPassword
                }

                return VR_Sec_UserAPIService.ResetPassword(ResetPasswordInput)
                    .then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                            if ($scope.onPasswordReset != undefined)
                                $scope.onPasswordReset(response);

                            $scope.modalContext.closeModal();
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });

            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.ConfirmPassword = function () {

                if ($scope.txtPassword != $scope.txtPasswordConfirmed)
                    return "Your Passwords do not match";
                else
                    return null;
            };
        }

        function load() {
            setTitle();
        }

        function setTitle()
        {
            $scope.title = "Reset Password";
        }

    }

    appControllers.controller('VR_Sec_ResetPasswordEditorController', ResetPasswordEditorController);

})(appControllers);

