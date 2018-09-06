(function (appControllers) {

    "use strict";

    RenameZoneDialogController.$inject = ['$scope', 'Vr_NP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vr_NP_NewCPOutputResultEnum', 'Vr_NP_ValidationOutput', 'Vr_NP_CodePrepService'];

    function RenameZoneDialogController($scope, Vr_NP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vr_NP_NewCPOutputResultEnum, Vr_NP_ValidationOutput, Vr_NP_CodePrepService) {

        var countryId;
        var sellingNumberPlanId;
        var oldZoneName;
        var zoneId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                oldZoneName = parameters.ZoneName;
                countryId = parameters.CountryId;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
        }

        function defineScope() {
            $scope.zoneName = oldZoneName;

            $scope.renameZone = function () {
                return renameZone();
            };

            $scope.close = function () {
				$scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.title = "Rename " + oldZoneName + " Zone";
        }

        function buildRenamedZoneObjFromScope() {
            var obj = {
                SellingNumberPlanId: sellingNumberPlanId,
                ZoneId: zoneId,
                CountryId: countryId,
                OldZoneName: oldZoneName,
                NewZoneName: $scope.zoneName
            };
            return obj;
        }


        function renameZone() {
            var input = buildRenamedZoneObjFromScope();
            return Vr_NP_CodePrepAPIService.RenameZone(input)
            .then(function (response) {
                if (response.Result == Vr_NP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    $scope.onZoneRenamed(response.Zone);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == Vr_NP_ValidationOutput.ValidationError.value) {
                    Vr_NP_CodePrepService.NotifyValidationWarning(response.Message);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('Vr_NP_RenameZoneDialogController', RenameZoneDialogController);
})(appControllers);
