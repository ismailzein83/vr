(function (appControllers) {

    "use strict";

    RatePlanController.$inject = ["$scope", "WhS_Sales_MainService", "WhS_Sales_RatePlanAPIService", "WhS_BE_CustomerZoneAPIService", "VRNotificationService"];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_BE_CustomerZoneAPIService, VRNotificationService) {
        
        var carrierAccountDirectiveAPI;
        var ratePlanGridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.zoneLetters = [];
            $scope.ratePlanItems = [];

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
            };

            $scope.onCarrierAccountChanged = function () {
                $scope.showZoneLetters = false;
                $scope.showRatePlanGrid = false;
                $scope.disableSellNewZonesButton = (carrierAccountDirectiveAPI == undefined || carrierAccountDirectiveAPI.getData() == undefined);
            };

            $scope.onRatePlanGridReady = function (api) {
                ratePlanGridAPI = api;
            };

            $scope.search = function () {
                return loadZoneLetters().then(function () {
                    if ($scope.zoneLetters.length > 0) {
                        return loadRatePlanGrid($scope.zoneLetters[0]);
                    }
                });
            };

            $scope.loadRatePlanGrid = function (zoneLetter) {
                loadRatePlanGrid(zoneLetter);
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    return loadZoneLetters().then(function () {
                        if ($scope.zoneLetters.length > 0) {
                            return loadRatePlanGrid($scope.zoneLetters[0]);
                        }
                    });
                };
                
                WhS_Sales_MainService.sellCustomerZones(carrierAccountDirectiveAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };
        }

        function load() {
            if (carrierAccountDirectiveAPI == undefined)
                return;

            $scope.loadingFilters = true;

            carrierAccountDirectiveAPI.load()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose($scope, error);
                })
                .finally(function () {
                    $scope.loadingFilters = false;
                });
        }

        function loadZoneLetters() {
            if (carrierAccountDirectiveAPI != undefined) {
                var customerId = carrierAccountDirectiveAPI.getData().CarrierAccountId;

                return WhS_BE_CustomerZoneAPIService.GetCustomerZoneLetters(customerId).then(function (response) {
                    $scope.zoneLetters = [];

                    for (var i = 0; i < response.length; i++) {
                        $scope.zoneLetters.push(response[i]);
                    }

                    $scope.showZoneLetters = ($scope.zoneLetters.length > 0);
                });
            }
        }

        function loadRatePlanGrid(zoneLetter) {
            if (ratePlanGridAPI != undefined) {
                $scope.ratePlanItems = [];
                $scope.showRatePlanGrid = true;

                var query = {
                    CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId,
                    ZoneLetter: zoneLetter,
                    CountryId: null
                };

                return WhS_Sales_RatePlanAPIService.GetRatePlanItems(query).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.ratePlanItems.push(response[i]);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
