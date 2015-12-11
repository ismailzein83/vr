(function (appControllers) {

    "use strict";

    RatePlanChangesController.$inject = ["$scope", "WhS_Sales_RatePlanAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RatePlanChangesController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var ownerType;
        var ownerId;
        var changes;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                ownerType = parameters.OwnerType;
                ownerId = parameters.OwnerId;
            }
        }

        function defineScope() {
            $scope.totalNewRates = 0;
            $scope.totalIncreasedRates = 0;
            $scope.totalDecreasedRates = 0;
            $scope.totalRateChanges = 0;
            $scope.totalNewZoneRoutingProducts = 0;
            $scope.totalZoneRoutingProductChanges = 0;

            $scope.defaultItem;
            $scope.zoneChanges = [];

            $scope.save = function () {
                closeModal(true);
            };

            $scope.close = function () {
                closeModal(false);
            };
        }

        function load() {
            $scope.isLoading = true;
            var promises = [];

            var getChangesPromise = getChanges();
            promises.push(getChangesPromise);
            
            var defaultItemGetDeferred = UtilsService.createPromiseDeferred();
            promises.push(defaultItemGetDeferred.promise);

            var zoneItemsGetDeferred = UtilsService.createPromiseDeferred();
            promises.push(zoneItemsGetDeferred.promise);

            getChangesPromise.then(function () {
                if (changes && changes.DefaultChanges && (changes.DefaultChanges.Entity.NewDefaultRoutingProduct || changes.DefaultChanges.Entity.DefaultRoutingProductChange)) {
                    getDefaultItem().then(function () {
                        defaultItemGetDeferred.resolve();
                    }).catch(function (error) { defaultItemGetDeferred.reject(); });
                }
                else
                    defaultItemGetDeferred.resolve();

                if (changes && changes.ZoneChanges) {
                    getZoneItems().then(function () {
                        zoneItemsGetDeferred.resolve();
                    }).catch(function (error) { zoneItemsGetDeferred.reject(); });
                }
                else
                    zoneItemsGetDeferred.resolve();
            });

            UtilsService.waitMultiplePromises(promises).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function getChanges() {
                return WhS_Sales_RatePlanAPIService.GetChanges(ownerType, ownerId).then(function (response) {
                    changes = response;
                });
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerType, ownerId).then(function (response) {
                    $scope.defaultItem = {};

                    // Set current properties
                    $scope.defaultItem.currentRoutingProductName = response.CurrentRoutingProductName;
                    if (response.CurrentRoutingProductEED) {
                        var currentEED = new Date(response.CurrentRoutingProductEED);
                        $scope.defaultItem.currentRoutingProductEED = currentEED.toDateString();
                    }
                    else $scope.defaultItem.currentRoutingProductEED = "NULL";

                    // Set new properties
                    var newDefaultRoutingProduct = changes.DefaultChanges.Entity.NewDefaultRoutingProduct;
                    var defaultRoutingProductChange = changes.DefaultChanges.Entity.DefaulRoutingProductChange;

                    $scope.defaultItem.newRoutingProductName = changes.DefaultChanges.DefaultRoutingProductName;
                    $scope.defaultItem.newRoutingProductEED = newDefaultRoutingProduct ? newDefaultRoutingProduct.EED : defaultRoutingProductChange.EED;

                    if ($scope.defaultItem.newRoutingProductEED) {
                        var newEED = new Date($scope.defaultItem.newRoutingProductEED);
                        $scope.defaultItem.newRoutingProductEED = newEED.toDateString();
                    }
                    else $scope.defaultItem.newRoutingProductEED = "NULL";
                });
            }

            function getZoneItems() {
                var entities = UtilsService.getPropValuesFromArray(changes.ZoneChanges, "Entity");
                var zoneIds = UtilsService.getPropValuesFromArray(entities, "ZoneId");

                return WhS_Sales_RatePlanAPIService.GetZoneItems({
                    Filter: {
                        OwnerType: ownerType,
                        OwnerId: ownerId,
                        ZoneIds: zoneIds
                    }
                }).then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            var gridItem = {};

                            // Set current properties
                            gridItem.zoneId = response[i].ZoneId;
                            gridItem.zoneName = response[i].ZoneName;
                            gridItem.currentRate = response[i].CurrentRate;
                            gridItem.currentRateEED = response[i].CurrentRateEED;
                            gridItem.currentRoutingProductName = response[i].CurrentRoutingProductName;
                            gridItem.currentRoutingProductEED = response[i].CurrentRoutingProductEED;

                            // Set new properties
                            var itmChanges = UtilsService.getItemByVal(changes.ZoneChanges, gridItem.zoneName, "ZoneName"); // It should be by Entity.ZoneId, but nested properties aren't supported by UtilsService.getItemByVal
                            if (itmChanges) {
                                if (itmChanges.Entity.NewRate) {
                                    gridItem.newRate = itmChanges.Entity.NewRate.NormalRate;
                                    gridItem.newRateEED = itmChanges.Entity.NewRate.EED;
                                }
                                else if (itmChanges.Entity.RateChange)
                                    gridItem.newRateEED = itmChanges.Entity.RateChange.EED;

                                if (itmChanges.Entity.NewRoutingProduct) {
                                    gridItem.newRoutingProductName = itmChanges.ZoneRoutingProductName;
                                    gridItem.newRoutingProductEED = itmChanges.Entity.NewRoutingProduct.EED;
                                }
                                else if (itmChanges.Entity.RoutingProductChange)
                                    gridItem.newRoutingProductEED = itmChanges.Entity.RoutingProductChange.EED;
                            }

                            updateSummary(gridItem);
                            $scope.zoneChanges.push(gridItem);
                        }
                    }
                });

                function updateSummary(gridItem) {
                    if (gridItem) {
                        // Update rate properties
                        if (gridItem.currentRate && gridItem.newRate) {
                            var currentRate = Number(gridItem.currentRate);
                            var newRate = Number(gridItem.newRate);

                            if (newRate > currentRate) $scope.totalIncreasedRates++;
                            else if (newRate < currentRate) $scope.totalDecreasedRates++;
                        }
                        else if (gridItem.newRate)
                            $scope.totalNewRates++;
                        else if (gridItem.currentRate && gridItem.newRateEED)
                            $scope.totalRateChanges++;

                        // Update routing product properties
                        if (gridItem.newRoutingProductName)
                            $scope.totalNewZoneRoutingProducts++;
                        else if (gridItem.currentRoutingProductName && gridItem.newRoutingProductEED)
                            $scope.totalZoneRoutingProductChanges++;
                    }
                }
            }
        }

        function closeModal(save) {
            if ($scope.onRatePlanChangesClose)
                $scope.onRatePlanChangesClose(save);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller("WhS_Sales_RatePlanChangesController", RatePlanChangesController);

})(appControllers);
