ResetPasswordEditorController.$inject = ['$scope', 'UsersAPIService', 'VRNavigationService'];
function ResetPasswordEditorController($scope, UsersAPIService, VRNavigationService) {

    var parameters = VRNavigationService.getParameters($scope);

    if (parameters != undefined) {
        //$scope.txtPassword = parameters.Password;
        //$scope.txtPasswordC = parameters.PasswordC;
    }

    $scope.ResetPassword = function () {

        var ResetPasswordInput = {};

        if ($scope.txtPassword != $scope.txtPasswordC) {
            return "Invalid Password"
        }
        else {
            if (parameters != undefined) {

                ResetPasswordInput = {

                    UserId: parameters.UserId,
                    Password: $scope.txtPassword
                }

                UsersAPIService.ResetPassword(ResetPasswordInput).then(function (response) {
                    //if ($scope.onUserUpdated != undefined)
                    //    $scope.onUserUpdated(ResetPasswordInput);

                }).finally(function () {
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
appControllers.controller('Security_ResetPasswordEditorController', ResetPasswordEditorController);