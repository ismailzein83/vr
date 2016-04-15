(function (appControllers) {

    "use strict";

    moveCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_NewCPOutputResultEnum','WhS_CP_ZoneItemDraftStatusEnum'];

    function moveCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_CPOutputResultEnum,WhS_CP_ZoneItemDraftStatusEnum) {

        var countryId;
        var sellingNumberPlanId;
        var currentZoneName;
        var zoneName;
        var zoneId;
        var zoneItems;
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
                zoneName = parameters.ZoneName;
                currentZoneName = parameters.currentZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
                zoneItems = parameters.ZoneDataSource;
               
            }
        }

        function defineScope() {
            $scope.selectedZone;
            $scope.saleZones = [];

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
            loadSaleZoneSelector();
            $scope.isLoading = false;
            $scope.isGettingData = false;
        }


        function loadSaleZoneSelector() {

            for (var i = 0; i < zoneItems.length ; i++) {
                if (zoneItems[i].DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value)
                    $scope.saleZones.push({ "Name": zoneItems[i].nodeName });
            }

            var index = UtilsService.getItemIndexByVal($scope.saleZones, currentZoneName, "Name");

            if (index != -1)
                $scope.saleZones.splice(index, 1);

        }

     
        function buildCodeMoveObjFromScope() {
            var zoneItemIndex = UtilsService.getItemIndexByVal(zoneItems, $scope.selectedZone.Name, "nodeName");
            var zoneItem = zoneItems[zoneItemIndex];

            
            var obj = {
                Codes: $scope.codes,
                CurrentZoneName:zoneName.toLowerCase() != currentZoneName.toLowerCase() ? zoneName :currentZoneName ,
                NewZoneName: zoneItem.originalZoneName != null ? zoneItem.originalZoneName : zoneItem.nodeName
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
