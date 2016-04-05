(function (appControllers) {

    "use strict";

    renameZoneDialogController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_NewCPOutputResultEnum'];

    function renameZoneDialogController($scope, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_CPOutputResultEnum) {

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

            $scope.isGettingData = true;
            loadAllControls();
            $scope.title = "Rename " + oldZoneName + " Zone";
        }
        function loadAllControls() {
            $scope.isLoading = false;
            $scope.isGettingData = false;
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
        return WhS_CodePrep_CodePrepAPIService.RenameZone(input)
        .then(function (response) {
            if (response.Result == WhS_CP_CPOutputResultEnum.Existing.value) {
                VRNotificationService.showWarning(response.Message);
            }
            else if (response.Result == WhS_CP_CPOutputResultEnum.Inserted.value) {
                VRNotificationService.showSuccess(response.Message);
                $scope.onZoneRenamed(response.Zones);
                $scope.modalContext.closeModal();
            }
            else if (response.Result == WhS_CP_CPOutputResultEnum.Failed.value) {
                VRNotificationService.showError(response.Message);
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}

    appControllers.controller('whs-codepreparation-renamezonedialog', renameZoneDialogController);
})(appControllers);
