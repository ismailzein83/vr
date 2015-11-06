(function (appControllers) {

    "use strict";

    zoneServiceConfigEditorController.$inject = ['$scope', 'WhS_BE_ZoneServiceConfigAPIService', 'VRNotificationService', 'VRNavigationService'];

    function zoneServiceConfigEditorController($scope, WhS_BE_ZoneServiceConfigAPIService, VRNotificationService, VRNavigationService) {


        var serviceFlag;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
           
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                serviceFlag = parameters.ServiceFlag;
            }
            $scope.inedit = editMode = (serviceFlag != undefined);
        }
        function defineScope() {
            $scope.saveZoneServiceConfig = function () {
                if (editMode) {
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
            $scope.isGettingData = true;
            if (editMode) {
                getZoneServiceConfig();
            }
            else {
                $scope.isGettingData = false;
            }

        }
        function getZoneServiceConfig() {
            return WhS_BE_ZoneServiceConfigAPIService.GetZoneServiceConfig(serviceFlag).then(function (zoneServiceConfig) {
                fillScopeFromZoneServiceConfigObj(zoneServiceConfig);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildZoneServiceConfigObjFromScope() {
            var obj = {
                ServiceFlag: $scope.serviceFlag,
                Name: $scope.name
            };
            return obj;
        }

        function fillScopeFromZoneServiceConfigObj(zoneServiceConfig) {
            $scope.name = zoneServiceConfig.Name;
            $scope.serviceFlag = zoneServiceConfig.ServiceFlag;
        }
        function insertZoneServiceConfig() {
            var zoneServiceConfigObject = buildZoneServiceConfigObjFromScope();
            return WhS_BE_ZoneServiceConfigAPIService.AddZoneServiceConfig(zoneServiceConfigObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("ZoneServiceConfig", response, "Name or Service Flag")) {
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
                if (VRNotificationService.notifyOnItemUpdated("ZoneServiceConfig", response, "Name")) {
                    if ($scope.onZoneServiceConfigUpdated != undefined)
                        $scope.onZoneServiceConfigUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_ZoneServiceConfigEditorController', zoneServiceConfigEditorController);
})(appControllers);
