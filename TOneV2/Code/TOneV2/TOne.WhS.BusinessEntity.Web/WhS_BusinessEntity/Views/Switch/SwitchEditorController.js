﻿(function (appControllers) {

    "use strict";

    switchEditorController.$inject = ["$scope", "WhS_BE_SwitchAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function switchEditorController($scope, WhS_BE_SwitchAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var switchId;
        var switchEntity;

        var switchSyncSettingsDirectiveAPI;
        var switchSyncSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var switchCDRMappingConfigurationDirectiveAPI;
        var switchCDRMappingConfigurationDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                switchId = parameters.switchId;
            }

            isEditMode = (switchId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.hasSaveSwitchPermission = function () {
                if (isEditMode)
                    return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
                else
                    return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
            };

            $scope.scopeModel.saveSwitch = function () {
                if (isEditMode) {
                    return updateSwitch();
                }
                else {
                    return insertSwitch();
                }
            };

            $scope.scopeModel.onSwitchSyncSettingsDirectiveReady = function (api) {
                switchSyncSettingsDirectiveAPI = api;
                switchSyncSettingsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onSwitchCDRMappingConfiguration = function (api) {
                switchCDRMappingConfigurationDirectiveAPI = api;
                switchCDRMappingConfigurationDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Switch";
                getSwitch().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getSwitch() {
            return WhS_BE_SwitchAPIService.GetSwitch(switchId).then(function (whsSwitch) {
                switchEntity = whsSwitch;
                $scope.scopeModel.switchName = switchEntity.Name;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSwitchSyncSettingsDirective, loadSwitchCDRMappingConfigurationDirectiveDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                var switchName = (switchEntity != undefined) ? switchEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(switchName, 'SwitchName');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('SwitchName');
            }
        }
        function loadSwitchSyncSettingsDirective() {
            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            switchSyncSettingsDirectiveReadyDeferred.promise.then(function () {
                var settingsDirectivePayload;
                if (switchEntity != undefined && switchEntity.Settings != undefined) {
                    settingsDirectivePayload = { switchSynchronizerSettings: switchEntity.Settings.RouteSynchronizer };
                }
                VRUIUtilsService.callDirectiveLoad(switchSyncSettingsDirectiveAPI, settingsDirectivePayload, switchSyncSettingsDirectiveReadyDeferred);
            });

            return switchSyncSettingsDirectiveReadyDeferred.promise;
        }
        function loadSwitchCDRMappingConfigurationDirectiveDirective() {
            var switchCDRMappingConfigurationDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            switchCDRMappingConfigurationDirectiveReadyDeferred.promise.then(function () {

                var switchCDRMappingConfigurationDirectivePayload;
                if (switchEntity != undefined && switchEntity.Settings != undefined) {
                    switchCDRMappingConfigurationDirectivePayload = {
                        switchCDRMappingConfiguration: switchEntity.Settings.SwitchCDRMappingConfiguration
                    }
                }

                VRUIUtilsService.callDirectiveLoad(switchCDRMappingConfigurationDirectiveAPI, switchCDRMappingConfigurationDirectivePayload, switchCDRMappingConfigurationDirectiveLoadDeferred);
            });

            return switchCDRMappingConfigurationDirectiveLoadDeferred.promise;
        }

        function insertSwitch() {
            $scope.isLoading = true;
            return WhS_BE_SwitchAPIService.AddSwitch(buildSwitchObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                    if ($scope.onSwitchAdded != undefined)
                        $scope.onSwitchAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function updateSwitch() {
            $scope.isLoading = true;
            return WhS_BE_SwitchAPIService.UpdateSwitch(buildSwitchObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                    if ($scope.onSwitchUpdated != undefined)
                        $scope.onSwitchUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildSwitchObjFromScope() {

            var obj = {
                SwitchId: (switchId != null) ? switchId : 0,
                Name: $scope.scopeModel.switchName,
                Settings: {
                    $type: "TOne.WhS.BusinessEntity.Entities.SwitchSettings, TOne.WhS.BusinessEntity.Entities",
                    RouteSynchronizer: switchSyncSettingsDirectiveAPI.getData().SwitchRouteSynchronizer,
                    SwitchCDRMappingConfiguration: switchCDRMappingConfigurationDirectiveAPI.getData()
                }
            };
            return obj;
        }
    }

    appControllers.controller("WhS_BE_SwitchEditorController", switchEditorController);
})(appControllers);
