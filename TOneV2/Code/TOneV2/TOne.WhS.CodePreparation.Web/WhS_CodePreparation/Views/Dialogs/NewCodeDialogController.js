(function (appControllers) {

    "use strict";

    newCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','WhS_CP_NewCPOutputResultEnum'];

    function newCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_NewCPOutputResultEnum) {

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
                $scope.disabledCode = (value == undefined) || UtilsService.getItemIndexByVal($scope.codes, value,"code")!=-1;
            }
            $scope.addCodeValue = function () {
                $scope.codes.push({ code: $scope.codeValue});
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
                    Code:  $scope.codes[i].code,
                    ZoneName: zoneName,
                    BED: $scope.bed,
                    EED: $scope.eed,
                    CountryId: countryId
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
                if (response.Result == WhS_CP_NewCPOutputResultEnum.Existing.value) {
                    VRNotificationService.showWarning(response.Message);

                    $scope.codes.length = 0;
                    for (var i = 0; i < response.CodeItems.length;i++)
                    {
                        $scope.codes.push({ code: response.CodeItems[i].Code, message: response.CodeItems[i].Message});
                    }
                }
                else if (response.Result == WhS_CP_NewCPOutputResultEnum.Inserted.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodeAdded != undefined)
                        $scope.onCodeAdded(response);
                    $scope.modalContext.closeModal();
                }
                else if (response.Result == WhS_CP_NewCPOutputResultEnum.Failed.value) {
                    VRNotificationService.showError(response.Message);
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

    appControllers.controller('WhS_Codepreparation_NewcodedialogController', newCodeDialogController);
})(appControllers);
