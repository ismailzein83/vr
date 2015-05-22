appControllers.controller('UserEditorController', function UserEditorController($scope, UsersAPIService, VRNavigationService) {

    var parameters = VRNavigationService.getParameters($scope);

    if (parameters != undefined) {
        $scope.txtName = parameters.Name;
        $scope.txtPassword = parameters.Password;
        $scope.txtEmail = parameters.Email;
        $scope.txtDescription = parameters.Description;
        $scope.IsActive = parameters.Status;
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
                Status: $scope.IsActive,
                Description: $scope.txtDescription
            }
        }
        else
        {
            user = {
                
                Name: $scope.txtName,
                Password: $scope.txtPassword,
                Email: $scope.txtEmail,
                Status: $scope.IsActive,
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

    $scope.CheckUserName = function (name) {
        var bool = false;
        if (name == undefined)
            return null;

        UsersAPIService.CheckUserName(name == undefined ? " " : name).then(function (response) {
            bool = response;
            //if (response == false)
                //return "Invalid";
            //else
            //  return null;

        }).finally(function () {

        });

        if (! bool)
            return "Invalid";
    }

    //function CheckUserName(name) {
    //    UsersAPIService.CheckUserName(name == undefined ? " " : name).then(function (response) {

    //        if (response == false)
    //            return "Invalid";
    //        else
    //            return null;

    //    }).finally(function () {

    //    });
    //};


});