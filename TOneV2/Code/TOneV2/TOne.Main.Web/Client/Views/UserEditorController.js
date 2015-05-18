appControllers.controller('UserEditorController', function UserEditorController($scope, UsersAPIService, $modal) {

    if ($scope.user != undefined) {
        $scope.txtName=   $scope.user.Name;
        $scope.txtPassword=   $scope.user.Password;
        $scope.txtEmail=   $scope.user.Email;
        $scope.txtDescription=   $scope.user.Description;
    }

    $scope.SaveUser = function () {
        var user = {

            UserId: null,
            Name: $scope.txtName,
            Password: $scope.txtPassword,
            Email: $scope.txtEmail,
            Description: $scope.txtDescription
        }
        UsersAPIService.SaveUser(user).then(function (response) {

        }).finally(function () {
            $scope.$hide();
            $scope.callBack();
            $scope.grid.itemAdded(user);

        });
    };

    $scope.hide = function () {
        $scope.$hide();
    };


});