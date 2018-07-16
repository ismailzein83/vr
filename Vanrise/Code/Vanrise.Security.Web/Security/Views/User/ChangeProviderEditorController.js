(function (appControllers) {

    'use strict';

    ChangeProviderEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_SecurityAPIService', 'VRUIUtilsService', 'VR_Sec_GroupAPIService'];

    function ChangeProviderEditorController($scope, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_SecurityAPIService, VRUIUtilsService, VR_Sec_GroupAPIService) {
        var isEditMode;
        var userId;
        var userEntity;
        var context;

        var securityProviderSelectorApi;
        var securityProviderSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        var findUserDirectiveApi;
        var findUserDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        $scope.scopeModel.isRemote = false;
        var connectionId;

        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                userId = parameters.userId;
                context = parameters.context;
                connectionId = parameters.connectionId;
                $scope.scopeModel.isRemote = connectionId != undefined;
            }
        }

        function defineScope() {

            $scope.scopeModel.onSecurityProviderSelectorReady = function (api) {
                securityProviderSelectorApi = api;
                securityProviderSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onFindUserEditorReady = function (api) {
                findUserDirectiveApi = api;
                if (findUserDirectivePromiseDeferred != undefined) {
                    findUserDirectivePromiseDeferred.resolve();
                }
                else {
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingFindUserEditor = value;
                    };
                    var payload = {
                        securityProviderId: securityProviderSelectorApi.getSelectedIds(), showPasswordSection: true, isEditMode: true, context: buildContext()
                        };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, findUserDirectiveApi, payload, setLoader);
                }
            };

            $scope.scopeModel.save = function () {
                    return updateUser();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.hasSaveUserPermission = function () {
                    return VR_Sec_UserAPIService.HasUpdateUserPermission();
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;

                getUser().then(function () {
                        loadAllControls().finally(function () {
                            userEntity = undefined;
                        });

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });       
        }

        function getUser() {
            return VR_Sec_UserAPIService.GetUserbyId(userId).then(function (response) {
                userEntity = response;
            });
        }

        function loadAllControls() {
            var loadFinduserDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultipleAsyncOperations([setTitle,loadSecurityProviderSelector]).then(function () {
                loadFinduserDirective().then(function () {
                    loadFinduserDirectivePromiseDeferred.resolve();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            })
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.scopeModel.isLoading = false;
             });
            return loadFinduserDirectivePromiseDeferred.promise;
        }

        function loadFinduserDirective() {
            var findUserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultiplePromises([findUserDirectivePromiseDeferred.promise]).then(function () {
                findUserDirectivePromiseDeferred = undefined;
                var findUserDirectivePayload =
                {
                    email: userEntity != undefined ? userEntity.Email : undefined,
                    password: userEntity != undefined ? userEntity.Password : undefined,
                    enablePasswordExpiration: userEntity != undefined && userEntity.Settings != undefined ? userEntity.Settings.EnablePasswordExpiration : undefined,
                    securityProviderId: userEntity != undefined ? userEntity.SecurityProviderId : securityProviderSelectorApi.getSelectedIds(),
                    showPasswordSection: true,
                    isEditMode :true,
                    context: buildContext()
                };
                VRUIUtilsService.callDirectiveLoad(findUserDirectiveApi, findUserDirectivePayload, findUserDirectiveLoadDeferred);
            });
            return findUserDirectiveLoadDeferred.promise;
        }

        function buildContext() {
            return {
                fillUserInfo: fillUserInfo,
                connectionId: connectionId
            };
        }

        function fillUserInfo(userInfo) {
            if (userInfo != undefined) {
                $scope.scopeModel.name = userInfo.name;
                $scope.scopeModel.description = userInfo.description;
            }
        }

        function loadSecurityProviderSelector() {
            var securityProviderSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            securityProviderSelectorPromiseDeferred.promise.then(function () {
                var securityProviderPayload = { selectfirstitem: true, connectionId: connectionId };

                if (userEntity != undefined) {
                    securityProviderPayload.selectedIds = userEntity.SecurityProviderId;
                }

                VRUIUtilsService.callDirectiveLoad(securityProviderSelectorApi, securityProviderPayload, securityProviderSelectorLoadDeferred);
            });
            return securityProviderSelectorLoadDeferred.promise.then(function () {
                $scope.scopeModel.showSecurityProviderSelector = !securityProviderSelectorApi.hasSingleItem();
            });
        }

 
        function setTitle() {
            $scope.title = 'Change Provider';
        }


        function buildUserObjFromScope() {
            var findUserDirectiveData = findUserDirectiveApi.getData();

            var userObject = {
                UserId: (userId != null) ? userId : 0,
                SecurityProviderId: securityProviderSelectorApi.getSelectedIds(),
                Email: findUserDirectiveData.email,
                EnablePasswordExpiration: findUserDirectiveData.enablePasswordExpiration,
                Password : findUserDirectiveData.password
            };
          
            return userObject;
        }


        function buildRemoteUserObjFromScope() {
            return { User: buildUserObjFromScope(), VRConnectionId: connectionId };
        }


        function updateUser() {
            $scope.scopeModel.isLoading = true;

            var userObject = buildUserObjFromScope();

            return VR_Sec_UserAPIService.ChangeUserSecurityProvider(userObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('User', response, 'Email')) {
                    if ($scope.onUserUpdated != undefined)
                        $scope.onUserUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_ChangeProviderEditorController', ChangeProviderEditorController);

})(appControllers);
