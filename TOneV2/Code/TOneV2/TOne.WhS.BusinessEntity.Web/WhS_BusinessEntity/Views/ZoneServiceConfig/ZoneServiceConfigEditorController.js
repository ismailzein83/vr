(function (appControllers) {

    "use strict";

    zoneServiceConfigEditorController.$inject = ['$scope', 'WhS_BE_ZoneServiceConfigAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function zoneServiceConfigEditorController($scope, WhS_BE_ZoneServiceConfigAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var zoneServiceConfigId;
        var zoneServiceConfigEntity;

        var zoneServiceAPI;
        var zoneServiceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                zoneServiceConfigId = parameters.zoneServiceConfigId;
            }
            isEditMode = (zoneServiceConfigId != undefined);
        }

        function defineScope() {

            $scope.hasSaveZoneServiceConfigPermission = function () {
                if (isEditMode)
                    return WhS_BE_ZoneServiceConfigAPIService.HasUpdateZoneServiceConfigPermission();
                else
                    return WhS_BE_ZoneServiceConfigAPIService.HasAddZoneServiceConfigPermission();
            };
            $scope.onZoneServiceSelectorReady = function (api) {
                zoneServiceAPI = api;
                zoneServiceSelectorReadyDeferred.resolve();
            };
            $scope.saveZoneServiceConfig = function () {
                if (isEditMode) {
                    return updateZoneServiceConfig();
                }
                else {
                    return insertZoneServiceConfig();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getZoneServiceConfig().then(function () {
                    loadAllControls().finally(function () { zoneServiceConfigEntity = undefined; });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getZoneServiceConfig() {
            return WhS_BE_ZoneServiceConfigAPIService.GetZoneServiceConfig(zoneServiceConfigId).then(function (response) {
                zoneServiceConfigEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadServiceZoneConfigSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(zoneServiceConfigEntity ? zoneServiceConfigEntity.Settings.Name : null, 'Zone Service Config') : UtilsService.buildTitleForAddEditor('Zone Service Config');
        }

        function loadStaticControls() {
            if (zoneServiceConfigEntity == undefined)
                return;

            $scope.name = zoneServiceConfigEntity.Settings.Name;
            $scope.symbol = zoneServiceConfigEntity.Symbol;
            $scope.description = zoneServiceConfigEntity.Settings.Description;
            $scope.color = zoneServiceConfigEntity.Settings.Color;
            $scope.weight = zoneServiceConfigEntity.Settings.Weight;
        }

        function loadServiceZoneConfigSelector() {
            var serviceZoneConfigLoadDeferred = UtilsService.createPromiseDeferred();

            zoneServiceSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (zoneServiceConfigEntity != undefined && zoneServiceConfigEntity.Settings != undefined) {
                    payload = {
                        filter: {
                            AssinableToServiceId: zoneServiceConfigId
                        }
                    };
                    payload.selectedIds = zoneServiceConfigEntity.Settings.ParentId != null ? zoneServiceConfigEntity.Settings.ParentId : undefined;
                }
                VRUIUtilsService.callDirectiveLoad(zoneServiceAPI, payload, serviceZoneConfigLoadDeferred);
            });

            return serviceZoneConfigLoadDeferred.promise;
        }
        function insertZoneServiceConfig() {
            var zoneServiceConfigObject = buildZoneServiceConfigObjFromScope();
            return WhS_BE_ZoneServiceConfigAPIService.AddZoneServiceConfig(zoneServiceConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Zone Service Config", response, "Symbol or Color")) {
                    if ($scope.onZoneServiceConfigAdded != undefined)
                        $scope.onZoneServiceConfigAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateZoneServiceConfig() {
            var zoneServiceConfigObject = buildZoneServiceConfigObjFromScope();
            WhS_BE_ZoneServiceConfigAPIService.UpdateZoneServiceConfig(zoneServiceConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Zone Service Config", response, "Name")) {
                    if ($scope.onZoneServiceConfigUpdated != undefined)
                        $scope.onZoneServiceConfigUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildZoneServiceConfigObjFromScope() {
            var obj = {
                ZoneServiceConfigId: zoneServiceConfigId != undefined ? zoneServiceConfigId : undefined,
                Symbol: $scope.symbol,
                Settings: {
                    $type: "TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities",
                    Name: $scope.name,
                    Description: $scope.description,
                    Color: $scope.color,
                    ParentId: zoneServiceAPI.getSelectedIds(),
                    Weight: $scope.weight
                }
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_ZoneServiceConfigEditorController', zoneServiceConfigEditorController);

})(appControllers);
