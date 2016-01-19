(function (appControllers) {
    "use strict";

    UserEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function UserEditorController($scope, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var isEditMode;
        var userEntity;
        
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            $scope.userId = undefined;

            if (parameters != undefined && parameters != null)
                $scope.userId = parameters.userId;

            isEditMode = ($scope.userId != undefined);
        }

        function defineScope() {
            $scope.SaveUser = function () {
                
                if (isEditMode)
                    return updateUser();
                else
                    return insertUser();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            if (isEditMode) {
                $scope.isLoading = true;

                getUser().then(function () {
                    loadAllControls().finally(function () {
                        userEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getUser() {
            return VR_Sec_UserAPIService.GetUserbyId($scope.userId).then(function (response) {
                userEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && userEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(userEntity.Name, "User");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("User");
        }

        function loadStaticData() {
            
            if (userEntity == undefined)
                return;

            $scope.name = userEntity.Name;
            $scope.email = userEntity.Email;
            $scope.description = userEntity.Description;
            $scope.isActive = userEntity.Status;
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

        function insertUser() {
            $scope.isLoading = true;

            var userObject = buildUserObjFromScope();

            return VR_Sec_UserAPIService.AddUser(userObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("User", response, "Email")) {
                    if ($scope.onUserAdded != undefined)
                        $scope.onUserAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateUser() {
            $scope.isLoading = true;

            var userObject = buildUserObjFromScope();

            return VR_Sec_UserAPIService.UpdateUser(userObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("User", response, "Email")) {
                    if ($scope.onUserUpdated != undefined)
                        $scope.onUserUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_UserEditorController', UserEditorController);

})(appControllers);


