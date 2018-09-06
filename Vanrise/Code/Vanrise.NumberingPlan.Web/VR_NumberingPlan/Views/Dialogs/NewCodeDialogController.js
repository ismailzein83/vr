(function (appControllers) {

    "use strict";

    NewCodeDialogController.$inject = ['$scope', 'VR_NumberingPlan_ModuleConfig', 'Vr_NP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vr_NP_ValidationOutput', 'Vr_NP_CodePrepService'];

    function NewCodeDialogController($scope, VR_NumberingPlan_ModuleConfig, Vr_NP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vr_NP_ValidationOutput, Vr_NP_CodePrepService) {

        var countryId;
        var sellingNumberPlanId;
        var zoneName;
        var zoneId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                $scope.code = parameters.Code;
                countryId = parameters.CountryId;
                zoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }

        }

        function defineScope() {
            $scope.code;
            $scope.codeValue;
            $scope.codes = [];

            $scope.saveCode = function () {
                return insertCode();
            };

            $scope.close = function () {
				$scope.modalContext.closeModal();
            };

            $scope.disabledCode = true;

            $scope.onCodeValueChange = function (value) {
                if (value == undefined && $scope.codeValue.length > 0) {
                    var code = $scope.codeValue.substring(0, $scope.codeValue.length - 1);
                    $scope.disabledCode = UtilsService.getItemIndexByVal($scope.codes, code.trim(), "code") != -1;
                }

                if (value != undefined)
                    $scope.disabledCode = UtilsService.getItemIndexByVal($scope.codes, value, "code") != -1;
            };

            $scope.addCodeValue = function () {
                $scope.codes.push({ code: $scope.codeValue });
                $scope.codeValue = undefined;
                $scope.disabledCode = true;
            };

            $scope.validateCodes = function () {
                if ($scope.codes != undefined && $scope.codes.length == 0)
                    return "Enter at least one code.";
                return null;
            };
        }

        function load() {
            $scope.title = UtilsService.buildTitleForAddEditor("Code for " + zoneName);
        }


        function buildCodesObjFromScope() {
            var result = [];
            for (var i = 0; i < $scope.codes.length ; i++) {
                result.push({
                    Code: $scope.codes[i].code,
                    ZoneName: zoneName,
                    ZoneId: zoneId,
                    CountryId: countryId
                });
            }
            return result;
        }

        function getNewCodeFromCodeObj() {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                ZoneId: zoneId,
                NewCodes: buildCodesObjFromScope()
            };
        }

        function insertCode() {
            var input = getNewCodeFromCodeObj();
            return Vr_NP_CodePrepAPIService.SaveNewCode(input)
            .then(function (response) {
                if (response.Result == Vr_NP_ValidationOutput.ValidationError.value) {
                    Vr_NP_CodePrepService.NotifyValidationWarning(response.Message);
                    $scope.codes.length = 0;
                    for (var i = 0; i < response.CodeItems.length; i++) {
                        $scope.codes.push({ code: response.CodeItems[i].Code, message: response.CodeItems[i].Message });
                    }
                }
                else if (response.Result == Vr_NP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodeAdded != undefined)
                        $scope.onCodeAdded(response.CodeItems);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('Vr_NP_NewCodeDialogController', NewCodeDialogController);
})(appControllers);
