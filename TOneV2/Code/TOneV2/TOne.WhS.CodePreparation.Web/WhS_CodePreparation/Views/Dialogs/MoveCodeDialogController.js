(function (appControllers) {

    "use strict";

    moveCodeDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function moveCodeDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var countryId;
        var codeEntity;
        var sellingNumberPlanId;
        var zoneName;
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
                zoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }

            load();
        }
        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.codes;

            $scope.moveCode = function () {

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
            if (zoneId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor("Move Code for " + zoneName);
                loadAllControls();
            }
            else {
                loadAllControls();
                $scope.title = UtilsService.buildTitleForAddEditor("Move Code for " + zoneName);
            }
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSaleZones])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }


        function loadSaleZones() {
            var loadSaleZonesPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                var payload = {};
                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonesPromiseDeferred);
            });

            return loadSaleZonesPromiseDeferred.promise;
        }


        function getCode() {

        }

        function buildCodeObjFromScope() {
            var obj = {
                Codes: $scope.codes,
                ZoneName: zoneName,
                BED: $scope.bed,
                EED: $scope.eed
            };
            return obj;
        }

        function getNewCodeFromCodeObj(codeObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                CountryId: countryId,
                ZoneId: zoneId,
                NewCode: codeObj
            }
        }

        function fillScopeFromCodeObj(code) {
            $scope.name = code.Code;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.code, "Move Code for " + zoneName);
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
                    if ($scope.onCodeAdded != undefined)
                        $scope.onCodeAdded(codeItem);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('whs-codepreparation-movecodedialog', moveCodeDialogController);
})(appControllers);
