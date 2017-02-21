(function (appControllers) {

    "use strict";

    VRApplicationVisibilityEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRCommon_VRApplicationVisibilityAPIService'];

    function VRApplicationVisibilityEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService, VRCommon_VRApplicationVisibilityAPIService) {

        var isEditMode;

        var vrApplicationVisibilityId;
        var vrApplicationVisibilityEntity;
        var vrApplicationVisibilityEditorRuntime;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrApplicationVisibilityId = parameters.vrApplicationVisibilityId;
            }

            isEditMode = (vrApplicationVisibilityId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModel.hasSaveVRApplicationVisibilityPermission = function () {
                if (isEditMode)
                    return VRCommon_VRApplicationVisibilityAPIService.HasEditVRApplicationVisibilityPermission();
                else
                    return VRCommon_VRApplicationVisibilityAPIService.HasAddVRApplicationVisibilityPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRApplicationVisibility().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRApplicationVisibility() {
            return VRCommon_VRApplicationVisibilityAPIService.GetVRApplicationVisibilityEditorRuntime(vrApplicationVisibilityId).then(function (response) {
                if (response != undefined) {
                    vrApplicationVisibilityEditorRuntime = response;
                    vrApplicationVisibilityEntity = response.Entity;
                }
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var vrApplicationVisibilityName = (vrApplicationVisibilityEntity != undefined) ? vrApplicationVisibilityEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(vrApplicationVisibilityName, 'Application Visibility');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Application Visibility');
            }
        }
        function loadStaticData() {
            if (vrApplicationVisibilityEntity == undefined)
                return;
            $scope.scopeModel.name = vrApplicationVisibilityEntity.Name;
        }
        function loadSettingsDirective() {
            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            settingsDirectiveReadyDeferred.promise.then(function () {
                var settingsDirectivePayload;
                if (vrApplicationVisibilityEntity != undefined) {
                    settingsDirectivePayload = {
                        Settings: vrApplicationVisibilityEntity.Settings,
                        ModulesVisibilityEditorRuntime: vrApplicationVisibilityEditorRuntime.ModulesVisibilityEditorRuntime
                    };
                }
                VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
            });

            return settingsDirectiveLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRApplicationVisibilityAPIService.AddVRApplicationVisibility(buildVRApplicationVisibilityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRApplicationVisibility', response, 'Name')) {
                    if ($scope.onVRApplicationVisibilityAdded != undefined)
                        $scope.onVRApplicationVisibilityAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRApplicationVisibilityAPIService.UpdateVRApplicationVisibility(buildVRApplicationVisibilityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRApplicationVisibility', response, 'Name')) {
                    if ($scope.onVRApplicationVisibilityUpdated != undefined) {
                        $scope.onVRApplicationVisibilityUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRApplicationVisibilityObjFromScope() {
            
            return {
                VRApplicationVisibilityId: vrApplicationVisibilityEntity != undefined ? vrApplicationVisibilityEntity.VRApplicationVisibilityId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settingsDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VRCommon_VRApplicationVisibilityEditorController', VRApplicationVisibilityEditorController);

})(appControllers);