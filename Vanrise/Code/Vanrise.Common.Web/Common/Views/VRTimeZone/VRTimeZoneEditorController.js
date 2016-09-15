(function (appControllers) {

    "use strict";

    vrTimeZoneEditorController.$inject = ['$scope', 'VRCommon_VRTimeZoneAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function vrTimeZoneEditorController($scope, VRCommon_VRTimeZoneAPIService, VRNotificationService, VRNavigationService ,UtilsService) {
        
        var vrTimeZonetId;
        var editMode;
        var vrTimeZoneEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrTimeZonetId = parameters.VRTimeZoneId;
            }
            editMode = (vrTimeZonetId != undefined);
        }
        function defineScope() {


            //$scope.signs = [{ text: '+', value: '' }, { text: '-', value: '-' }];
            $scope.saveVRTimeZone = function () {
                if (editMode)
                    return updateVRTimeZone();
                else
                    return insertVRTimeZone();
            };
            $scope.hasSaveVRTimeZonePermission = function () {
                if (editMode) {
                    return VRCommon_VRTimeZoneAPIService.HasEditVRTimeZonePermission();
                }
                else {
                    return VRCommon_VRTimeZoneAPIService.HasAddVRTimeZonePermission();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            if (editMode) {
                getVRTimeZone().then(function(){
                    loadAllControls().finally(function () {
                        vrTimeZoneEntity = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.isLoading = false;
                    });
                });
            }
            else {
                loadAllControls();
                $scope.isLoading = false;
                
            }

        }

        function getVRTimeZone() {
            return VRCommon_VRTimeZoneAPIService.GetVRTimeZone(vrTimeZonetId).then(function (vrTimeZone) {
                vrTimeZoneEntity = vrTimeZone;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
           
            if (editMode && vrTimeZoneEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(vrTimeZoneEntity.Name, "Time Zone");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Time Zone");
        }

        function buildVRTimeZoneObjFromScope() {
            var obj = {
                TimeZoneId: (vrTimeZonetId != null) ? vrTimeZonetId : 0,
                Name: $scope.name,
                Settings: {
                    Offset:$scope.sign.value+ $scope.time.Hour + ":" + $scope.time.Minute + ":00"
                }
            };
            return obj;
        }

        function loadStaticData() {

            if (vrTimeZoneEntity == undefined)
                return;
            $scope.name = vrTimeZoneEntity.Name;
            $scope.sign = vrTimeZoneEntity.Settings.Offset[0] == '-' ? $scope.signs[1] : $scope.signs[0];
            var timetab = vrTimeZoneEntity.Settings.Offset.split(":");
            var hours = timetab[0];
            var minutes = timetab[1];
            if (timetab[0][0] == '-')
                hours = timetab[0].substring(1);
            $scope.time = {
                Hour: hours,
                Minute: minutes
            };
           
        }
        function insertVRTimeZone() {
            $scope.isLoading = true;

            var vrTimeZoneObject = buildVRTimeZoneObjFromScope();
            return VRCommon_VRTimeZoneAPIService.AddVRTimeZone(vrTimeZoneObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Time Zone", response ,"Name")) {
                    if ($scope.onVRTimeZoneAdded != undefined)
                        $scope.onVRTimeZoneAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateVRTimeZone() {
            $scope.isLoading = true;

            var vrTimeZoneObject = buildVRTimeZoneObjFromScope();

            VRCommon_VRTimeZoneAPIService.UpdateVRTimeZone(vrTimeZoneObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Time Zone", response, "Name")) {
                    if ($scope.onVRTimeZoneUpdated != undefined)
                        $scope.onVRTimeZoneUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_VRTimeZoneEditorController', vrTimeZoneEditorController);
})(appControllers);
