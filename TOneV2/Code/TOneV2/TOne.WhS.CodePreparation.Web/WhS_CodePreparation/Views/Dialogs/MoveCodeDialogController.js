(function (appControllers) {

    "use strict";

    MoveCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_CP_ValidationOutput', 'WhS_CP_CodePrepService', 'WhS_CP_ZoneItemDraftStatusEnum'];

    function MoveCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_CP_ValidationOutput, WhS_CP_CodePrepService, WhS_CP_ZoneItemDraftStatusEnum) {

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
            $scope.title = "Move Code from " + currentZoneName;
            $scope.isGettingData = true;
            loadAllControls();
        }

        function loadAllControls() {
            loadSaleZoneSelector();
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
                CurrentZoneName: zoneName.toLowerCase() != currentZoneName.toLowerCase() ? zoneName : currentZoneName,
                NewZoneName: zoneItem.originalZoneName != null ? zoneItem.originalZoneName : zoneItem.nodeName
            };
            return obj;
        }

        function getMoveCodeInput() {
            var moveItem = buildCodeMoveObjFromScope();
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                Codes: moveItem.Codes,
                CurrentZoneName: moveItem.CurrentZoneName,
                NewZoneName: moveItem.NewZoneName,
                ZoneId: zoneId
            }
        }

        function moveCodes() {
            var input = getMoveCodeInput();
            return WhS_CP_CodePrepAPIService.MoveCodes(input)
            .then(function (response) {
                if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
                    WhS_CP_CodePrepService.NotifyValidationWarning(response.Message);
                }
                else if (response.Result == WhS_CP_ValidationOutput.Success.value) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodesMoved != undefined)
                        $scope.onCodesMoved(response.NewCodes);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('Whs_CP_MoveCodeDialogController', MoveCodeDialogController);
})(appControllers);
