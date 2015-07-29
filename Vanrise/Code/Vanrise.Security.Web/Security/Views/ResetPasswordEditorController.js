ResetPasswordEditorController.$inject = ['$scope', 'UsersAPIService', 'VRNavigationService', 'VRNotificationService'];

function ResetPasswordEditorController($scope, UsersAPIService, VRNavigationService, VRNotificationService) {

    var parameters;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        parameters = VRNavigationService.getParameters($scope);
    }

    function defineScope() {

        $scope.ResetPassword = function () {

            var ResetPasswordInput = {};

            if ($scope.txtPassword != $scope.txtPasswordC) {
                return "Invalid Password"
            }
            else {
                if (parameters != undefined) {

                    ResetPasswordInput = {
                        UserId: parameters.userId,
                        Password: $scope.txtPassword
                    }

                    UsersAPIService.ResetPassword(ResetPasswordInput)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's password was", response)) {
                                if ($scope.onPasswordReset != undefined)
                                    $scope.onPasswordReset(response.UpdatedObject);

                                $scope.modalContext.closeModal();
                            }
                            //if ($scope.onUserUpdated != undefined)
                            //    $scope.onUserUpdated(ResetPasswordInput);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        })
                        .finally(function () {
                            $scope.$hide();
                            //$scope.grid.itemAdded(ResetPasswordInput);
                        });
                }

            }
        };

        $scope.hide = function () {
            $scope.$hide();
        };

        $scope.ConfirmPassword = function (text) {

            if ($scope.txtPassword != text)
                return "Invalid Password";
            else
                return null;
        };
    }
    
    function load() {
    }
    

}

appControllers.controller('Security_ResetPasswordEditorController', ResetPasswordEditorController);