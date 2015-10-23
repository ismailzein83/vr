(function (appControllers) {

    "use strict";

    ratePlanEditorController.$inject = ["$scope", "WhS_BE_SaleZoneAPIService", "WhS_BE_CustomerZoneAPIService", "UtilsService", "VRNavigationService"];

    function ratePlanEditorController($scope, WhS_BE_SaleZoneAPIService, WhS_BE_CustomerZoneAPIService, UtilsService, VRNavigationService) {

        var customerId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                customerId = parameters.CustomerId;
            }
        }

        function defineScope() {
            $scope.title = UtilsService.buildTitleForUpdateEditor("Rate Plan");

            $scope.selectedSaleZones = [];

            $scope.getSaleZonesByName = function (saleZoneNameFilter) {
                return WhS_BE_SaleZoneAPIService.GetSaleZonesByName(customerId, saleZoneNameFilter);
            }

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.loadingEditor = true;

            WhS_BE_CustomerZoneAPIService.GetCustomerZone(customerId).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.Data.Zones.length; i++) {
                        $scope.selectedSaleZones.push(response.Data.Zones[i].ZoneId);
                    }
                }
            }).finally(function () {
                $scope.loadingEditor = false;
            });
        }
    }

    appControllers.controller("WhS_Sales_RatePlanEditorController", ratePlanEditorController);

})(appControllers);
