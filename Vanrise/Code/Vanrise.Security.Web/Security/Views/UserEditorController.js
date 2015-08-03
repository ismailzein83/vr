UserEditorController.$inject = ['$scope', 'UsersAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function UserEditorController($scope, UsersAPIService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.userId = undefined;

        if (parameters != undefined && parameters != null)
            $scope.userId = parameters.userId;

        if ($scope.userId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveUser = function () {
            if (editMode) {
                return updateUser();
            }
            else {
                return insertUser();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;

            getUser().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getUser() {
        return UsersAPIService.GetUserbyId($scope.userId)
            .then(function (response) {
                fillScopeFromUserObj(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildUserObjFromScope() {
        var userObject = {
            userId: ($scope.userId != null) ? $scope.userId : 0,
            name: $scope.name,
            email: $scope.email,
            description: $scope.description,
            Status: $scope.isActive == false ? "0" : "1"
        };
        return userObject;
    }

    function fillScopeFromUserObj(userObject) {
        $scope.name = userObject.Name;
        $scope.email = userObject.Email;
        $scope.description = userObject.Description;
        $scope.isActive = userObject.Status;
    }

    function insertUser() {
        $scope.issaving = true;
        var userObject = buildUserObjFromScope();
        return UsersAPIService.AddUser(userObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("User", response)) {
                if ($scope.onUserAdded != undefined)
                    $scope.onUserAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateUser() {
        $scope.issaving = true;
        var userObject = buildUserObjFromScope();
        UsersAPIService.UpdateUser(userObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("User", response)) {
                if ($scope.onUserUpdated != undefined)
                    $scope.onUserUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    //$scope.CheckUserName = function (name) {
    //    var bool = false;
    //    if (name == undefined)
    //        return null;

    //    UsersAPIService.CheckUserName(name == undefined ? " " : name).then(function (response) {
    //        bool = response;
    //        //if (response == false)
    //        //return "Invalid";
    //        //else
    //        //  return null;

    //    }).finally(function () {

    //    });

    //    if (!bool)
    //        return "Invalid";
    //}
}

appControllers.controller('Security_UserEditorController', UserEditorController);
