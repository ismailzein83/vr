(function (appControllers) {

    "use strict";

    NewZoneDialogController.$inject = ['$scope', 'VR_NumberingPlan_ModuleConfig', 'Vr_NP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vr_NP_ValidationOutput', 'Vr_NP_CodePrepService'];

    function NewZoneDialogController($scope, VR_NumberingPlan_ModuleConfig, Vr_NP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vr_NP_ValidationOutput, Vr_NP_CodePrepService) {

        var zoneId;
        var countryId;
        var countryName;
        var sellingNumberPlanId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                countryId = parameters.CountryId;
                countryName = parameters.CountryName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
        }

        function defineScope() {

            $scope.zones = [];

            $scope.validateZones = function () {
                if ($scope.zones != undefined && $scope.zones.length == 0)
                    return "Enter at least one zone.";
                return null;
            };

            $scope.saveZone = function () {
                return insertZone();
            };


            $scope.close = function () {
				$scope.modalContext.closeModal();
            };

            $scope.disabledZone = true;

            $scope.onZoneValueChange = function (value) {
                $scope.disabledZone = (value == undefined) || UtilsService.getItemIndexByStringVal($scope.zones, value, "zone", true) != -1;
            };

            $scope.addZoneValue = function () {
                $scope.zones.push({ zone: $scope.zoneValue });
                $scope.zoneValue = undefined;
                $scope.disabledZone = true;
            };
        }

        function load() {
            $scope.title = UtilsService.buildTitleForAddEditor("Zone for Country " + countryName);
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

        function getNewZoneFromZoneObj() {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                NewZones: buildZoneObjFromScope()
            };
        }

        function insertZone() {
            var input = getNewZoneFromZoneObj();
            return Vr_NP_CodePrepAPIService.SaveNewZone(input)
            .then(function (response) {
                if (response.Result == Vr_NP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onZoneAdded != undefined)
                        $scope.onZoneAdded(response.ZoneItems);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == Vr_NP_ValidationOutput.ValidationError.value) {
                    $scope.zones.length = 0;
                    for (var i = 0; i < response.ZoneItems.length; i++) {
                        $scope.zones.push({ zone: response.ZoneItems[i].Name, message: response.ZoneItems[i].Message });
                    }
                    Vr_NP_CodePrepService.NotifyValidationWarning(response.Message);
                }


            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('Vr_NP_NewZoneDialogController', NewZoneDialogController);
})(appControllers);
