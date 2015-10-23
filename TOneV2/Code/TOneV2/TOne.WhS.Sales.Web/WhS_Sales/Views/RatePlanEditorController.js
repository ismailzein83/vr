(function (appControllers) {

    "use strict";

    ratePlanEditorController.$inject = ["$scope", "WhS_BE_SaleZoneAPIService", "WhS_BE_CustomerZoneAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function ratePlanEditorController($scope, WhS_BE_SaleZoneAPIService, WhS_BE_CustomerZoneAPIService, UtilsService, VRNavigationService, VRNotificationService) {

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
            $scope.title = "Sell New Zones";
            //$scope.title = UtilsService.buildTitleForUpdateEditor("Rate Plan");

            $scope.selectedSaleZones = [];

            $scope.getSaleZonesByName = function (saleZoneNameFilter) {
                return WhS_BE_SaleZoneAPIService.GetSaleZonesByName(customerId, saleZoneNameFilter);
            };

            $scope.sellNewZones = function () {
                var customerZonesObj = buildCutomerZonesObjFromScope();

                console.log(customerZonesObj);

                WhS_BE_CustomerZoneAPIService.AddCustomerZones(customerZonesObj).then(function (response) {
                    console.log(response);

                    if (VRNotificationService.notifyOnItemAdded("Customer Zones", response)) {
                        if ($scope.onCustomerZonesAdded != undefined) {
                            $scope.onCustomerZonesAdded();
                        }

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.loadingEditor = true;

            WhS_BE_CustomerZoneAPIService.GetCustomerZone(customerId).then(function (response) {
                console.log(response);
                fillScopeFromCustomerZonesObj(response);
            }).finally(function () {
                $scope.loadingEditor = false;
            });
        }

        function fillScopeFromCustomerZonesObj(customerZones) {
        }

        function buildCutomerZonesObjFromScope() {
            return {
                CustomerId: customerId,
                Zones: getCustomerZoneArray(),
                StartEffectiveTime: Date()
            };
        }

        function getCustomerZoneArray() {
            var customerZones = [];

            for (var i = 0; i < $scope.selectedSaleZones.length; i++) {
                customerZones.push({
                    ZoneId: $scope.selectedSaleZones[i].SaleZoneId
                });
            }

            return customerZones;
        }
    }

    appControllers.controller("WhS_Sales_RatePlanEditorController", ratePlanEditorController);

})(appControllers);
