(function (appControllers) {

    'use strict';

    UserEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_SecurityAPIService', 'VRUIUtilsService', 'VR_Sec_GroupAPIService'];

    function UserEditorController($scope, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_SecurityAPIService, VRUIUtilsService, VR_Sec_GroupAPIService) {
        var isEditMode;
        var userId;
        var groupIds;
        var userEntity;
        var context;
        var isViewHistoryMode;
        var tenantSelectorAPI;
        var tenantReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var groupSelectorAPI;
        var groupReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();
        $scope.scopemodel = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                userId = parameters.userId;
                context = parameters.context;
            }
            isEditMode = (userId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }

        function defineScope() {
            $scope.showTenantSelector = true;

            $scope.passwordHint = "";

            $scope.requiredPassword = false;

            $scope.showPasswordSection = !isEditMode;

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

            $scope.onGroupSelectorReady = function (api) {
                groupSelectorAPI = api;
                groupReadyPromiseDeferred.resolve();
            };
            $scope.validatePasswords = function () {
                if ($scope.password != $scope.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };
            $scope.hideTenantSelectorIfNotNeeded = function () {                
                $scope.showTenantSelector = false;
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getUser().then(function () {
                    getUserGroups().finally(function () {
                        loadAllControls().finally(function () {
                            userEntity = undefined;
                            groupIds =  undefined;
                        });
                    });
                    
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getUserHistory().then(function () {
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

        function getUserGroups() {
            return VR_Sec_GroupAPIService.GetAssignedUserGroups(userId).then(function (response) {
                groupIds = response;
            });
        }
        function getUserHistory() {
            return VR_Sec_UserAPIService.GetUserHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                userEntity = response;

            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, hasAuthServer, loadTenantSelector, loadGroupSelector , loadPasswordHint])
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
        function loadGroupSelector() {
            var loadGroupPromiseDeferred = UtilsService.createPromiseDeferred();

            var payload = undefined;
            if (groupIds != undefined) {
                payload = { selectedIds: groupIds };
            }
            groupReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(groupSelectorAPI, payload, loadGroupPromiseDeferred);
            });

            return loadGroupPromiseDeferred.promise;
        }

        function loadPasswordHint() {
            if(isEditMode ==  true) return ;
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo().then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
                $scope.requiredPassword = response.RequiredPassword  ;
            });
        }

        function hasAuthServer() {
            return VR_Sec_SecurityAPIService.HasAuthServer().then(function (response) {
                $scope.hasAuthServer = response;
            });
        }

        function setTitle() {
            if (isEditMode && userEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(userEntity.Name, 'User');
            else if (isViewHistoryMode && userEntity != undefined)
                $scope.title = "View User: " + userEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor('User');
        }

        function loadStaticData() {

            if (userEntity == undefined)
                return;

            $scope.scopemodel.name = userEntity.Name;
            $scope.scopemodel.email = userEntity.Email;
            $scope.scopemodel.description = userEntity.Description;
            $scope.scopemodel.enabledTill = userEntity.EnabledTill;

            if (userEntity.Settings!= undefined && userEntity.Settings.PhotoFileId != null)
                $scope.scopemodel.userPhoto = {
                    fileId: userEntity.Settings.PhotoFileId
                };
            else
                $scope.scopemodel.userPhoto = null;
        }

        function buildUserObjFromScope() {
            var userObject = {
                UserId: (userId != null) ? userId : 0,
                Name: $scope.scopemodel.name,
                Email: $scope.scopemodel.email,
                Description: $scope.scopemodel.description,
                EnabledTill: $scope.scopemodel.enabledTill,
                TenantId: tenantSelectorAPI.getSelectedIds(),
                GroupIds: groupSelectorAPI.getSelectedIds(),
                PhotoFileId: $scope.scopemodel.userPhoto != null ? $scope.scopemodel.userPhoto.fileId : null
            };
            if (!isEditMode)
                userObject.Password = $scope.password;
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
