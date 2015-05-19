appControllers.controller('UserUpdateorController', function UserUpdateorController($scope, UsersAPIService, VRNavigationService) {

    var parameters = VRNavigationService.getParameters($scope);


    if (parameters != undefined) {
        $scope.txtName = parameters.Name;
        $scope.txtPassword = parameters.Password;
        $scope.txtEmail = parameters.Email;
        $scope.txtDescription = parameters.Description;
    }

    $scope.SaveUser = function () {
        var user = {};
        
        if (parameters != undefined)
        {
            user = {

                UserId: parameters.UserId,
                Name: $scope.txtName,
                Password: $scope.txtPassword,
                Email: $scope.txtEmail,
                Description: $scope.txtDescription
            }
        }
        else
        {
            user = {

                Name: $scope.txtName,
                Password: $scope.txtPassword,
                Email: $scope.txtEmail,
                Description: $scope.txtDescription
            }
        }
        
        if (user.UserId == null) {
            UsersAPIService.AddUser(user).then(function (response) {
                if ($scope.onUserAdded != undefined)
                    $scope.onUserAdded(user);

            }).finally(function () {
                $scope.$hide();
                $scope.grid.itemAdded(user);

            });
        }
        else {
            UsersAPIService.UpdateUser(user).then(function (response) {
                if ($scope.onUserUpdated != undefined)
                    $scope.onUserUpdated(user);

            }).finally(function () {
                $scope.$hide();
                $scope.grid.itemAdded(user);

            });
        }
    };

    $scope.hide = function () {
        $scope.$hide();
    };

});