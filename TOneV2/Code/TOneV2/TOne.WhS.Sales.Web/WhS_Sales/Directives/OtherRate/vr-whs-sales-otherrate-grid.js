﻿"use strict";

app.directive("vrWhsSalesOtherrateGrid", ["UtilsService", "VRNotificationService", 'WhS_Sales_RatePlanUtilsService', 'WhS_Sales_RatePlanService', function (UtilsService, VRNotificationService, WhS_Sales_RatePlanUtilsService, WhS_Sales_RatePlanService) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var otherRateGrid = new OtherRateGrid($scope, ctrl, $attrs);
            otherRateGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/OtherRate/Templates/OtherRateGridTemplate.html"
    };

    function OtherRateGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var zoneItem;
        var settings;
        var ownerCurrencyId;

        function initializeController() {

            $scope.otherRates = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.areOtherRatesEditable = function () {
                if (zoneItem.CurrentRate != null)
                    return true;
                if (zoneItem.NewRate != null && WhS_Sales_RatePlanUtilsService.validateNewRate(zoneItem, ownerCurrencyId) == null)
                    return true;
                return false;
            };

            $scope.onCurrentRateClicked = function (dataItem) {
                WhS_Sales_RatePlanService.viewFutureRate(zoneItem.ZoneName, dataItem.FutureRate);
            };

            $scope.onCurrentRateNewEEDChanged = function () {
                zoneItem.IsDirty = true;
            };

            $scope.onNewRateChanged = function (dataItem) {
                zoneItem.IsDirty = true;
                WhS_Sales_RatePlanUtilsService.onNewRateChanged(dataItem);
            };

            $scope.onNewRateBlurred = function (dataItem) {
                zoneItem.IsDirty = true;
                WhS_Sales_RatePlanUtilsService.onNewRateBlurred(dataItem, settings);
            };

            $scope.validateNewRate = function (otherRate) {
                return WhS_Sales_RatePlanUtilsService.validateNewRate(otherRate, ownerCurrencyId);
            };

            $scope.validateNewRateDates = function (otherRate) {
                return WhS_Sales_RatePlanUtilsService.validateNewRateDates(otherRate);
            };
        }
        function defineAPI() {

            var api = {};

            api.loadGrid = function (query)
            {
            	zoneItem = query.zoneItem;
            	settings = query.settings;
            	ownerCurrencyId = query.ownerCurrencyId;

            	$scope.isCountryEnded = query.zoneItem.IsCountryEnded;
            	$scope.isZonePendingClosed = query.zoneItem.IsZonePendingClosed;
            	$scope.isCountryNew = query.zoneItem.IsCountryNew;

            	$scope.isSellingProductZone = zoneItem.isSellingProductZone;

            	if (zoneItem.NewRates == null)
            		zoneItem.NewRates = [];
            	if (zoneItem.ClosedRates == null)
            		zoneItem.ClosedRates = [];

            	return loadGrid();
            };
            
            function loadGrid()
            {
            	if (zoneItem.RateTypes == null)
            		return;

            	for (var i = 0; i < zoneItem.RateTypes.length; i++)
            	{
            		var otherRate = {};



            		otherRate.RateTypeId = zoneItem.RateTypes[i].RateTypeId;
            		otherRate.Name = zoneItem.RateTypes[i].Name;

            		otherRate.ZoneBED = zoneItem.ZoneBED;
            		otherRate.ZoneEED = zoneItem.ZoneEED;

            		otherRate.CountryBED = zoneItem.CountryBED;
            		otherRate.IsCountryNew = zoneItem.IsCountryNew;

            		if (zoneItem.CurrentOtherRates != null) {
            			var currentOtherRate = zoneItem.CurrentOtherRates[otherRate.RateTypeId];
            			if (currentOtherRate != undefined) {
            			    otherRate.CurrentRate = currentOtherRate.Rate;
            			    otherRate.CurrentRateCurrencyId = currentOtherRate.CurrencyId;
            				otherRate.IsCurrentRateEditable = currentOtherRate.IsRateEditable;
            				otherRate.CurrentRateBED = currentOtherRate.BED;
            				otherRate.CurrentRateEED = currentOtherRate.EED;
            				otherRate.CurrentRateNewEED = currentOtherRate.EED;
            			}
            		}

            		var newOtherRate = UtilsService.getItemByVal(zoneItem.NewRates, otherRate.RateTypeId, 'RateTypeId');
            		if (newOtherRate != null) {
            			zoneItem.IsDirty = true;
            			otherRate.NewRate = newOtherRate.Rate;
            			otherRate.NewRateBED = newOtherRate.BED;
            			otherRate.NewRateEED = newOtherRate.EED;
            		}
            		else {
            			var closedOtherRate = UtilsService.getItemByVal(zoneItem.ClosedRates, otherRate.RateTypeId, 'RateTypeId');
            			if (closedOtherRate != null) {
            				zoneItem.IsDirty = true;
            				otherRate.CurrentRateNewEED = closedOtherRate.EED;
            			}
            		}

            		if (zoneItem.FutureOtherRates != null) {
            			otherRate.FutureRate = zoneItem.FutureOtherRates[otherRate.RateTypeId];
            		}

            		WhS_Sales_RatePlanUtilsService.onNewRateChanged(otherRate);
            		$scope.otherRates.push(otherRate);
            	}
            }

            api.applyChanges = function ()
            {
                if (!$scope.areOtherRatesEditable())
                    return;

                for (var i = 0; i < $scope.otherRates.length; i++)
                {
                    var otherRate = $scope.otherRates[i];
                    clearOtherRateDrafts(otherRate.RateTypeId);

                    if (otherRate.NewRate != null)
                    {
                        zoneItem.NewRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.RateTypeId,
                            Rate: otherRate.NewRate,
                            BED: otherRate.NewRateBED,
                            EED: otherRate.NewRateEED
                        });
                    }
                    else if (!WhS_Sales_RatePlanService.areDatesTheSame(otherRate.CurrentRateEED, otherRate.CurrentRateNewEED))
                    {
                        zoneItem.ClosedRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.RateTypeId,
                            EED: otherRate.CurrentRateNewEED
                        });
                    }
                }

                if (zoneItem.NewRates.length == 0)
                	zoneItem.NewRates = null;
                if (zoneItem.ClosedRates.length == 0)
                	zoneItem.ClosedRates = null;
            };

            function clearOtherRateDrafts(rateTypeId) {
            	if (!clearNewOtherRate(rateTypeId))
            		clearClosedOtherRate(rateTypeId);
            }
            function clearNewOtherRate(rateTypeId) {
            	var newOtherRateIndex = UtilsService.getItemIndexByVal(zoneItem.NewRates, rateTypeId, 'RateTypeId');
            	if (newOtherRateIndex != -1) {
            		zoneItem.NewRates.splice(newOtherRateIndex, 1);
            		return true;
            	}
            	return false;
            }
            function clearClosedOtherRate(rateTypeId) {
            	var closedOtherRateIndex = UtilsService.getItemIndexByVal(zoneItem.ClosedRates, rateTypeId, 'RateTypeId');
            	if (closedOtherRateIndex != -1) {
            		zoneItem.ClosedRates.splice(closedOtherRateIndex, 1);
            		return true;
            	}
                return false;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);