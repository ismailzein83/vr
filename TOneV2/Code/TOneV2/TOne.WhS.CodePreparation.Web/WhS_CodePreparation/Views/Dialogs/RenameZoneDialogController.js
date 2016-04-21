(function (appControllers) {

    "use strict";

    RenameZoneDialogController.$inject = ['$scope', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_NewCPOutputResultEnum', 'WhS_CP_ValidationOutput', 'WhS_CP_CodePrepService'];

    function RenameZoneDialogController($scope, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_CPOutputResultEnum, WhS_CP_ValidationOutput, WhS_CP_CodePrepService) {

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
                $scope.modalContext.closeModal()
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
            return WhS_CP_CodePrepAPIService.RenameZone(input)
            .then(function (response) {
                if (response.Result == WhS_CP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    $scope.onZoneRenamed(response.Zone);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                    WhS_CP_CodePrepService.NotifyValidationWarning(response.Message);
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('Whs_CP_RenameZoneDialogController', RenameZoneDialogController);
})(appControllers);
