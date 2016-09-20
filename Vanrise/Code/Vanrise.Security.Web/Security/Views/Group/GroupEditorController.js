(function (appControllers) {
    'use strict';

    GroupEditorController.$inject = ['$scope', 'VR_Sec_GroupAPIService', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function GroupEditorController($scope, VR_Sec_GroupAPIService, VR_Sec_UserAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var groupEntity;
        var members;

        var groupeTypeAPI;
        var groupeTypeReadyPromiseDeferred; 

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
            $scope.scopeModal = {};
            $scope.scopeModal.groupTypeTemplates = [];
            $scope.saveGroup = function () {
                if (isEditMode)
                    return updateGroup();
                else
                    return insertGroup();
            };
            $scope.hasSaveGroupPermission = function () {
                if (isEditMode) {
                    return VR_Sec_GroupAPIService.HasEditGroupPermission();
                }
                else {
                    return VR_Sec_GroupAPIService.HasAddGroupPermission();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onGroupeTypeDirectiveReady = function (api) {
                groupeTypeAPI = api;

                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingGroupType = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, groupeTypeAPI, undefined, setLoader, groupeTypeReadyPromiseDeferred);
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
                    $scope.isLoading = false;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGroupTemplate])
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

            $scope.scopeModal.name = groupEntity.Name;
            $scope.scopeModal.description = groupEntity.Description;
        }
        
        function loadGroupTemplate() {
            var promises = [];
            var goupPayload;
            if (groupEntity != undefined && groupEntity.Settings!=undefined) {
                goupPayload = {
                    settings: groupEntity.Settings
                };
            }
            var groupTemplateLoad = VR_Sec_GroupAPIService.GetGroupTemplate().then(function (response) {
                $scope.scopeModal.groupTypeTemplates.length = 0;
                angular.forEach(response, function (item) {
                    $scope.scopeModal.groupTypeTemplates.push(item);
                });

                if (goupPayload) {
                    $scope.scopeModal.selectedGroupTypeTemplate = UtilsService.getItemByVal($scope.scopeModal.groupTypeTemplates, goupPayload.settings.ConfigId, "ExtensionConfigurationId");
                }
            });
            promises.push(groupTemplateLoad);
            if (goupPayload) {
                var loadGroupTypePromiseDeferred = UtilsService.createPromiseDeferred();
                groupeTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadGroupTypePromiseDeferred.promise);
                groupeTypeReadyPromiseDeferred.promise.then(function () {
                    groupeTypeReadyPromiseDeferred = undefined;                  
                    VRUIUtilsService.callDirectiveLoad(groupeTypeAPI, goupPayload, loadGroupTypePromiseDeferred);

                });
            }
            return UtilsService.waitMultiplePromises(promises);
        }
       
        function buildGroupObjFromScope() {
            var settings = VRUIUtilsService.getSettingsFromDirective($scope.scopeModal, groupeTypeAPI, 'selectedGroupTypeTemplate')
            var groupObj = {
                groupId: ($scope.groupId != null) ? $scope.groupId : 0,
                name: $scope.scopeModal.name,
                description: $scope.scopeModal.description,
                Settings: settings
            };

            return groupObj;
        }

        function insertGroup() {
            $scope.isLoading = true;

            var groupObj = buildGroupObjFromScope();

            return VR_Sec_GroupAPIService.AddGroup(groupObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded('Group', response, 'Name')) {
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
                    if (VRNotificationService.notifyOnItemUpdated('Group', response, 'Name')) {
                        if ($scope.onGroupUpdated && typeof $scope.onGroupUpdated == 'function') {
                            $scope.onGroupUpdated(response.UpdatedObject);
                        }
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
