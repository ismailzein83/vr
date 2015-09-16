ResetPasswordEditorController.$inject = ['$scope', 'UsersAPIService', 'VRNavigationService', 'VRNotificationService'];

function ResetPasswordEditorController($scope, UsersAPIService, VRNavigationService, VRNotificationService) {

    var userId = undefined;

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

            return UsersAPIService.ResetPassword(ResetPasswordInput)
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

        $scope.ConfirmPassword = function (text) {

            if ($scope.txtPassword != text)
                return "Your Passwords do not match";
            else
                return null;
        };
    }
    
    function load() {
    }
    
}

appControllers.controller('Security_ResetPasswordEditorController', ResetPasswordEditorController);