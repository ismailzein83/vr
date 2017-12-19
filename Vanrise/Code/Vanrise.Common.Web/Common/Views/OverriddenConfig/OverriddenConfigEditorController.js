(function (appControllers) {

    "use strict";

    overriddenConfigEditorController.$inject = ['$scope', 'VRCommon_OverriddenConfigAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function overriddenConfigEditorController($scope, VRCommon_OverriddenConfigAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var overriddenConfigurationId;
        var isEditMode;
        var overriddenConfigurationEntity;
        var overriddenConfigurationGroupSelectorApi;
        var overriddenConfigurationGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var settingReadyPromiseDeferred;
        var settingDirectiveAPI;
        var context;
        var isViewHistoryMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                overriddenConfigurationId = parameters.OverriddenConfigId;
                context = parameters.context;
            }
            isEditMode = (overriddenConfigurationId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSettingDirectiveReady = function (api) {

                settingDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingDirectiveAPI, undefined, setLoader, settingReadyPromiseDeferred);
            };
            $scope.scopeModel.overriddenConfigSettingConfigs = [];

            $scope.scopeModel.onOverrinddenConfigGroupSelectorReady = function (api) {
                overriddenConfigurationGroupSelectorApi = api;
                overriddenConfigurationGroupSelectorReadyPromiseDeferred.resolve();
            };

            $scope.saveOverriddenConfig = function () {
                if (isEditMode)
                    return updateOverriddenConfig();
                else {
                    return insertOverriddenConfig();
                }
                   
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };


        }

        function load() {

            $scope.isLoading = true;
            getOverriddenConfigSettingConfigs().then(function () {
                if (isEditMode) {
                    getOverriddenConfiguration().then(function () {
                        loadAllControls()
                            .finally(function () {
                                overriddenConfigurationEntity = undefined;
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                }
                else if (isViewHistoryMode) {
                    getOverriddenConfigurationHistory().then(function () {
                        loadAllControls().finally(function () {
                            overriddenConfigurationEntity = undefined;
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });

                }

                else {
                    loadAllControls();

                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }

        function getOverriddenConfigSettingConfigs() {
            return VRCommon_OverriddenConfigAPIService.GetOverriddenConfigSettingConfigs().then(function (response) {
                if (response) {
                    $scope.scopeModel.overriddenConfigSettingConfigs = response;
                }
            });
        }

        function getOverriddenConfigurationHistory() {
            return VRCommon_OverriddenConfigAPIService.GetOverriddenConfigurationHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                overriddenConfigurationEntity = response;

            });
        }
        function getOverriddenConfiguration() {
            return VRCommon_OverriddenConfigAPIService.GetOverriddenConfiguration(overriddenConfigurationId).then(function (overriddenConfiguration) {
                overriddenConfigurationEntity = overriddenConfiguration;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadOverriddenConfigurationGroupSelector, loadSettingDirectiveSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && overriddenConfigurationEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(overriddenConfigurationEntity.Name, "Overridden Configuration");
            else if (isViewHistoryMode && overriddenConfigurationEntity != undefined)
                $scope.title = "View Overridden Configuration: " + overriddenConfigurationEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Overridden Configuration");
        }

        function loadStaticData() {

            if (overriddenConfigurationEntity == undefined)
                return;

            $scope.scopeModel.name = overriddenConfigurationEntity.Name;
        }

        function loadOverriddenConfigurationGroupSelector() {

            var overriddenConfigGroupLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            overriddenConfigurationGroupSelectorReadyPromiseDeferred.promise
                .then(function () {

                    var directivePayload = {
                        selectedIds: overriddenConfigurationEntity != undefined ? overriddenConfigurationEntity.GroupId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(overriddenConfigurationGroupSelectorApi, directivePayload, overriddenConfigGroupLoadPromiseDeferred);
                });

            return overriddenConfigGroupLoadPromiseDeferred.promise;
        }
        function loadSettingDirectiveSection() {

            if (overriddenConfigurationEntity != undefined && overriddenConfigurationEntity.Settings != undefined) {
                settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.overriddenConfigSettingConfigs, overriddenConfigurationEntity.Settings.ExtendedSettings.ConfigId, "ExtensionConfigurationId");

                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                settingReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            extendedSettings: overriddenConfigurationEntity.Settings.ExtendedSettings
                        };
                        settingReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(settingDirectiveAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                    });

                return loadSettingDirectivePromiseDeferred.promise;
            }

        }
        function buildOverriddenConfigurationObjFromScope() {
            var extendedSettings = settingDirectiveAPI.getData();
            extendedSettings.ConfigId = $scope.scopeModel.selectedSetingsTypeConfig.ExtensionConfigurationId;
            var obj = {
                OverriddenConfigurationId: (overriddenConfigurationId != null) ? overriddenConfigurationId : null,
                Name: $scope.scopeModel.name,
                GroupId: overriddenConfigurationGroupSelectorApi.getSelectedIds(),
                Settings: {
                    ExtendedSettings: extendedSettings
                }
            };
            return obj;
        }


        function insertOverriddenConfig() {
            $scope.isLoading = true;

            var overriddenConfigObject = buildOverriddenConfigurationObjFromScope();
            return VRCommon_OverriddenConfigAPIService.AddOverriddenConfiguration(overriddenConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Overridden Configuration", response, "Name")) {
                    if ($scope.onOverriddenConfigAdded != undefined)
                        $scope.onOverriddenConfigAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
        function updateOverriddenConfig() {
            $scope.isLoading = true;

            var overriddenConfigObject = buildOverriddenConfigurationObjFromScope();
            VRCommon_OverriddenConfigAPIService.UpdateOverriddenConfiguration(overriddenConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Overridden Configuration", response, "Name")) {
                    if ($scope.onOverriddenConfigUpdated != undefined)
                        $scope.onOverriddenConfigUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_OverridddenConfigEditorController', overriddenConfigEditorController);
})(appControllers);
