ChangePasswordController.$inject = ['$scope', 'SecurityAPIService', 'VRNotificationService'];

function ChangePasswordController($scope, SecurityAPIService, VRNotificationService) {

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }

    function defineScope() {

        $scope.ChangePassword = function () {

            var ChangedPasswordObject = {
                OldPassword: $scope.txtPasswordOld,
                NewPassword: $scope.txtPasswordNew
            }

            return SecurityAPIService.ChangePassword(ChangedPasswordObject)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's password", response)) {
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

        $scope.ConfirmPassword = function (text) {

            if ($scope.txtPasswordNew != text )
                return "Your Passwords do not match";
            else
                return null;
        };
    }

    function load() {
    }

};

appControllers.controller('Security_ChangePasswordController', ChangePasswordController);