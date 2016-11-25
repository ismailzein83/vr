(function (appControllers) {

    'use strict';

    UserEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_SecurityAPIService', 'VRUIUtilsService'];

    function UserEditorController($scope, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_SecurityAPIService, VRUIUtilsService) {
        var isEditMode;
        var userId;
        var userEntity;
        var tenantSelectorAPI;
        var tenantReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();
        $scope.scopemodel = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                userId = parameters.userId;

            isEditMode = (userId != undefined);
        }

        function defineScope() {
            $scope.showTenantSelector = true;

            $scope.save = function () {
                if (isEditMode)
                    return updateUser();
                else
                    return insertUser();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasSaveUserPermission = function () {
                if (isEditMode) {
                    return VR_Sec_UserAPIService.HasUpdateUserPermission();
                }
                else {
                    return VR_Sec_UserAPIService.HasAddUserPermission();
                }
            };

            $scope.onTenantSelectorReady = function (api) {
                tenantSelectorAPI = api;
                tenantReadyPromiseDeferred.resolve();
            };

            $scope.hideTenantSelectorIfNotNeeded = function () {
                $scope.showTenantSelector = false;
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
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getUser() {
            return VR_Sec_UserAPIService.GetUserbyId(userId).then(function (response) {
                userEntity = response;
                $scope.isInEditMode = true;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, hasAuthServer, loadTenantSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadTenantSelector() {
            var loadTenantPromiseDeferred = UtilsService.createPromiseDeferred();

            var payload = undefined;
            if (userEntity != undefined) {
                payload = { selectedIds: userEntity.TenantId };
            }
            tenantReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(tenantSelectorAPI, payload, loadTenantPromiseDeferred);
            });

            return loadTenantPromiseDeferred.promise;
        }

        function hasAuthServer() {
            return VR_Sec_SecurityAPIService.HasAuthServer().then(function (response) {
                $scope.hasAuthServer = response;
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

            $scope.scopemodel.name = userEntity.Name;
            $scope.scopemodel.email = userEntity.Email;
            $scope.scopemodel.description = userEntity.Description;
            $scope.scopemodel.isActive = userEntity.Status;
        }

        function buildUserObjFromScope() {
            var userObject = {
                UserId: (userId != null) ? userId : 0,
                Name: $scope.scopemodel.name,
                Email: $scope.scopemodel.email,
                Description: $scope.scopemodel.description,
                Status: $scope.scopemodel.isActive == false ? '0' : '1',
                TenantId: tenantSelectorAPI.getSelectedIds()
            };

            return userObject;
        }

        function insertUser() {
            $scope.isLoading = true;

            var userObject = buildUserObjFromScope();

            return VR_Sec_UserAPIService.AddUser(userObject)
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

            return VR_Sec_UserAPIService.UpdateUser(userObject).then(function (response) {
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

    appControllers.controller('VR_Sec_UserEditorController', UserEditorController);

})(appControllers);
