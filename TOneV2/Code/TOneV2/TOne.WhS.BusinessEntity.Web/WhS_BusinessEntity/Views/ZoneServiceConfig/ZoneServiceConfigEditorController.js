(function (appControllers) {

    "use strict";

    zoneServiceConfigEditorController.$inject = ['$scope', 'WhS_BE_ZoneServiceConfigAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function zoneServiceConfigEditorController($scope, WhS_BE_ZoneServiceConfigAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var isEditMode;
        var serviceFlag;
        var zoneServiceConfigEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
           
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                serviceFlag = parameters.ServiceFlag;
            }
            $scope.inedit = isEditMode = (serviceFlag != undefined);
        }

        function defineScope() {

            $scope.hasSaveZoneServiceConfigPermission = function () {
                if (isEditMode)
                    return WhS_BE_ZoneServiceConfigAPIService.HasUpdateZoneServiceConfigPermission();
                else
                    return WhS_BE_ZoneServiceConfigAPIService.HasAddZoneServiceConfigPermission();
            }

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
            return WhS_BE_ZoneServiceConfigAPIService.GetZoneServiceConfig(serviceFlag).then(function (response) {
                zoneServiceConfigEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(zoneServiceConfigEntity ? zoneServiceConfigEntity.Name : null, 'Zone Service Config') : UtilsService.buildTitleForAddEditor('Zone Service Config');
        }

        function loadStaticControls() {
            if (zoneServiceConfigEntity) {
                $scope.name = zoneServiceConfigEntity.Name;
                $scope.serviceFlag = zoneServiceConfigEntity.ServiceFlag;
            }
        }

        function fillScopeFromZoneServiceConfigObj(zoneServiceConfig) {
            
        }

        function insertZoneServiceConfig() {
            var zoneServiceConfigObject = buildZoneServiceConfigObjFromScope();
            return WhS_BE_ZoneServiceConfigAPIService.AddZoneServiceConfig(zoneServiceConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Zone Service Config", response, "Name or Service Flag")) {
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
                ServiceFlag: $scope.serviceFlag,
                Name: $scope.name
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_ZoneServiceConfigEditorController', zoneServiceConfigEditorController);

})(appControllers);
