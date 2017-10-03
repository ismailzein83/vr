(function (appControllers) {

    "use strict";

    saleZoneEditorController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function saleZoneEditorController($scope, WhS_BE_SaleZoneAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var saleZoneId;
        var saleZoneName;
        var editMode;
        var sellingNumberPlanId;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                saleZoneId = parameters.SaleZoneId;
                saleZoneName = parameters.Name;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
            }

            editMode = (saleZoneId != undefined);
        }

        function defineScope() {

            $scope.updateSaleZoneName = function () {
                $scope.isGettingData = true;
                return WhS_BE_SaleZoneAPIService.UpdateSaleZoneName(saleZoneId,$scope.name,sellingNumberPlanId)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Sale Zone", response, "Name")) {
                        if ($scope.onSaleZoneUpdated != undefined)
                            $scope.onSaleZoneUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            };



            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.name = saleZoneName;
            $scope.title = UtilsService.buildTitleForUpdateEditor(saleZoneName, "Sale Zone");
        }

    }

    appControllers.controller('WhS_BE_SaleZoneEditorController', saleZoneEditorController);
})(appControllers);
