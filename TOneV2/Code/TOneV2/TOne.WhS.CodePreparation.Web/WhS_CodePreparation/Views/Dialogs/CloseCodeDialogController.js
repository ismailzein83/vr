(function (appControllers) {

    "use strict";

    CloseCodeDialogController.$inject = ['$scope', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_ValidationOutput', 'WhS_CP_CodePrepService'];

    function CloseCodeDialogController($scope, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_ValidationOutput, WhS_CP_CodePrepService) {

        var countryId;
        var sellingNumberPlanId;
        var currentZoneName;
        var zoneId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                $scope.codes = parameters.Codes;
                countryId = parameters.CountryId;
                currentZoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
        }


        function defineScope() {

            $scope.closeCodes = function () {
                return closeCodes();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.title = "Close Codes for " + currentZoneName;
        }


        function getCloseCodeInput() {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                Codes: $scope.codes,
                ZoneName: currentZoneName,
                ZoneId: zoneId
            };
        }

        function closeCodes() {
            var input = getCloseCodeInput();
            return WhS_CP_CodePrepAPIService.CloseCodes(input)
            .then(function (response) {
                if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                    WhS_CP_CodePrepService.NotifyValidationWarning(response.Message);
                }
                else if (response.Result == WhS_CP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodesClosed != undefined)
                        $scope.onCodesClosed(response.ClosedCodes);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('Whs_CP_CloseCodeDialogController', CloseCodeDialogController);
})(appControllers);
