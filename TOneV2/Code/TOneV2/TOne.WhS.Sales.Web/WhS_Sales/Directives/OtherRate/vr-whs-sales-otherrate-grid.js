"use strict";

app.directive("vrWhsSalesOtherrateGrid", ["UtilsService", "VRNotificationService", 'WhS_Sales_RatePlanUtilsService', 'WhS_Sales_RatePlanService', 'WhS_Sales_RateSourceEnum', 'VRDateTimeService', 'WhS_Sales_SalePriceListOwnerTypeEnum', function (UtilsService, VRNotificationService, WhS_Sales_RatePlanUtilsService, WhS_Sales_RatePlanService, WhS_Sales_RateSourceEnum, VRDateTimeService, WhS_Sales_SalePriceListOwnerTypeEnum) {
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
        var saleAreaSetting;
        var ownerType;
        var context;
        var normalRateEED;
        var increasedRateDayOffset;
        var isCurrentRateEditable;

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
       /*    $scope.validateNewRateDates = function (dataItem) {
                if (dataItem.CurrentRateNewEED != undefined) {
                    for (var j = 0; j < $scope.otherRates.length; j++) {
                        var otherRateChild = $scope.otherRates[j];

                        if (otherRateChild.RateTypeId != dataItem.RateTypeId && otherRateChild.CurrentRateNewEED != undefined &&  !UtilsService.areDateTimesEqual(dataItem.CurrentRateNewEED , otherRateChild.CurrentRateNewEED))
                            return "All other rates EED's must have same value";
                    }
                }
                return null;
            };*/

            $scope.onNewRateChanged = function (dataItem) {
                zoneItem.IsDirty = true;
                dataItem.IsEEDDisabled = isEEDDisabled(dataItem);
                WhS_Sales_RatePlanUtilsService.onNewRateChanged(dataItem);
                $scope.isNewRateBEDRequired = isNewRateBEDRequired($scope.NewRateBED, dataItem.NewRate);
            };

            $scope.onNewRateBlurred = function (dataItem) {
                zoneItem.IsDirty = true;
                if ($scope.NewRateBED == undefined && dataItem.NewRate > 0 && zoneItem.CurrentRateEED == undefined) {
                    var today = VRDateTimeService.getNowDateTime();
                    var newDateValue = new Date(today.getFullYear(), today.getMonth(), today.getDate() + increasedRateDayOffset);
                    $scope.NewRateBED = newDateValue;
                };
                if (zoneItem.CurrentRateEED != undefined && dataItem.NewRate > 0)
                {
                    $scope.NewRateBED = zoneItem.CurrentRateEED;
                }
                WhS_Sales_RatePlanUtilsService.onNewRateBlurred(dataItem, settings);
            };
            $scope.validateNewRate = function (otherRate) {
                    return WhS_Sales_RatePlanUtilsService.validateNewRate(otherRate, ownerCurrencyId);
            };

            $scope.validateNewRateBED = function () {
                var currentEED = UtilsService.createDateFromString(zoneItem.CurrentRateEED);
                if ($scope.NewRateBED != undefined) {
                    if (zoneItem.CurrentRateEED != undefined && $scope.NewRateBED < currentEED.getTime()) {
                        return "BED of the other rate must be greater than or equal to " + zoneItem.CurrentRateEED + " BED of the  last normal rate of the zone";
                    }
                }
                else
                    return null;
            };
        }
        function defineAPI() {

            var api = {};

            api.loadGrid = function (query) {
                if (query.settings != undefined)
                    increasedRateDayOffset = query.settings.increasedRateDayOffset;
                zoneItem = query.zoneItem;
                isCurrentRateEditable = zoneItem.IsCurrentRateEditable;
                context = query.context;
                settings = query.settings;
                ownerCurrencyId = query.ownerCurrencyId;
                saleAreaSetting = query.saleAreaSetting;
                ownerType = query.ownerType;
                if (context != undefined && context.getZoneBED() != undefined) {
                    $scope.NewRateBED = query.context.getZoneBED();
                    $scope.isNewRateBEDDisabled = $scope.NewRateBED != undefined || $scope.isCountryEnded;
                }
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

            function loadGrid() {
                $scope.isNewRateBEDDisabled = $scope.isCountryEnded || $scope.isZonePendingClosed || $scope.NewRateBED != undefined;
                if (zoneItem.RateTypes == null)
                    return;
                if ($scope.NewRateBED == undefined)
                    $scope.NewRateBED = zoneItem.NewOtherRateBED;
                for (var i = 0; i < zoneItem.RateTypes.length; i++) {
                    var otherRate = {};
                    var currentOtherRate;
                    otherRate.RateTypeId = zoneItem.RateTypes[i].RateTypeId;
                    if (zoneItem.CurrentOtherRates != null)
                        currentOtherRate = zoneItem.CurrentOtherRates[otherRate.RateTypeId];
                    otherRate.Name = zoneItem.RateTypes[i].Name;
                    otherRate.ZoneBED = zoneItem.ZoneBED;
                    otherRate.ZoneEED = zoneItem.ZoneEED;
                    otherRate.CountryBED = zoneItem.CountryBED;
                    otherRate.IsCountryNew = zoneItem.IsCountryNew;

                    if (currentOtherRate != undefined) {
                        otherRate.CurrentRate = currentOtherRate.Rate;
                        otherRate.CurrentRateCurrencyId = currentOtherRate.CurrencyId;
                        otherRate.IsCurrentRateEditable = currentOtherRate.IsRateEditable;
                        otherRate.CurrentRateBED = currentOtherRate.BED;
                        otherRate.CurrentRateEED = currentOtherRate.EED;
                        otherRate.CurrentRateNewEED = currentOtherRate.EED;
                        if (normalRateEED != undefined && otherRate.CurrentRateNewEED == undefined)
                            otherRate.CurrentRateNewEED = normalRateEED;
                        WhS_Sales_RatePlanUtilsService.setNormalRateIconProperties(otherRate, ownerType, saleAreaSetting);
                    }
                  
                    var newOtherRate = UtilsService.getItemByVal(zoneItem.NewRates, otherRate.RateTypeId, 'RateTypeId');
                    if (newOtherRate != null) {
                        zoneItem.IsDirty = true;
                        otherRate.NewRate = newOtherRate.Rate;
                        otherRate.NewRateBED = newOtherRate.BED;
                        otherRate.NewRateEED = newOtherRate.EED;
                        $scope.isNewRateBEDRequired = isNewRateBEDRequired($scope.NewRateBED, otherRate.NewRate);
                    }
                   
                    else {
                        var closedOtherRate = UtilsService.getItemByVal(zoneItem.ClosedRates, otherRate.RateTypeId, 'RateTypeId');
                        if (closedOtherRate != null) {
                            zoneItem.IsDirty = true;
                            otherRate.CurrentRateNewEED = closedOtherRate.EED;
                        }
                    }
                    if (ownerType == WhS_Sales_SalePriceListOwnerTypeEnum.Customer.value && zoneItem.CurrentOtherRates.length != 0)
                        otherRate.IsEEDDisabled = isEEDDisabled(otherRate);
                 

                    if (zoneItem.FutureOtherRates != null) {
                        otherRate.FutureRate = zoneItem.FutureOtherRates[otherRate.RateTypeId];
                    }

                    WhS_Sales_RatePlanUtilsService.onNewRateChanged(otherRate);
                    $scope.otherRates.push(otherRate);
                }
            }

            api.applyChanges = function () {
                if (!$scope.areOtherRatesEditable())
                    return;

                for (var i = 0; i < $scope.otherRates.length; i++) {
                    var otherRate = $scope.otherRates[i];
                    clearOtherRateDrafts(otherRate.RateTypeId);

                    if (otherRate.NewRate != null) {
                        zoneItem.NewRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.RateTypeId,   
                            Rate: otherRate.NewRate,
                            BED: $scope.NewRateBED,
                            EED: otherRate.NewRateEED
                        });
                        zoneItem.NewOtherRateBED = $scope.NewRateBED;
                    }
                    else if (!WhS_Sales_RatePlanService.areDatesTheSame(otherRate.CurrentRateEED, otherRate.CurrentRateNewEED)) {
                        zoneItem.ClosedRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.RateTypeId,
                            EED: otherRate.CurrentRateNewEED
                        });
                    }
                }
                if (zoneItem.NewRates != null && zoneItem.NewRates.length == 0)
                    zoneItem.NewRates = null;
                if (zoneItem.ClosedRates != null && zoneItem.ClosedRates.length == 0)
                    zoneItem.ClosedRates = null;
            };
            api.setBED = function (date) {
                $scope.NewRateBED = date;
                $scope.isNewRateBEDDisabled = date != null || $scope.isCountryEnded;
            };

            function clearOtherRateDrafts(rateTypeId) {
                if (!clearNewOtherRate(rateTypeId))
                    clearClosedOtherRate(rateTypeId);
            }
            function clearNewOtherRate(rateTypeId) {
                if (zoneItem.NewRates == null)
                    return false;
                var newOtherRateIndex = UtilsService.getItemIndexByVal(zoneItem.NewRates, rateTypeId, 'RateTypeId');
                if (newOtherRateIndex != -1) {
                    zoneItem.NewRates.splice(newOtherRateIndex, 1);
                    return true;
                }
                return false;
            }
            function clearClosedOtherRate(rateTypeId) {
                if (zoneItem.ClosedRates == null)
                    return false;
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
        function isEEDDisabled(otherRate) {
            var isEEDLoadedFromDrafts = false;
            console.log(otherRate);
            if (zoneItem.ClosedRates.length > 0)
            {
                for(var i = 0; i < zoneItem.ClosedRates.length; i++)
                {
                    var closedRate = zoneItem.ClosedRates[i];
                    if (closedRate.RateTypeId == otherRate.RateTypeId)
                        isEEDLoadedFromDrafts = true;
                }
            }
            return ((otherRate.NewRate != undefined && otherRate.NewRate != "") || otherRate.CurrentRate == null || !otherRate.IsCurrentRateEditable || otherRate.CurrentRateBED > VRDateTimeService.getNowDateTime() || $scope.isCountryEnded || $scope.isZonePendingClosed || (otherRate.CurrentRateNewEED != undefined && !isEEDLoadedFromDrafts) );
        }
        function isNewRateBEDRequired(date,rate) {
            return (date == undefined && rate > 0);
        }

    }
}]);