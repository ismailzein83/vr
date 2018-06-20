(function (appControllers) {

    'use strict';

    ChangePasswordController.$inject = ['$scope', 'VR_Sec_SecurityAPIService', 'VRNotificationService', 'Sec_CookieService'];

    function ChangePasswordController($scope, VR_Sec_SecurityAPIService, VRNotificationService, Sec_CookieService) {

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
        }

        function defineScope() {
            $scope.passwordHint = "";

            $scope.save = function () {
                var changedPasswordObject = {
                    OldPassword: $scope.oldPassword,
                    NewPassword: $scope.newPassword
                };

                return VR_Sec_SecurityAPIService.ChangePassword(changedPasswordObject).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password", response)) {
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
                if ($scope.newPassword != $scope.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };
        }

        function load() {
            return loadPasswordHint();
        }

        function loadPasswordHint() {
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo(Sec_CookieService.getLoggedInUserInfo().SecurityProviderId).then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
            });
        }
    };

    appControllers.controller('VR_Sec_ChangePasswordController', ChangePasswordController);

})(appControllers);
