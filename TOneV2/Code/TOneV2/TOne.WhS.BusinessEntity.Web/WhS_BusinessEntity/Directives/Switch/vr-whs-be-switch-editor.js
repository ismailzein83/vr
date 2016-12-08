"use strict";

app.directive("vrWhsBeSwitchEditor", ["UtilsService", "VRNotificationService", "WhS_BE_SwitchAPIService", "WhS_BE_SwitchService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SwitchAPIService, WhS_BE_SwitchService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            onswitchadded:"=",
            onswitchupdated: "="

            //= (means function), @ (means attribute)
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchEditor = new SwitchEditor($scope, ctrl, $attrs);
            switchEditor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Switch/Templates/SwitchFormEditor.html"

    };

    function SwitchEditor($scope, ctrl, $attrs) {
        var isEditMode;
        var switchId;
        var switchEntity;

        var switchSyncSettingsDirectiveAPI;
        var switchSyncSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var switchCDRMappingConfigurationDirectiveAPI;
        var switchCDRMappingConfigurationDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        defineScope();

        function initializeController() {

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());

            function getDirectiveAPI() {
                var directiveAPI = {};
                directiveAPI.load = function (payload) {
                    if (payload != undefined) {
                        switchId = payload.switchId;
                    }
                    isEditMode = (switchId != undefined);
                    return load();
                };
                directiveAPI.getTitle = function () {                   
                    return switchEntity != undefined ?switchEntity.Name: undefined;
                };
                directiveAPI.save = function () {
                    if (isEditMode) {
                        return updateSwitch();
                    }
                    else {
                        return insertSwitch();
                    }
                };
                directiveAPI.hasSavePermission = function () {
                    if (isEditMode)
                        return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
                    else
                        return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
                };
                return directiveAPI;
            }
        };

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSwitchSyncSettingsDirectiveReady = function (api) {
                switchSyncSettingsDirectiveAPI = api;
                switchSyncSettingsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onSwitchCDRMappingConfiguration = function (api) {
                switchCDRMappingConfigurationDirectiveAPI = api;
                switchCDRMappingConfigurationDirectiveReadyDeferred.resolve();
            };
        }
        function load() {
            if (isEditMode) {
               return getSwitch().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
              return  loadAllControls();
            }
        }

        function getSwitch() {
            return WhS_BE_SwitchAPIService.GetSwitch(switchId).then(function (whsSwitch) {
                switchEntity = whsSwitch;
                $scope.scopeModel.switchName = switchEntity.Name;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSwitchSyncSettingsDirective, loadSwitchCDRMappingConfigurationDirectiveDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
            });
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
            return WhS_BE_SwitchAPIService.AddSwitch(buildSwitchObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                    if (ctrl.onswitchadded != undefined)
                        ctrl.onswitchadded(response.InsertedObject);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {                
            });
        }
        function updateSwitch() {
            
            return WhS_BE_SwitchAPIService.UpdateSwitch(buildSwitchObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                    if (ctrl.onswitchupdated != undefined) {
                        ctrl.onswitchupdated(response.UpdatedObject);
                    }
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                
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

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
