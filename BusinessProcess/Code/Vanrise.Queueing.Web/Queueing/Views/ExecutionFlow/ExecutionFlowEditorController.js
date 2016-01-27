(function (appControllers) {

    'use strict';

    ExecutionFlowEditorController.$inject = ['$scope', 'VR_Queueing_ExecutionFlowAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function ExecutionFlowEditorController($scope, VR_Queueing_ExecutionFlowAPIService, VRNotificationService, VRNavigationService, UtilsService) {
        var isEditMode;
        var userId;
        var userEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                userId = parameters.userId;

            isEditMode = (userId != undefined);
        }

        function defineScope() {
            $scope.save = function () {
                if (isEditMode)
                    return updateUser();
                else
                    return insertUser();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
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
            return VR_Queueing_ExecutionFlowAPIService.GetUserbyId(userId).then(function (response) {
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
                $scope.title = UtilsService.buildTitleForUpdateEditor(userEntity.Name, 'User');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('User');
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
                userId: (userId != null) ? userId : 0,
                name: $scope.name,
                email: $scope.email,
                description: $scope.description,
                Status: $scope.isActive == false ? '0' : '1'
            };
            return userObject;
        }

        function insertUser() {
            $scope.isLoading = true;

            var userObject = buildUserObjFromScope();

            return ExecutionFlowAPIService.AddUser(userObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('User', response, 'Email')) {
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

            return ExecutionFlowAPIService.UpdateUser(userObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('User', response, 'Email')) {
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

    appControllers.controller('ExecutionFlowEditorController', ExecutionFlowEditorController);

})(appControllers);
