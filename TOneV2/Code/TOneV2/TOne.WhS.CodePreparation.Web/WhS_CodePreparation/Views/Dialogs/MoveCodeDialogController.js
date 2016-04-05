(function (appControllers) {

    "use strict";

    moveCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_NewCPOutputResultEnum'];

    function moveCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_CPOutputResultEnum) {

        var countryId;
        var sellingNumberPlanId;
        var currentZoneName;
        var zoneId;        
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
                $scope.saleZones = parameters.ZoneDataSource;

                var index = UtilsService.getItemIndexByVal($scope.saleZones, currentZoneName, "Name");
                if(index !=-1)
                  $scope.saleZones.splice(index, 1);
            }

            
        }
        function defineScope() {
            $scope.selectedZone;
            $scope.saveMoveCodes = function () {
                return moveCodes();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.isGettingData = true;
            loadAllControls();
            $scope.title = "Move Code from " + currentZoneName;
        }
        function loadAllControls() {
            $scope.isLoading = false;
            $scope.isGettingData = false;
        }

     
        function buildCodeMoveObjFromScope() {
            var obj = {
                Codes: $scope.codes,
                CurrentZoneName: currentZoneName,
                NewZoneName: $scope.selectedZone.Name
            };
            return obj;
        }

        function getMoveCodeInput(codeObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                Codes: codeObj.Codes,
                CurrentZoneName: codeObj.CurrentZoneName,
                NewZoneName: codeObj.NewZoneName
            }
        }

        function moveCodes() {
            var moveItem = buildCodeMoveObjFromScope();
            var input = getMoveCodeInput(moveItem);
            return WhS_CodePrep_CodePrepAPIService.MoveCodes(input)
            .then(function (response) {
                if (response.Result == WhS_CP_CPOutputResultEnum.Existing.value) {
                    VRNotificationService.showWarning(response.Message);
                }
                else if (response.Result == WhS_CP_CPOutputResultEnum.Inserted.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodesMoved != undefined)
                        $scope.onCodesMoved(response.NewCodes);
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

    appControllers.controller('whs-codepreparation-movecodedialog', moveCodeDialogController);
})(appControllers);
