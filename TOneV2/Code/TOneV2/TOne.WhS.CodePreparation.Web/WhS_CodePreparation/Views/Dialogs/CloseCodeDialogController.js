(function (appControllers) {

    "use strict";

    closeCodeDialogController.$inject = ['$scope', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_ValidationOutput', 'WhS_CodePrep_CodePrepService'];

    function closeCodeDialogController($scope, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_ValidationOutput, WhS_CodePrep_CodePrepService) {

        var countryId;
        var sellingNumberPlanId;
        var currentZoneName;
        var zoneId;


        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                $scope.codes = parameters.Codes;
                countryId = parameters.CountryId;
                currentZoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }

            load();
        }
        function defineScope() {
            $scope.codes = [];
            $scope.closeCodes = function () {
                return closeCodes();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.isGettingData = true;
            loadAllControls();
            $scope.title = "Close Codes for " + currentZoneName;
        }
        function loadAllControls() {
            $scope.isLoading = false;
            $scope.isGettingData = false;
        }

        function buildCodeCloseObjFromScope() {
            var obj = {
                Codes: $scope.codes,
                CurrentZoneName: currentZoneName
            };
            return obj;
        }

        function getCloseCodeInput(codeObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                Codes: codeObj.Codes,
                ZoneName: codeObj.CurrentZoneName
            }
        }

        function closeCodes() {
            var closeItem = buildCodeCloseObjFromScope();
            var input = getCloseCodeInput(closeItem);
            return WhS_CodePrep_CodePrepAPIService.CloseCodes(input)
            .then(function (response) {
                if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                    WhS_CodePrep_CodePrepService.NotifyValidationWarning(response.Message);
                }
                else if (response.Result == WhS_CP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodesClosed != undefined)
                        $scope.onCodesClosed(response.NewCodes);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('whs-codepreparation-closecodedialog', closeCodeDialogController);
})(appControllers);
