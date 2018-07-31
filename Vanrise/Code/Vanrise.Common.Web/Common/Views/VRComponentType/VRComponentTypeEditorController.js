(function (appControllers) {

    "use strict";

    settingsEditorController.$inject = ['$scope', 'VRCommon_VRComponentTypeAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function settingsEditorController($scope, VRCommon_VRComponentTypeAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrComponentTypeId;
        var componentTypeEntity;

        var extensionConfigurationId;

        var componentTypeEditorAPI;
        var componentTypeEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrComponentTypeId = parameters.vrComponentTypeId;
                extensionConfigurationId = parameters.extensionConfigId;
            }
            isEditMode = (vrComponentTypeId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.ExtensionConfigurationEditor;

            $scope.scopeModel.onVRComponentTypeEditorReady = function (api) {
                componentTypeEditorAPI = api;
                componentTypeEditorReadyDeferred.resolve();
            };

            $scope.saveComponentType = function () {
                if (isEditMode) {
                    return updateComponentType();
                }
                else {
                    return insertComponentType();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasEditVRComponentTypePermission = function () {
                return VRCommon_VRComponentTypeAPIService.HasEditVRComponentTypePermission();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            var promises = [];

            var getComponentTypeExtensionPromise = getComponentExtensionConfig();
            promises.push(getComponentTypeExtensionPromise);

            if (isEditMode) {
                var getgetVRComponentTypePromise = getVRComponentType();
                promises.push(getgetVRComponentTypePromise);
            }

            UtilsService.waitMultiplePromises(promises).then(function () {
                loadAllControls().then(function () {

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getComponentExtensionConfig() {
            return VRCommon_VRComponentTypeAPIService.GetVRComponentTypeExtensionConfigById(extensionConfigurationId).then(function (response) {
                $scope.scopeModel.ExtensionConfigurationEditor = response.Editor;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getVRComponentType() {
            return VRCommon_VRComponentTypeAPIService.GetVRComponentType(vrComponentTypeId).then(function (setting) {
                $scope.vrComponentTypeEditor = setting.Settings.Editor;
                componentTypeEntity = setting;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadComponentTypeEditor])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadComponentTypeEditor() {
            var loadComponentTypeEditorPromiseDeferred = UtilsService.createPromiseDeferred();
            componentTypeEditorReadyDeferred.promise.then(function () {

                var payLoad = {
                    componentType: componentTypeEntity,
                    extensionConfigId: extensionConfigurationId
                };
                VRUIUtilsService.callDirectiveLoad(componentTypeEditorAPI, payLoad, loadComponentTypeEditorPromiseDeferred);
            });
            return loadComponentTypeEditorPromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor(componentTypeEntity.Name, "Component Type");
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Component Type');
            }
        }

        function loadStaticData() {
            if (componentTypeEntity != undefined) {
                $scope.name = componentTypeEntity.Name;
            }
        }

        function buildComponentTypeObjFromScope() {
            var obj = componentTypeEditorAPI.getData();
            return obj;
        }

        function updateComponentType() {
            $scope.scopeModel.isLoading = true;

            var settingObject = buildComponentTypeObjFromScope();
            settingObject.VRComponentTypeId = componentTypeEntity.VRComponentTypeId;
            VRCommon_VRComponentTypeAPIService.UpdateVRComponentType(settingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Component Type", response, "Name")) {
                    if ($scope.onVRComponentTypeUpdated != undefined)
                        $scope.onVRComponentTypeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertComponentType() {
            $scope.scopeModel.isLoading = true;

            var settingObject = buildComponentTypeObjFromScope();

            VRCommon_VRComponentTypeAPIService.AddVRComponentType(settingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Component Type", response, "Name")) {
                    if ($scope.onVRComponentTypeAdded != undefined)
                        $scope.onVRComponentTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_VRComponentTypeEditorController', settingsEditorController);
})(appControllers);
