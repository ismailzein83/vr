(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_MainService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_Sales_RatePlanOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRUIUtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
        
        var sellingProductSelectorAPI;
        var carrierAccountSelectorAPI;

        var defaultRoutingProductDirectiveAPI;
        var defaultRoutingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                clearRatePlan();

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
                    clearRatePlan();
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
                    clearRatePlan();
                }
            };
            
            /* Zone Letters */

            $scope.zoneLetters = [];

            $scope.zoneLetterConnector = {
                selectedZoneLetterIndex: 0,
                onZoneLetterSelectionChanged: saveChangesWithLoad
            };

            /* Default Routing Product */

            $scope.onDefaultRoutingProductDirectiveReady = function (api) {
                defaultRoutingProductDirectiveAPI = api;
                defaultRoutingProductDirectiveReadyPromiseDeferred.resolve();
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
                var customerId = $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId;
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlan();
                };
                
                WhS_Sales_MainService.sellNewZones(customerId, onCustomerZonesSold);
            };

            defineSaveButtonMenuActions();
        }

        function load() {
            $scope.isLoadingDefaultRoutingProductSection = true;

            loadDefaultRoutingProductSection().catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingDefaultRoutingProductSection = false;
            });
        }

        function loadDefaultRoutingProductSection() {
            var loadPromises = [];

            var defaultRoutingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            loadPromises.push(defaultRoutingProductDirectiveLoadPromiseDeferred.promise);

            defaultRoutingProductDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    CurrentRoutingProductId: 130248,
                    CurrentBED: new Date(),
                    CurrentEED: new Date()
                };

                VRUIUtilsService.callDirectiveLoad(defaultRoutingProductDirectiveAPI, payload, defaultRoutingProductDirectiveLoadPromiseDeferred);
                defaultRoutingProductDirectiveAPI.load(payload);
            });

            return UtilsService.waitMultiplePromises(loadPromises);
        }

        function loadRatePlan() {
            return loadZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    showRatePlan(true);
                    return gridAPI.loadGrid(buildGridQuery());
                }
            });

            function loadZoneLetters() {
                return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response == null)
                        return;

                    $scope.zoneLetters = [];

                    for (var i = 0; i < response.length; i++) {
                        $scope.zoneLetters.push(response[i]);
                    }
                });
            }
        }

        function saveChangesWithLoad() {
            return saveChanges(true);
        }

        function saveChangesWithoutLoad() {
            return saveChanges(false);
        }

        function saveChanges(loadGrid) {
            var input = buildSaveChangesInput();
            console.log(input.NewChanges);
            
            if (input.NewChanges != null) {
                return WhS_Sales_RatePlanAPIService.SaveChanges(input).then(function (response) {
                    if (loadGrid)
                        gridAPI.loadGrid(buildGridQuery());
                });
            }
            else {
                if (loadGrid)
                    gridAPI.loadGrid(buildGridQuery());

                var deferredPromise = UtilsService.createPromiseDeferred();
                deferredPromise.resolve();
                return deferredPromise.promise;
            }

            function buildSaveChangesInput() {
                return {
                    OwnerType: $scope.selectedOwnerType.value,
                    OwnerId: getOwnerId(),
                    NewChanges: gridAPI.getChanges()
                };
            }
        }

        function buildGridQuery() {
            return {
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: getOwnerId(),
                ZoneLetter: $scope.zoneLetters[$scope.zoneLetterConnector.selectedZoneLetterIndex]
            };
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showGrid = show;
        }

        function clearRatePlan() {
            $scope.zoneLetters.length = 0;
            $scope.zoneLetterConnector.selectedZoneLetterIndex = 0;
            showRatePlan(false);
        }

        function onSavePriceListClicked() {
            return saveChangesWithoutLoad().then(function () {
                return savePriceList().then(function (response) {
                    loadRatePlan();
                });
            });

            function savePriceList() {
                return WhS_Sales_RatePlanAPIService.SavePriceList($scope.selectedOwnerType.value, getOwnerId());
            }
        }

        function getOwnerId() {
            var ownerId = null;

            if (sellingProductSelectorAPI != undefined)
                ownerId = $scope.sellingProductConnector.selectedSellingProduct.SellingProductId;
            else if (carrierAccountSelectorAPI != undefined)
                ownerId = $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId;

            return ownerId;
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [
                { name: "Price List", clicked: onSavePriceListClicked },
                { name: "Draft", clicked: saveChangesWithoutLoad },
            ];
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
