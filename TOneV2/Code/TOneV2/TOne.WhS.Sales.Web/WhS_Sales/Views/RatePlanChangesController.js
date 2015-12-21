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
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            if (response.Data[i].IsCurrentRateInherited)
                                response.Data[i].CurrentRate += " (Inherited)";

                            var changeType = UtilsService.getEnum(WhS_Sales_RateChangeTypeEnum, "value", response.Data[i].ChangeType);
                            response.Data[i].ChangeType = changeType ? changeType.description : null;
                        }
                        onResponseReady(response);
                    }
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
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            if (response.Data[i].IsCurrentRoutingProductInherited)
                                response.Data[i].CurrentRoutingProductName += " (Inherited)";
                        }
                        onResponseReady(response);
                    }
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

            var getChangesSummaryPromise = getChangesSummary();
            promises.push(getChangesSummaryPromise);
            
            var getDefaultItemPromise = getDefaultItem();
            promises.push(getDefaultItemPromise);

            UtilsService.waitMultiplePromises(promises).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function getChangesSummary() {
                return WhS_Sales_RatePlanAPIService.GetChangesSummary(ownerType, ownerId).then(function (response) {
                    if (response) {
                        $scope.totalNewRates = response.TotalNewRates;
                        $scope.totalIncreasedRates = response.TotalRateIncreases;
                        $scope.totalDecreasedRates = response.TotalRateDecreases;
                        $scope.totalRateChanges = response.TotalRateChanges;
                        $scope.totalNewZoneRoutingProducts = response.TotalNewZoneRoutingProducts;
                        $scope.totalZoneRoutingProductChanges = response.TotalZoneRoutingProductChanges;
                    }
                });
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerType, ownerId).then(function (response) {
                    console.log(response);
                    if (response && (response.NewRoutingProductId || (response.CurrentRoutingProductId && response.RoutingProductChangeEED))) {
                        $scope.defaultItem = {};
                        $scope.defaultItem.currentRoutingProductName = response.CurrentRoutingProductName ? response.CurrentRoutingProductName : "None";
                        $scope.defaultItem.currentRoutingProductName = response.IsCurrentRoutingProductEditable === false ? $scope.defaultItem.currentRoutingProductName += " (Inherited)" : $scope.defaultItem.currentRoutingProductName;
                        $scope.defaultItem.newRoutingProductName = response.NewRoutingProductName;
                        $scope.defaultItem.changedToRoutingProductName = !response.NewRoutingProductName ? "(Default)" : null;
                        $scope.defaultItem.effectiveOn = new Date().toDateString();
                    }
                    console.log($scope.defaultItem);
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
