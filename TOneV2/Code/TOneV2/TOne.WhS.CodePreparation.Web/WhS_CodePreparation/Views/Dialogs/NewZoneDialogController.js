(function (appControllers) {

    "use strict";

    newZoneDialogController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function newZoneDialogController($scope, WhS_BE_SaleZoneAPIService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var zoneId;
        var countryId;
        var editMode;
        var zoneEntity;
        var sellingNumberPlanId;

        var disableCountry;

        defineScope();
        loadParameters();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                zoneId = parameters.ZoneId;
                countryId = parameters.CountryId;
                $scope.countryName = parameters.CountryName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }
            editMode = (zoneId != undefined);
            load();
        }
        function defineScope() {
            $scope.bed;
            $scope.eed;
            $scope.countryName;

            $scope.saveZone = function () {
                if (editMode) {
                    return updateZone();
                }
                else {
                    return insertZone();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {

            $scope.isGettingData = true;
            if (countryId != undefined) {
                $scope.title = UtilsService.buildTitleForAddEditor($scope.countryName + " Zone");
                loadAllControls();
            }
            else if (editMode) {
                getZone().then(function () {
                    loadAllControls()
                        .finally(function () {
                            zoneEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
                $scope.title = UtilsService.buildTitleForAddEditor("Zone");
            }
        }
        function loadAllControls() {
            $scope.isGettingData = false;
        }


        function getZone() {

        }

        function buildZoneObjFromScope() {
            var obj = {
                Name: $scope.name,
                CountryId: countryId
            };
            return obj;
        }

        function getNewZoneFromZoneObj(zoneObj) {
            return {
                SellingNumberPlanId: sellingNumberPlanId,
                NewZone: zoneObj
            }
        }

        function fillScopeFromZoneObj(zone) {
            $scope.name = zone.Name;
            $scope.title = UtilsService.buildTitleForUpdateEditor($scope.name, "Zone");
        }
        function insertZone() {
            var zoneItem = buildZoneObjFromScope();
            //var zoneChanges = [];
            //applyChanges(zoneChanges, zoneItem);
            //var input = {
            //    SellingNumberPlanId: sellingNumberPlanId,
            //    NewChanges: {
            //        NewZones: zoneChanges
            //    }
            //};
            var input = getNewZoneFromZoneObj(zoneItem);
            return WhS_CodePrep_CodePrepAPIService.SaveNewZone(input)
            .then(function (response) {
                if (response.Result == 0) {
                    VRNotificationService.showWarning(response.Message);
                }
                else if (response.Result == 1) {
                    VRNotificationService.showSuccess(response.Message);
                    $scope.modalContext.closeModal();
                }
                if ($scope.onZoneAdded != undefined)
                    $scope.onZoneAdded(response.ZoneItem);

            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
        function updateZone() {

        }

        function applyChanges(zoneChanges, zoneItem) {
            var zoneItemNew = {
                Name: zoneItem.Name,
                CountryId: zoneItem.CountryId,
            };
            //return zoneItemNew;
            zoneChanges.push(zoneItemNew);
        }

    }

    appControllers.controller('whs-codepreparation-newzonedialog', newZoneDialogController);
})(appControllers);
