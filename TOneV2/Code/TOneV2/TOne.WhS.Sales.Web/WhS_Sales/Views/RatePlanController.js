(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_MainService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_Sales_SalePriceListOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRUIUtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_Sales_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
        
        var sellingProductSelectorAPI;
        var sellingProductSelectorReadyPromiseDeferred;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyPromiseDeferred;

        var defaultRoutingProductDirectiveAPI;
        var defaultRoutingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var defaultRoutingProductDirectiveLoadPromiseDeferred;

        var gridAPI;
        var gridReadyPromiseDeferred;

        defineScope();
        
        function defineScope() {
            /* Owner Type */

            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Sales_SalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                clearRatePlan();

                if ($scope.selectedOwnerType != undefined) {
                    sellingProductSelectorAPI = undefined;
                    carrierAccountSelectorAPI = undefined;

                    if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.carrierAccountConnector.selectedCustomer = undefined;

                        sellingProductSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        carrierAccountSelectorReadyPromiseDeferred = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.sellingProductConnector.selectedSellingProduct = undefined;

                        carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        sellingProductSelectorReadyPromiseDeferred = undefined;
                    }

                    $scope.isLoadingOwnerSection = true;

                    loadOwnerSection().catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoadingOwnerSection = false;
                    });
                }
            };

            /* Selling Product */

            $scope.sellingProductConnector = {
                selectedSellingProduct: undefined,

                onSellingProductSelectorReady: function (api) {
                    sellingProductSelectorAPI = api;
                    sellingProductSelectorReadyPromiseDeferred.resolve();
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
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
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
                gridReadyPromiseDeferred.resolve();
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
        }

        function loadOwnerSection() {
            if (sellingProductSelectorReadyPromiseDeferred != undefined)
                return loadSellingProductSection();
            else if (carrierAccountSelectorReadyPromiseDeferred != undefined)
                return loadCarrierAccountSection();
        }

        function loadSellingProductSection() {
            var sellingProductSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingProductSelectorReadyPromiseDeferred.promise.then(function () {
                var sellingProductSelectorPayload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, sellingProductSelectorPayload, sellingProductSelectorLoadPromiseDeferred);
            });

            return sellingProductSelectorLoadPromiseDeferred.promise;
        }

        function loadCarrierAccountSection() {
            var carrierAccountSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {
                var carrierAccountSelectorPayload = {
                    filter: null,
                    selectedIds: null
                };

                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountSelectorPayload, carrierAccountSelectorLoadPromiseDeferred);
            });

            return carrierAccountSelectorLoadPromiseDeferred.promise;
        }

        function loadRatePlan() {
            return loadZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    defaultRoutingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    showRatePlan(true);

                    var promise1 = loadDefaultRoutingProductSection();
                    var promise2 = loadGrid();
                    return UtilsService.waitMultiplePromises([promise1, promise2]).then(function () {
                        defaultRoutingProductDirectiveReadyPromiseDeferred = undefined;
                        defaultRoutingProductDirectiveLoadPromiseDeferred = undefined;
                        gridReadyPromiseDeferred = undefined;
                    });
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

            function loadDefaultRoutingProductSection() {
                defaultRoutingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                getDefaultRoutingProduct().then(function (response) {
                    defaultRoutingProductDirectiveReadyPromiseDeferred.promise.then(function () {
                        var defaultRoutingProductDirectivePayload;

                        if (response != null) {
                            defaultRoutingProductDirectivePayload = {
                                CurrentRoutingProductId: response.RoutingProductId,
                                CurrentBED: response.BED,
                                CurrentEED: response.EED
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(defaultRoutingProductDirectiveAPI, defaultRoutingProductDirectivePayload, defaultRoutingProductDirectiveLoadPromiseDeferred);
                    });
                });

                return defaultRoutingProductDirectiveLoadPromiseDeferred.promise;

                function getDefaultRoutingProduct() {
                    return WhS_Sales_RatePlanAPIService.GetDefaultRoutingProduct($scope.selectedOwnerType.value, getOwnerId());
                }
            }

            function loadGrid() {
                var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                gridReadyPromiseDeferred.promise.then(function () {
                    var gridPayload = buildGridQuery();
                    VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridLoadPromiseDeferred);
                });

                return gridLoadPromiseDeferred.promise;
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
                        gridAPI.load(buildGridQuery());
                });
            }
            else {
                if (loadGrid)
                    gridAPI.load(buildGridQuery());

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

        function clearRatePlan() {
            $scope.zoneLetters.length = 0;
            $scope.zoneLetterConnector.selectedZoneLetterIndex = 0;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showDefaultRoutingProduct = show;
            $scope.showGrid = show;
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
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
