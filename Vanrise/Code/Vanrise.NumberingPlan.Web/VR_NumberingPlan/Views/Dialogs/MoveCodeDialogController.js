(function (appControllers) {

    "use strict";

    MoveCodeDialogController.$inject = ['$scope', 'VR_NumberingPlan_ModuleConfig', 'Vr_NP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Vr_NP_ValidationOutput', 'Vr_NP_CodePrepService', 'Vr_NP_ZoneItemDraftStatusEnum', 'Vr_NP_ZoneItemStatusEnum'];

    function MoveCodeDialogController($scope, VR_NumberingPlan_ModuleConfig, Vr_NP_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Vr_NP_ValidationOutput, Vr_NP_CodePrepService, Vr_NP_ZoneItemDraftStatusEnum, Vr_NP_ZoneItemStatusEnum) {

        var countryId;
        var sellingNumberPlanId;
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
				$scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.title = "Move Code from " + zoneName;
            $scope.isGettingData = true;
            loadAllControls();
        }

        function loadAllControls() {
            loadSaleZoneSelector();
            $scope.isGettingData = false;
        }


        function loadSaleZoneSelector() {
            //removing current zone and closed, pending zones from to zones
            for (var i = 0; i < zoneItems.length ; i++) {
                var zoneItem = zoneItems[i];
                if (zoneItem.DraftStatus != Vr_NP_ZoneItemDraftStatusEnum.ExistingClosed.value && zoneItem.nodeName != zoneName && zoneItem.status == null)
                      $scope.saleZones.push({ "Name": zoneItems[i].nodeName });
            }
        }


        function buildCodeMoveObjFromScope() {
            var zoneItemIndex = UtilsService.getItemIndexByVal(zoneItems, $scope.selectedZone.Name, "nodeName");
            var zoneItem = zoneItems[zoneItemIndex];


            var obj = {
                Codes: $scope.codes,
                CurrentZoneName: zoneName,
                NewZoneName: zoneItem.nodeName
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
            };
        }

        function moveCodes() {
            var input = getMoveCodeInput();
            return Vr_NP_CodePrepAPIService.MoveCodes(input)
            .then(function (response) {
                if (response.Result == Vr_NP_ValidationOutput.ValidationError.value) {
                    Vr_NP_CodePrepService.NotifyValidationWarning(response.Message);
                }
                else if (response.Result == Vr_NP_ValidationOutput.Success.value) {
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

    appControllers.controller('Vr_NP_MoveCodeDialogController', MoveCodeDialogController);
})(appControllers);
