(function (appControllers) {

    "use strict";

    newCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function newCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var countryId;
        var editMode;
        var codeEntity;
        var sellingNumberPlanId;
        var zoneName;
        var zoneId;
        defineScope();
        loadParameters();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                $scope.code = parameters.Code;
                countryId = parameters.CountryId;
                zoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
            editMode = ($scope.code != undefined);
            load();
        }
        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.code;
            $scope.codeValue;
            $scope.codes = [];
            $scope.saveCode = function () {
                if (editMode) {
                    return updateCode();
                }
                else {
                    return insertCode();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.disabledCode = true;
            $scope.onCodeValueChange = function (value) {
                $scope.disabledCode = (value == undefined) || UtilsService.contains($scope.codes, value);
            }
            $scope.addCodeValue = function () {
                $scope.codes.push($scope.codeValue);
                $scope.codeValue = undefined;
                $scope.disabledCode = true;
            };

            $scope.ValidateCodes = function () {
                if ($scope.codes != undefined && $scope.codes.length == 0)
                    return "Enter at least one code.";
                return null;
            };
        }

        function load() {

            $scope.isGettingData = true;
            if (zoneId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor("Code for " + zoneName);
                loadAllControls();
            }
            else if (editMode) {
                getCode().then(function () {
                    loadAllControls()
                        .finally(function () {
                            codeEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
                $scope.title = UtilsService.buildTitleForAddEditor("Code for " + zoneName);
            }
        }
        function loadAllControls() {
            $scope.isGettingData = false;
        }


        function getCode() {

        }

        function buildCodesObjFromScope() {
            var result = [];
            for (var i = 0; i < $scope.codes.length ; i++) {
                result.push({
                    Code: $scope.codes[i],
                    ZoneName: zoneName,
                    BED: $scope.bed,
                    EED: $scope.eed
                });
            }
            return result;
        }

        function getNewCodeFromCodeObj(codeItems) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                ZoneId: zoneId,
                NewCodes: codeItems,
            }
        }

        function fillScopeFromCodeObj(code) {
            $scope.name = code.Code;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.code, "Code for " + zoneName);
        }
        function insertCode() {
            var codeItems = buildCodesObjFromScope();

            var input = getNewCodeFromCodeObj(codeItems);
            return WhS_CodePrep_CodePrepAPIService.SaveNewCode(input)
            .then(function (response) {
                if (response.Result == 0) {
                    VRNotificationService.showWarning(response.Message);
                }
                else if (response.Result == 1) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodeAdded != undefined)
                        $scope.onCodeAdded(response);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
        function updateCode() {

        }

        function applyChanges(codeChanges, codeItem) {
            var codeItemNew = {

            };
            codeChanges.push(codeItemNew);
        }

    }

    appControllers.controller('whs-codepreparation-newcodedialog', newCodeDialogController);
})(appControllers);
