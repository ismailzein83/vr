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
                $scope.title = UtilsService.buildTitleForAddEditor("Code");
            }
        }
        function loadAllControls() {
            $scope.isGettingData = false;
        }


        function getCode() {

        }

        function buildCodeObjFromScope() {
            var obj = {
                Code: $scope.code,
                ZoneName: zoneName,
                BED: $scope.bed,
                EED: $scope.eed
            };
            return obj;
        }

        function getNewCodeFromCodeObj(codeObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                NewCode: codeObj
            }
        }

        function fillScopeFromCodeObj(code) {
            $scope.name = code.Code;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.code, "Code");
        }
        function insertCode() {
            var codeItem = buildCodeObjFromScope();

            var input = getNewCodeFromCodeObj(codeItem);
            return WhS_CodePrep_CodePrepAPIService.SaveNewCode(input)
            .then(function (response) {
                if (response.Result == 0) {
                    VRNotificationService.showWarning(response.Message);
                }
                else if (response.Result == 1) {
                    VRNotificationService.showSuccess(response.Message);
                }
                if ($scope.onCodeAdded != undefined)
                    $scope.onCodeAdded(codeItem);
                $scope.modalContext.closeModal();
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
