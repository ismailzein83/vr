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

        var defaultItemAPI;
        var defaultItemReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.selectedZoneLetterIndex = 0;

            $scope.onZoneLetterSelectionChanged = function () {
                console.log($scope.selectedZoneLetterIndex);
                return saveChanges(true);
            };

            $scope.onDefaultItemReady = function (api) {
                defaultItemAPI = api;
                defaultItemReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
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
            var promises = [];

            var gridLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(gridLoadDeferred.promise);

            getZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    showRatePlan(true);
                    
                    gridReadyDeferred.promise.then(function () {
                        var gridPayload = getGridQuery();
                        
                        VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridLoadDeferred);
                    });
                }
                else
                    gridLoadDeferred.resolve();
            });

            var defaultItemLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(defaultItemLoadDeferred.promise);

            getDefaultItem().then(function (response) {
                defaultItemReadyDeferred.promise.then(function () {
                    var defaultItemPayload;

                    if (response != null) {
                        defaultItemPayload = {
                            CurrentRoutingProductId: response.CurrentRoutingProductId,
                            CurrentRoutingProductName: response.CurrentRoutingProductName,
                            CurrentRoutingProductBED: (response.CurrentRoutingProductBED != null) ? new Date(response.CurrentRoutingProductBED) : null,
                            CurrentRoutingProductEED: (response.CurrentRoutingProductEED != null) ? new Date(response.CurrentRoutingProductEED) : null,
                            IsCurrentRoutingProductEditable: response.IsCurrentRoutingProductEditable,
                            NewRoutingProductId: response.NewRoutingProductId,
                            NewRoutingProductBED: (response.NewRoutingProductBED != null) ? new Date(response.NewRoutingProductBED) : null,
                            NewRoutingProductEED: (response.NewRoutingProductEED != null) ? new Date(response.NewRoutingProductEED) : null
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(defaultItemAPI, defaultItemPayload, defaultItemLoadDeferred);
                });
            });

            return UtilsService.waitMultiplePromises(promises);

            function getZoneLetters() {
                return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, getOwnerId()).then(function (response) {
                    if (response != null) {
                        $scope.zoneLetters = [];
                        
                        for (var i = 0; i < response.length; i++) {
                            $scope.zoneLetters.push(response[i]);
                        }
                    }
                });
            }

            function getGridQuery() {
                return {
                    OwnerType: $scope.selectedOwnerType.value,
                    OwnerId: getOwnerId(),
                    ZoneLetter: $scope.zoneLetters[$scope.selectedZoneLetterIndex]
                };
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem($scope.selectedOwnerType.value, getOwnerId());
            }
        }

        function saveChanges(refreshRatePlan) {
            var input = getSaveChangesInput();
            console.log(input);

            return WhS_Sales_RatePlanAPIService.SaveChanges(input).then(function (response) {
                if (refreshRatePlan)
                    loadRatePlan();
            });
            
            function getSaveChangesInput() {
                var defaultChanges = getDefaultChanges();
                var zoneChanges = gridAPI.getChanges();

                var newChanges = (defaultChanges != null || zoneChanges != null) ? { DefaultChanges: defaultChanges, ZoneChanges: zoneChanges } : null;

                return {
                    OwnerType: $scope.selectedOwnerType.value,
                    OwnerId: getOwnerId(),
                    NewChanges: newChanges
                };

                function getDefaultChanges() {
                    var defaultChanges = null;
                    var directiveChanges = defaultItemAPI.getChanges();

                    if (directiveChanges != null) {
                        defaultChanges = {
                            NewDefaultRoutingProduct: (directiveChanges.NewRoutingProduct != null) ? {
                                DefaultRoutingProductId: directiveChanges.NewRoutingProduct.RoutingProductId,
                                BED: directiveChanges.NewRoutingProduct.BED,
                                EED: directiveChanges.NewRoutingProduct.EED
                            } : null,
                            DefaultRoutingProductChange: (directiveChanges.RoutingProductChange != null) ? {
                                DefaultRoutingProductId: directiveChanges.RoutingProductChange.RoutingProductId,
                                EED: directiveChanges.RoutingProductChange.EED
                            } : null
                        };
                    }
                    
                    return defaultChanges;
                }
            }
        }

        function clearRatePlan() {
            $scope.zoneLetters= undefined;
            $scope.selectedZoneLetterIndex =undefined;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showDefaultItem = show;
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
                {
                    name: "Price List",
                    clicked: onSavePriceListClick
                },
                {
                    name: "Draft",
                    clicked: function () {
                        return saveChanges(true);
                    }
                }
            ];

            function onSavePriceListClick() {
                var deferred = UtilsService.createPromiseDeferred();

                saveChanges(false).then(function () {
                    savePriceList().then(function (response) {
                        deferred.resolve();
                        loadRatePlan();
                    }).catch(function (error) {
                        handleError(error);
                    });
                }).catch(function (error) {
                    handleError(error);
                });

                return deferred.promise;

                function savePriceList() {
                    return WhS_Sales_RatePlanAPIService.SavePriceList($scope.selectedOwnerType.value, getOwnerId());
                }

                function handleError(error) {
                    deferred.reject();
                    VRNotificationService.notifyException(error, $scope);
                }
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
