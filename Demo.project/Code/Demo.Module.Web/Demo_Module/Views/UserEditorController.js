(function (appControllers) {

    "use strict";

    userEditorController.$inject = ['$scope', 'Demo_Module_UserAPIService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function userEditorController($scope, Demo_Module_UserAPIService,VRUIUtilsService, VRNotificationService, VRNavigationService, UtilsService) {

        var Id;
        var CityId;
        var editMode;
        var userEntity;
        var CityDirectiveApi;
        var CityReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var disableCity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                Id = parameters.Id;
                CityId = parameters.CityId;
                disableCity = parameters.disableCity
            }
            editMode = (Id != undefined);
            $scope.disableCity = editMode || disableCity;
        }



        function defineScope() {
            $scope.onCityDirectiveReady = function (api) {
               CityDirectiveApi = api;
               CityReadyPromiseDeferred.resolve();
            };

            $scope.saveUser = function () {
                if (editMode)
                    return updateUser();
                else {
                 
                    return insertUser();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }


        function load() {

            $scope.isLoading = true;

            if (editMode) {
                getUser().then(function () {
                    loadAllControls()
                        .finally(function () {
                            UserEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getUser() {
            return Demo_Module_UserAPIService.GetUser(Id).then(function (user) {
                userEntity = user;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCitySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && userEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(userEntity.Name, "User");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("User");
        }
        function loadCitySelector() {
            var CityLoadPromiseDeferred = UtilsService.createPromiseDeferred();
           CityReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: userEntity != undefined ? userEntity.CityId : (CityId != undefined) ? CityId : undefined
                    };
                    
                    VRUIUtilsService.callDirectiveLoad(CityDirectiveApi, directivePayload, CityLoadPromiseDeferred);
                });
            return CityLoadPromiseDeferred.promise;
        }
        function loadStaticData() {

            if (userEntity == undefined)
                return;

            $scope.name = userEntity.Name;
        }



        function buildUserObjFromScope() {
            var obj = {
                Id: (Id != null) ? Id : 0,
                Name: $scope.name,
                CityId: CityDirectiveApi.getSelectedIds()

            };
           
            return obj;
        }


        function insertUser() {
            $scope.isLoading = true;

            var userObject = buildUserObjFromScope();
            return Demo_Module_UserAPIService.AddUser(userObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("User", response, "Name")) {
                    if ($scope.onUserAdded != undefined) {

                        $scope.onUserAdded(response.InsertedObject);
                    }
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
            Demo_Module_UserAPIService.UpdateUser(userObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("User", response, "Name")) {
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

    appControllers.controller('Demo_Module_UserEditorController', userEditorController);
})(appControllers);
