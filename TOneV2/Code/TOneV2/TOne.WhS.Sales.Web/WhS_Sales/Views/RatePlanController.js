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
        var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var defaultItemDirectiveAPI;
        var defaultItemDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        
        function defineScope() {
            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Sales_SalePriceListOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                clearRatePlan();

                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_SalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct = undefined;
                    }
                }
            };

            $scope.selectedSellingProduct = undefined;
            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                sellingProductSelectorReadyDeferred.resolve();
            };
            $scope.onSellingProductChanged = function () {
                clearRatePlan();
            };

            $scope.selectedCustomer = undefined;
            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onCarrierAccountChanged = function () {
                clearRatePlan();
            };

            $scope.zoneLetters = [];
            $scope.zoneLetterConnector = {
                selectedZoneLetterIndex: 0,
                onZoneLetterSelectionChanged: saveChangesWithLoad
            };

            $scope.onDefaultRoutingProductDirectiveReady = function (api) {
                defaultItemDirectiveAPI = api;
                defaultItemDirectiveReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                //var setLoader = function (value) { $scope.isLoadingSaleZonesSection = value };
                //var gridPayload = buildGridQuery();
                //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridAPI, gridPayload, setLoader, gridReadyDeferred);

               gridReadyDeferred.resolve();
            };

            $scope.search = function () {
                return loadRatePlan();
            };
            $scope.sellNewZones = function () {
                var customerId = $scope.selectedCustomer.CarrierAccountId;
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlan();
                };
                
                WhS_Sales_MainService.sellNewZones(customerId, onCustomerZonesSold);
            };

            defineSaveButtonMenuActions();
        }

        function load() {
            loadAllControls();

            function loadAllControls() {
                $scope.isLoading = true;

                return loadFilterSection().catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });

                function loadFilterSection() {
                    var promises = [];

                    var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(sellingProductSelectorLoadDeferred.promise);

                    sellingProductSelectorReadyDeferred.promise.then(function () {
                        var payload = {
                            filter: null,
                            selectedIds: null
                        };

                        VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, payload, sellingProductSelectorLoadDeferred);
                    });

                    var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(sellingProductSelectorLoadDeferred.promise);

                    carrierAccountSelectorReadyDeferred.promise.then(function () {
                        var payload = {
                            filter: null,
                            selectedIds: null
                        };

                        VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, sellingProductSelectorLoadDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                }
            }
        }

        function loadRatePlan() {
            return UtilsService.waitMultipleAsyncOperations([loadZoneLetters, loadDefaultRoutingProductSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            function loadZoneLetters() {
                return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response != null) {
                        $scope.zoneLetters = [];

                        for (var i = 0; i < response.length; i++) {
                            $scope.zoneLetters.push(response[i]);
                        }

                        showRatePlan(true);
                        return loadGrid();
                    }
                });
            }

            function loadDefaultRoutingProductSection() {
                var defaultItemDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                getDefaultRoutingProduct().then(function (response) {
                    console.log("Default RP");
                    console.log(response);

                    defaultItemDirectiveReadyDeferred.promise.then(function () {
                        var defaultItemDirectivePayload;

                        if (response != null) {
                            defaultItemDirectivePayload = {
                                IsCurrentRoutingProductEditable: response.IsCurrentRoutingProductEditable,
                                CurrentRoutingProductId: response.CurrentRoutingProductId,
                                CurrentRoutingProductName: response.CurrentRoutingProductName,
                                CurrentRoutingProductBED: (response.CurrentRoutingProductBED != null) ? new Date(response.CurrentRoutingProductBED) : null,
                                CurrentRoutingProductEED: (response.CurrentRoutingProductEED != null) ? new Date(response.CurrentRoutingProductEED) : null,
                                NewRoutingProductId: response.NewRoutingProductId,
                                NewRoutingProductBED: (response.NewRoutingProductBED != null) ? new Date(response.NewRoutingProductBED) : null,
                                NewRoutingProductEED: (response.NewRoutingProductEED != null) ? new Date(response.NewRoutingProductEED) : null
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(defaultItemDirectiveAPI, defaultItemDirectivePayload, defaultItemDirectiveLoadDeferred);
                    });
                });

                return defaultItemDirectiveLoadDeferred.promise;

                function getDefaultRoutingProduct() {
                    return WhS_Sales_RatePlanAPIService.GetDefaultItem($scope.selectedOwnerType.value, getOwnerId());
                }
            }

            function loadGrid() {
                var gridLoadDeferred = UtilsService.createPromiseDeferred();

                gridReadyDeferred.promise.then(function () {
                    var gridPayload = buildGridQuery();
                    VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridLoadDeferred);
                });

                return gridLoadDeferred.promise;
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
            console.log(input);

            if (input.NewChanges != null) {
                return WhS_Sales_RatePlanAPIService.SaveChanges(input).then(function (response) {
                    if (loadGrid)
                        gridAPI.load(buildGridQuery());
                });
            }
            else {
                if (loadGrid)
                    return gridAPI.load(buildGridQuery());

                var deferredPromise = UtilsService.createPromiseDeferred();
                deferredPromise.resolve();
                return deferredPromise.promise;
            }

            function buildSaveChangesInput() {
                var newChanges = {};
                var defaultChanges = defaultItemDirectiveAPI.getChanges();
                var zoneChanges = gridAPI.getChanges();

                newChanges = (defaultChanges != null || zoneChanges != null) ? { DefaultChanges: defaultChanges, ZoneChanges: zoneChanges } : null;

                return {
                    OwnerType: $scope.selectedOwnerType.value,
                    OwnerId: getOwnerId(),
                    NewChanges: newChanges
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

            if ($scope.showSellingProductSelector)
                ownerId = $scope.selectedSellingProduct.SellingProductId;
            else if ($scope.showCarrierAccountSelector)
                ownerId = $scope.selectedCustomer.CarrierAccountId;

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
