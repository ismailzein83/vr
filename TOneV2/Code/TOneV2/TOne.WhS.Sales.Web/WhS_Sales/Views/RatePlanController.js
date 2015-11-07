(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_MainService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_Sales_RatePlanOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRNotificationService) {
        
        var sellingProductSelectorAPI;
        var carrierAccountSelectorAPI;
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            /* Owner Type */

            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Sales_RatePlanOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                clearAndHideRatePlan();

                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_Sales_RatePlanOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.carrierAccountConnector.selectedCustomer = undefined;
                        carrierAccountSelectorAPI = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_RatePlanOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.sellingProductConnector.selectedSellingProduct = undefined;
                        sellingProductSelectorAPI = undefined;
                    }
                }
            };

            /* Selling Product */

            $scope.sellingProductConnector = {
                selectedSellingProduct: undefined,

                onSellingProductSelectorReady: function (api) {
                    sellingProductSelectorAPI = api;
                    api.load(undefined);
                },

                onSellingProductChanged: function () {
                    clearAndHideRatePlan();
                }
            };

            /* Carrier Account */

            $scope.carrierAccountConnector = {
                selectedCustomer: undefined,
                
                onCarrierAccountSelectorReady: function (api) {
                    carrierAccountSelectorAPI = api;
                    api.load(undefined);
                },
                
                onCarrierAccountChanged: function () {
                    clearAndHideRatePlan();
                }
            };
            
            /* Zone Letters */

            $scope.zoneLetters = [];

            $scope.zoneLetterConnector = {
                selectedZoneLetterIndex: 0,
                onZoneLetterSelectionChanged: function () {
                    WhS_Sales_RatePlanAPIService.SaveChanges(buildSaveChangesInput()).then(function (succeeded) {
                        console.log(succeeded);
                        gridAPI.loadGrid(buildGridQuery());
                    });
                }
            };

            /* Grid */

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            /* Action Bar */

            $scope.search = function () {
                return loadRatePlan();
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlan();
                };
                
                WhS_Sales_MainService.sellNewZones(carrierAccountSelectorAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };

            defineSaveButtonMenuActions();
        }

        function load() {

        }

        function loadRatePlan() {
            return loadZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    showRatePlan(true);
                    return gridAPI.loadGrid(buildGridQuery());
                }
            });
        }

        function buildGridQuery() {
            return {
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: getOwnerId(),
                ZoneLetter: $scope.zoneLetters[$scope.zoneLetterConnector.selectedZoneLetterIndex]
            };
        }

        function buildSaveChangesInput() {
            return {
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: getOwnerId(),
                Changes: gridAPI.getChanges()
            };
        }

        function getOwnerId() {
            var ownerId = null;

            if (sellingProductSelectorAPI != undefined)
                ownerId = $scope.sellingProductConnector.selectedSellingProduct.SellingProductId;
            else if (carrierAccountSelectorAPI != undefined)
                ownerId = $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId;

            return ownerId;
        }

        function clearAndHideRatePlan() {
            $scope.zoneLetters.length = 0;
            $scope.zoneLetterConnector.selectedZoneLetterIndex = 0;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showGrid = show;
        }

        function loadZoneLetters() {
            var ownerId = (sellingProductSelectorAPI != undefined) ?
                $scope.sellingProductConnector.selectedSellingProduct.SellingProductId :
                $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId;

            return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, ownerId).then(function (response) {
                if (response == null) return;

                $scope.zoneLetters = [];

                for (var i = 0; i < response.length; i++) {
                    $scope.zoneLetters.push(response[i]);
                }
            });
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [
                { name: "Price List", clicked: savePriceList },
                { name: "Draft", clicked: saveDraft },
            ];
        }

        function savePriceList() {
            console.log("savePriceList");
        }

        function saveDraft() {
            console.log("saveDraft");
        }

        function junkCode() {
            
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
