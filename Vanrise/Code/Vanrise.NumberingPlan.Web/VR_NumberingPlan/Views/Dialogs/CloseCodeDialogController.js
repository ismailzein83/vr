(function (appControllers) {

    "use strict";

    CloseCodeDialogController.$inject = ['$scope', 'Vr_NP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vr_NP_ValidationOutput', 'Vr_NP_CodePrepService'];

    function CloseCodeDialogController($scope, Vr_NP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vr_NP_ValidationOutput, Vr_NP_CodePrepService) {

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
            return Vr_NP_CodePrepAPIService.CloseCodes(input)
            .then(function (response) {
                if (response.Result == Vr_NP_ValidationOutput.ValidationError.value) {
                    Vr_NP_CodePrepService.NotifyValidationWarning(response.Message);
                }
                else if (response.Result == Vr_NP_ValidationOutput.Success.value) {
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

    appControllers.controller('Vr_NP_CloseCodeDialogController', CloseCodeDialogController);
})(appControllers);
