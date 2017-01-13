(function (appControllers) {

    "use strict";

    settingsEditorController.$inject = ['$scope', 'VRCommon_SettingsAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function settingsEditorController($scope, VRCommon_SettingsAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var settingsId;
        var settingEntity;

        var settingsEditorAPI;
        var settingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                settingsId = parameters.settingsId;
            }
        }

        function defineScope() {
            $scope.onSettingsEditorReady = function (api) {
                settingsEditorAPI = api;
                settingsEditorReadyDeferred.resolve();
            };

            $scope.saveSetting = function () {
                return updateSetting();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasUpdateSettingPermission = function () {
                return VRCommon_SettingsAPIService.HasUpdateSettingsPermission();
            };

        }

        function load() {
            $scope.isLoading = true;


            getSetting().then(function () {
                loadAllControls().finally(function () {
                    settingEntity = undefined;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });

        }

        function getSetting() {
            return VRCommon_SettingsAPIService.GetSetting(settingsId).then(function (setting) {
                $scope.settingEditor = setting.Settings.Editor;
                settingEntity = setting;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingEditor])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadSettingEditor() {
            var loadSettingsEditorPromiseDeferred = UtilsService.createPromiseDeferred();
            settingsEditorReadyDeferred.promise.then(function () {
                var payLoad;

                if (settingEntity != undefined) {
                    payLoad = {
                        data: settingEntity.Data
                    };
                }
                VRUIUtilsService.callDirectiveLoad(settingsEditorAPI, payLoad, loadSettingsEditorPromiseDeferred);
            });
            return loadSettingsEditorPromiseDeferred.promise;
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForUpdateEditor(settingEntity.Name, "Setting", $scope);
        }

        function loadStaticData() {
            if (settingEntity != undefined) {
                $scope.name = settingEntity.Name;
            }
        }

        function buildSettingObjFromScope() {
            var obj = {
                SettingId: settingsId,
                Name: $scope.name,
                Data: settingsEditorAPI.getData()
            };
            return obj;
        }

        function updateSetting() {
            $scope.isLoading = true;

            var settingObject = buildSettingObjFromScope();

            VRCommon_SettingsAPIService.UpdateSetting(settingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Setting", response, "Name")) {
                    if ($scope.onSettingsUpdated != undefined)
                        $scope.onSettingsUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_SettingsEditorController', settingsEditorController);
})(appControllers);
