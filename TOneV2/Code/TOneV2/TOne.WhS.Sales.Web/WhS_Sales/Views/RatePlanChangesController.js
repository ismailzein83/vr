(function (appControllers) {

    "use strict";

    RatePlanChangesController.$inject = ["$scope", "WhS_Sales_RatePlanAPIService", "WhS_Sales_RateChangeTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RatePlanChangesController($scope, WhS_Sales_RatePlanAPIService, WhS_Sales_RateChangeTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

        var ownerType;
        var ownerId;
        var changes;

        var rateGridAPI;
        var rateGridReadyDeferred = UtilsService.createPromiseDeferred();
        var routingProductGridAPI;
        var routingProductGridReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onRateGridReady = function (api) {
                rateGridAPI = api;
                rateGridReadyDeferred.resolve();
                rateGridAPI.retrieveData({ OwnerType: ownerType, OwnerId: ownerId });
            };
            $scope.rateDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanAPIService.GetFilteredZoneRateChanges(dataRetrievalInput).then(function (response) {
                    if (response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            if (response.Data[i].IsCurrentRateInherited)
                                response.Data[i].CurrentRate += " (Inherited)";

                            var changeType = UtilsService.getEnum(WhS_Sales_RateChangeTypeEnum, "value", response.Data[i].ChangeType);
                            response.Data[i].ChangeType = changeType ? changeType.description : null;
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.onRoutingProductGridReady = function (api) {
                routingProductGridAPI = api;
                routingProductGridReadyDeferred.resolve();
                routingProductGridAPI.retrieveData({ OwnerType: ownerType, OwnerId: ownerId });
            };
            $scope.routingProductDataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanAPIService.GetFilteredZoneRoutingProductChanges(dataRetrievalInput).then(function (response) {
                    if (response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            if (response.Data[i].IsCurrentRoutingProductInherited)
                                response.Data[i].CurrentRoutingProductName += " (Inherited)";
                            else if (response.Data[i].IsNewRoutingProductInherited)
                                response.Data[i].NewRoutingProductName += " (Inherited)";
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

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

            promises.push(rateGridReadyDeferred.promise);
            promises.push(routingProductGridReadyDeferred.promise);

            var getChangesPromise = getChanges();
            promises.push(getChangesPromise);
            
            var getDefaultItemDeferred = UtilsService.createPromiseDeferred();
            promises.push(getDefaultItemDeferred.promise);

            var zoneItemsGetDeferred = UtilsService.createPromiseDeferred();
            promises.push(zoneItemsGetDeferred.promise);

            getChangesPromise.then(function () {
                if (changes && changes.DefaultChanges && (changes.DefaultChanges.Entity.NewDefaultRoutingProduct || changes.DefaultChanges.Entity.DefaultRoutingProductChange)) {
                    getDefaultItem().then(function () {
                        getDefaultItemDeferred.resolve();
                    }).catch(function (error) { getDefaultItemDeferred.reject(); });
                }
                else
                    getDefaultItemDeferred.resolve();

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

                            if (zoneItemChanges) {
                                // Update rate summary totals
                                if (zoneItemChanges.Entity.NewRate || zoneItemChanges.Entity.RateChange) {
                                    var currentRate = zoneItem.CurrentRate;
                                    var newRate = zoneItemChanges.Entity.NewRate ? zoneItemChanges.Entity.NewRate.NormalRate : null;

                                    if (currentRate && newRate) {
                                        if (Number(newRate) > Number(currentRate))
                                            $scope.totalIncreasedRates++;
                                        else if (Number(newRate) < Number(currentRate))
                                            $scope.totalDecreasedRates++;
                                        else
                                            console.log("Same");
                                    }
                                    else if (newRate) {
                                        $scope.totalNewRates++;
                                    }
                                    else if (currentRate) { // && effectiveOn
                                        $scope.totalRateChanges++;
                                    }
                                }

                                // Update routing product summary totals
                                if (zoneItemChanges.Entity.NewRoutingProduct || zoneItemChanges.Entity.RoutingProductChange) {
                                    var currentRoutingProductId = zoneItem.CurrentRoutingProductId;
                                    var newRoutingProductId = zoneItemChanges.Entity.NewRoutingProduct ? zoneItemChanges.Entity.NewRoutingProduct.ZoneRoutingProductId : null;

                                    if (newRoutingProductId)
                                        $scope.totalNewZoneRoutingProducts++;
                                    else if (currentRoutingProductId) // && effectiveOn
                                        $scope.totalZoneRoutingProductChanges++;
                                }
                            }
                        }
                    }
                });
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
