(function (appControllers) {

    "use strict";

    moveCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function moveCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var countryId;
        var codeEntity;
        var sellingNumberPlanId;
        var currentZoneName;
        var zoneId;
        
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                $scope.saleZones = parameters.ZoneDataSource;
            }

            load();
        }
        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.codes = [];
            $scope.saleZones;
            $scope.selectedZone;
            $scope.moveCodes = function () {
                return moveCodes();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

        }

        function load() {

            $scope.isGettingData = true;
            loadAllControls();
            $scope.title = UtilsService.buildTitleForAddEditor("Move Code for " + currentZoneName);
        }
        function loadAllControls() {
            $scope.isLoading = false;
            $scope.isGettingData = false;
        }


        function loadSaleZones() {
            var loadSaleZonesPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                var payload = {};
                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonesPromiseDeferred);
            });

            return loadSaleZonesPromiseDeferred.promise;
        }

        function buildCodeMoveObjFromScope() {
            var obj = {
                Codes: $scope.codes,
                CurrentZoneName: currentZoneName,
                NewZoneName: $scope.selectedZone.Name,
                BED: $scope.bed,
                EED: $scope.eed
            };
            return obj;
        }

        function getMoveCodeInput(codeObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                Codes: codeObj.Codes,
                CurrentZoneName: codeObj.CurrentZoneName,
                NewZoneName: codeObj.NewZoneName,
                BED: codeObj.BED,
                EED: codeObj.EED
            }
        }

        function moveCodes() {
            var moveItem = buildCodeMoveObjFromScope();
            var input = getMoveCodeInput(moveItem);
            return WhS_CodePrep_CodePrepAPIService.MoveCodes(input)
            .then(function (response) {
                if (response.Result == 0) {
                    VRNotificationService.showWarning(response.Message);
                }
                else if (response.Result == 1) {
                    VRNotificationService.showSuccess(response.Message);
                    if ($scope.onCodesMoved != undefined)
                        $scope.onCodesMoved(response);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('whs-codepreparation-movecodedialog', moveCodeDialogController);
})(appControllers);
