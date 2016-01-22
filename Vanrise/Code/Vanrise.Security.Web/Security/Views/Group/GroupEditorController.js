(function (appControllers) {
    'use strict';

    GroupEditorController.$inject = ['$scope', 'VR_Sec_GroupAPIService', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function GroupEditorController($scope, VR_Sec_GroupAPIService, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var groupEntity;
        var members;

        var userSelecorDirectiveAPI;
        var userSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                $scope.groupId = parameters.groupId;
            }

            isEditMode = ($scope.groupId != undefined);
        }

        function defineScope() {
            $scope.saveGroup = function () {
                if (isEditMode)
                    return updateGroup();
                else
                    return insertGroup();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onUserSelectorReady = function (api) {
                userSelecorDirectiveAPI = api;
                userSelectorReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode)
            {
                getGroup().then(function () {
                    loadAllControls().finally(function () {
                        groupEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
            {
                loadAllControls();
            }
        }

        function getGroup() {
            return VR_Sec_GroupAPIService.GetGroup($scope.groupId)
                .then(function (response) {
                    groupEntity = response;
                });
        }

        function loadAllControls()
        {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGroupMembers])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle()
        {
            if (isEditMode && groupEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(groupEntity.Name, 'Group');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Group');
        }

        function loadStaticData() {

            if (groupEntity == undefined)
                return;

            $scope.name = groupEntity.Name;
            $scope.description = groupEntity.Description;
        }

        function loadGroupMembers()
        {
            var userSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            userSelectorReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    filter: null,
                    selectedIds: undefined
                };

                if (groupEntity) {
                    VR_Sec_UserAPIService.GetMembers(groupEntity.GroupId).then(function (memberIds) {
                        if (memberIds) {
                            directivePayload.selectedIds = [];

                            for (var i = 0; i < memberIds.length; i++) {
                                directivePayload.selectedIds.push(memberIds[i]);
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(userSelecorDirectiveAPI, directivePayload, userSelectorLoadPromiseDeferred);
                    });
                }
                else {
                    VRUIUtilsService.callDirectiveLoad(userSelecorDirectiveAPI, directivePayload, userSelectorLoadPromiseDeferred);
                }
            });
            
            return userSelectorLoadPromiseDeferred.promise;
        }

        function buildGroupObjFromScope() {
            var groupObj = {
                groupId: ($scope.groupId != null) ? $scope.groupId : 0,
                name: $scope.name,
                description: $scope.description,
                members: userSelecorDirectiveAPI.getSelectedIds()
            };

            return groupObj;
        }

        function insertGroup() {
            $scope.isLoading = true;

            var groupObj = buildGroupObjFromScope();

            return VR_Sec_GroupAPIService.AddGroup(groupObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded('Group', response)) {
                        if ($scope.onGroupAdded != undefined)
                            $scope.onGroupAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }

        function updateGroup() {
            $scope.isLoading = true;

            var groupObj = buildGroupObjFromScope();

            return VR_Sec_GroupAPIService.UpdateGroup(groupObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated('Group', response)) {
                        if ($scope.onGroupUpdated != undefined)
                            $scope.onGroupUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
    }

    appControllers.controller('VR_Sec_GroupEditorController', GroupEditorController);

})(appControllers);
