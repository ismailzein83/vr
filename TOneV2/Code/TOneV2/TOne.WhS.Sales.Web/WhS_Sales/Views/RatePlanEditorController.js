(function (appControllers) {

    "use strict";

    ratePlanEditorController.$inject = ["$scope", "WhS_BE_CustomerZoneAPIService", "WhS_BE_SaleZoneAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function ratePlanEditorController($scope, WhS_BE_CustomerZoneAPIService, WhS_BE_SaleZoneAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var customerId;
        var gridAPI;
        var selectedSaleZones;

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

            $scope.saleZones = [];
            $scope.disableSaveButton = true;

            $scope.onGridReady = function (api) {
                gridAPI = api;
                return gridAPI.retrieveData(getQuery());
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_CustomerZoneAPIService.GetFilteredSaleZonesToSell(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            $scope.onSearchClicked = function () {
                if (gridAPI != undefined) {
                    return gridAPI.retrieveData(getQuery());
                }
            };

            $scope.onSaleZoneCheckedChanged = function () {
                var saleZone = UtilsService.getItemByVal($scope.saleZones, true, "isSelected");
                // disable the add button if no sale zone is selected
                $scope.disableSaveButton = (saleZone == undefined);
            };

            $scope.sellNewZones = function () {
                var customerZonesObj = buildCutomerZonesObjFromScope();

                WhS_BE_CustomerZoneAPIService.AddCustomerZones(customerZonesObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Customer Zones", response)) {
                        if ($scope.onCustomerZonesSold != undefined) {
                            $scope.onCustomerZonesSold(response);
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

        }

        function getQuery() {
            return {
                CustomerId: customerId,
                Name: $scope.name
            };
        }

        function buildCutomerZonesObjFromScope() {
            return {
                CustomerId: customerId,
                Zones: getSelectedCustomerZones(),
                StartEffectiveTime: new Date()
            };
        }

        function getSelectedCustomerZones() {
            var newCustomerZones = [];

            for (var i = 0; i < $scope.saleZones.length; i++) {
                if ($scope.saleZones[i].isSelected) {
                    newCustomerZones.push({
                        ZoneId: $scope.saleZones[i].Entity.SaleZoneId
                    });
                }
            }

            if (newCustomerZones.length > 0) return newCustomerZones;
            return null;
        }
    }

    appControllers.controller("WhS_Sales_RatePlanEditorController", ratePlanEditorController);

})(appControllers);
