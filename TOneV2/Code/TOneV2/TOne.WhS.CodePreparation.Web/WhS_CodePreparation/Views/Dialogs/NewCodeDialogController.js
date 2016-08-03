(function (appControllers) {

    "use strict";

    NewCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_ValidationOutput', 'WhS_CP_CodePrepService'];

    function NewCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_ValidationOutput, WhS_CP_CodePrepService) {

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
                $scope.modalContext.closeModal()
            };

            $scope.disabledCode = true;

            $scope.onCodeValueChange = function (value) {
                $scope.disabledCode = value == undefined && $scope.codeValue.length - 1 < 1; 
            }

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
                NewCodes: buildCodesObjFromScope(),
            }
        }

        function insertCode() {
            var input = getNewCodeFromCodeObj();
            return WhS_CP_CodePrepAPIService.SaveNewCode(input)
            .then(function (response) {
                if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                    WhS_CP_CodePrepService.NotifyValidationWarning(response.Message);
                    $scope.codes.length = 0;
                    for (var i = 0; i < response.CodeItems.length; i++) {
                        $scope.codes.push({ code: response.CodeItems[i].Code, message: response.CodeItems[i].Message });
                    }
                }
                else if (response.Result == WhS_CP_ValidationOutput.Success.value) {
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

    appControllers.controller('WhS_CP_NewCodeDialogController', NewCodeDialogController);
})(appControllers);
