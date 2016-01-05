(function (appControllers) {

    "use strict";

    newZoneDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','WhS_CP_NewCPOutputResultEnum'];

    function newZoneDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_NewCPOutputResultEnum) {

        var zoneId;
        var countryId;
        var editMode;
        var zoneEntity;
        var sellingNumberPlanId;
        var disableCountry;

        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                countryId = parameters.CountryId;
                $scope.countryName = parameters.CountryName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
            editMode = (zoneId != undefined);
            load();
        }
 
        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.countryName;
            $scope.zones = [];

            $scope.validateZones = function () {
                if ($scope.zones != undefined && $scope.zones.length == 0)
                    return "Enter at least one zone.";
                return null;
            };
            $scope.saveZone = function () {
                if (editMode) {
                    return updateZone();
                }
                else {
                    return insertZone();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.disabledZone = true;
            $scope.onZoneValueChange = function (value) {
                $scope.disabledZone = (value == undefined) || UtilsService.getItemIndexByVal($scope.zones, value, "zone") != -1;
            }
            $scope.addZoneValue = function () {
                $scope.zones.push({ zone: $scope.zoneValue });
                $scope.zoneValue = undefined;
                $scope.disabledZone = true;
            };
        }

        function load() {
            $scope.isGettingData = true;
            if (countryId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor("Zone for Country " + $scope.countryName);
                loadAllControls();
            }
            else if (editMode) {
                getZone().then(function () {
                    loadAllControls()
                        .finally(function () {
                            zoneEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
                $scope.title = UtilsService.buildTitleForAddEditor("Zone");
            }
        }

        function loadAllControls() {
            $scope.isGettingData = false;
        }

        function getZone() {

        }

        function buildZoneObjFromScope() {
            var result = [];
            for (var i = 0; i < $scope.zones.length; i++) {
                result.push({
                    Name: $scope.zones[i].zone,
                    CountryId: countryId
                });
            }

            return result;
        }

        function getNewZoneFromZoneObj(zoneObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                NewZones: zoneObj
            }
        }

        function fillScopeFromZoneObj(zone) {
            $scope.name = zone.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.name, "Zone");
        }

        function insertZone() {
            var zoneItem = buildZoneObjFromScope();
            var input = getNewZoneFromZoneObj(zoneItem);
            return WhS_CodePrep_CodePrepAPIService.SaveNewZone(input)
            .then(function (response) {

                if (response.Result ==  WhS_CP_NewCPOutputResultEnum.Existing.value) {
                    VRNotificationService.showWarning(response.Message);
                    $scope.zones.length = 0;
                    for (var i = 0; i < response.ZoneItems.length; i++) {
                        $scope.zones.push({ zone: response.ZoneItems[i].Name, message: response.ZoneItems[i].Message });
                    }
                }
                else if (response.Result == WhS_CP_NewCPOutputResultEnum.Inserted.value) {
                    VRNotificationService.showSuccess(response.Message);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == WhS_CP_NewCPOutputResultEnum.Failed.value) {
                    VRNotificationService.showError(response.Message);
                }
                if ($scope.onZoneAdded != undefined)
                    $scope.onZoneAdded(response.ZoneItem);

            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function updateZone() {

        }
    }

    appControllers.controller('whs-codepreparation-newzonedialog', newZoneDialogController);
})(appControllers);
