(function (appControllers) {

    "use strict";

    RatePlanManagementController.$inject = ["$scope", "WhS_Sales_MainService"];

    function RatePlanManagementController($scope, WhS_Sales_MainService) {
        
        var carrierAccountDirectiveAPI;
        var ratePlanGridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
            };

            $scope.onRatePlanGridReady = function (api) {
                ratePlanGridAPI = api;
            };

            $scope.search = function () {
                return loadRatePlanGrid();
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlanGrid();
                };
                
                WhS_Sales_MainService.sellCustomerZones(carrierAccountDirectiveAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };

            $scope.onCarrierAccountChanged = function () {
                $scope.showRatePlanGrid = false;
                $scope.disableSellNewZonesButton = (carrierAccountDirectiveAPI == undefined || carrierAccountDirectiveAPI.getData() == undefined);
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

        function loadRatePlanGrid() {
            if (ratePlanGridAPI != undefined) {
                $scope.showRatePlanGrid = true;

                var query = {
                    CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId
                };

                return ratePlanGridAPI.loadGrid(query);
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanManagementController", RatePlanManagementController);

})(appControllers);
