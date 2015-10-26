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

        editMode = ($scope.userId != undefined);
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
        var userObject = buildUserObjFromScope();
        return UsersAPIService.AddUser(userObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("User", response, "Email")) {
                if ($scope.onUserAdded != undefined)
                    $scope.onUserAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateUser() {
        var userObject = buildUserObjFromScope();
        return UsersAPIService.UpdateUser(userObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("User", response, "Email")) {
                if ($scope.onUserUpdated != undefined)
                    $scope.onUserUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

   
}

appControllers.controller('Security_UserEditorController', UserEditorController);
