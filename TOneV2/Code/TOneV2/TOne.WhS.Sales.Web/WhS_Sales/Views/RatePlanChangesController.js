(function (appControllers) {

    "use strict";

    RatePlanChangesController.$inject = ["$scope", "WhS_Sales_RatePlanAPIService", "WhS_Sales_RateChangeTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RatePlanChangesController($scope, WhS_Sales_RatePlanAPIService, WhS_Sales_RateChangeTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

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
            $scope.zoneRateChanges = [];
            $scope.zoneRoutingProductChanges = [];

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
                    console.log(changes);
                });
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerType, ownerId).then(function (response) {
                    $scope.defaultItem = {};
                    
                    $scope.defaultItem.currentRoutingProductName = response.CurrentRoutingProductName;
                    $scope.defaultItem.newRoutingProductName = changes.DefaultChanges.Entity.NewDefaultRoutingProduct ? changes.DefaultChanges.DefaultRoutingProductName : null;
                    $scope.defaultItem.effectiveOn = new Date().toDateString();
                });
            }

            function getZoneItems() {
                var entities = UtilsService.getPropValuesFromArray(changes.ZoneChanges, "Entity");
                var zoneIds = entities ? UtilsService.getPropValuesFromArray(entities, "ZoneId") : [];

                return WhS_Sales_RatePlanAPIService.GetZoneItems({
                    Filter: {
                        OwnerType: ownerType,
                        OwnerId: ownerId,
                        ZoneIds: zoneIds
                    }
                }).then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            var zoneItem = response[i];
                            var zoneItemChanges = UtilsService.getItemByVal(changes.ZoneChanges, zoneItem.ZoneName, "ZoneName"); // It should be by Entity.ZoneId, but nested properties aren't supported by UtilsService.getItemByVal
                            addRateChange(zoneItem, zoneItemChanges);
                            addRoutingProductChange(zoneItem, zoneItemChanges);
                        }
                    }
                });

                function addRateChange(zoneItem, zoneItemChanges) {
                    if (zoneItemChanges && (zoneItemChanges.Entity.NewRate || zoneItemChanges.Entity.RateChange)) {
                        var newRate = zoneItemChanges.Entity.NewRate ? zoneItemChanges.Entity.NewRate.NormalRate : null;

                        var dataItem = {
                            zoneId: zoneItem.ZoneId,
                            zoneName: zoneItem.ZoneName,
                            currentRate: zoneItem.CurrentRate,
                            newRate: newRate
                        };

                        setRateChangeType(dataItem);
                        setEffectiveDates(dataItem, zoneItemChanges.Entity.NewRate, zoneItemChanges.Entity.RateChange);

                        $scope.zoneRateChanges.push(dataItem);
                        updateRateSummary(dataItem);
                    }

                    function setRateChangeType(dataItem) {
                        var currentRate = dataItem.currentRate;
                        var newRate = dataItem.newRate;

                        if (currentRate && newRate) {
                            if (Number(newRate) > Number(currentRate))
                                dataItem.changeType = WhS_Sales_RateChangeTypeEnum.Increase.description;
                            else if (Number(newRate) < Number(currentRate))
                                dataItem.changeType = WhS_Sales_RateChangeTypeEnum.Decrease.description;
                        }
                        else if (newRate)
                            dataItem.changeType = WhS_Sales_RateChangeTypeEnum.New.description;
                        else
                            dataItem.changeType = WhS_Sales_RateChangeTypeEnum.Close.description;
                    }
                    function setEffectiveDates(dataItem, newRateEntity, rateChangeEntity) {
                        dataItem.effectiveOn = rateChangeEntity ? rateChangeEntity.EED : newRateEntity.BED;
                        dataItem.effectiveUntil = newRateEntity ? newRateEntity.EED : null;
                    }
                    function updateRateSummary(dataItem) {
                        if (!dataItem)
                            return;

                        if (dataItem.currentRate && dataItem.newRate) {
                            if (Number(dataItem.newRate) > Number(dataItem.currentRate))
                                $scope.totalIncreasedRates++;
                            else if (Number(dataItem.newRate) < Number(dataItem.currentRate))
                                $scope.totalDecreasedRates++;
                        }
                        else if (dataItem.newRate)
                            $scope.totalNewRates++;
                        else if (dataItem.currentRate && dataItem.effectiveOn)
                            $scope.totalRateChanges++;
                    }
                }
                function addRoutingProductChange(zoneItem, zoneItemChanges) {
                    if (zoneItemChanges && (zoneItemChanges.Entity.NewRoutingProduct || zoneItemChanges.Entity.RoutingProductChange)) {
                        var dataItem = {
                            zoneId: zoneItem.ZoneId,
                            zoneName: zoneItem.ZoneName,
                            currentRoutingProduct: zoneItem.CurrentRoutingProductName,
                            newRoutingProduct: zoneItemChanges.Entity.NewRoutingProduct ?
                                zoneItemChanges.ZoneRoutingProductName : null,
                            effectiveOn: new Date()
                        };

                        $scope.zoneRoutingProductChanges.push(dataItem);
                        updateRoutingProductSummary(dataItem);
                    }

                    function updateRoutingProductSummary(dataItem) {
                        if (dataItem.newRoutingProduct)
                            $scope.totalNewZoneRoutingProducts++;
                        else if (dataItem.currentRoutingProduct && dataItem.effectiveOn)
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
