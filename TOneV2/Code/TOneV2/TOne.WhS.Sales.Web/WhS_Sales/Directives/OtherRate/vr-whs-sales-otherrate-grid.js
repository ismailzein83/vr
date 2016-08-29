"use strict";

app.directive("vrWhsSalesOtherrateGrid", ["UtilsService", "VRNotificationService", "VRCommon_RateTypeAPIService", 'WhS_Sales_RatePlanUtilsService', function (UtilsService, VRNotificationService, VRCommon_RateTypeAPIService, WhS_Sales_RatePlanUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var rateTypeGrid = new RateTypeGrid($scope, ctrl, $attrs);
            rateTypeGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Sales/Directives/OtherRate/Templates/OtherRateGridTemplate.html"
    };

    function RateTypeGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var zoneItem;
        var settings;

        var originalNewRates;
        var originalClosedRates;

        function initializeController() {

            $scope.otherRates = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.areOtherRatesEditable = function () {
                return (zoneItem.CurrentRate != null || zoneItem.NewRate != null);
            };

            $scope.onCurrentRateNewEEDChanged = function () {
                zoneItem.IsDirty = true;
            };

            $scope.onNewRateChanged = function (otherRate)
            {
                zoneItem.IsDirty = true;
                WhS_Sales_RatePlanUtilsService.onNewRateChanged(otherRate, settings, true);
            };

            $scope.validateNewRate = function (otherRate) {
                return WhS_Sales_RatePlanUtilsService.validateNewRate(otherRate);
            };

            $scope.validateNewRateDates = function (otherRate) {
                return WhS_Sales_RatePlanUtilsService.validateNewRateDates(otherRate);
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
            {
                return VRCommon_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput).then(function (response)
                {
                    if (response != null && response.Data != null)
                    {
                        for (var i = 0; i < response.Data.length; i++)
                        {
                            var otherRate = response.Data[i];

                            otherRate.ZoneBED = zoneItem.ZoneBED;
                            otherRate.ZoneEED = zoneItem.ZoneEED;

                            if (zoneItem.CurrentOtherRates != null)
                            {
                                var currentOtherRate = zoneItem.CurrentOtherRates[otherRate.Entity.RateTypeId];

                                if (currentOtherRate != undefined)
                                {
                                    otherRate.CurrentRate = currentOtherRate.Rate;
                                    otherRate.CurrentRateBED = currentOtherRate.BED;
                                    otherRate.CurrentRateEED = currentOtherRate.EED;
                                    otherRate.CurrentRateNewEED = currentOtherRate.EED;
                                }
                            }
                            
                            if (originalNewRates != null)
                            {
                                var newOtherRate = UtilsService.getItemByVal(originalNewRates, otherRate.Entity.RateTypeId, 'RateTypeId');

                                if (newOtherRate != null)
                                {
                                    zoneItem.IsDirty = true;
                                    otherRate.NewRate = newOtherRate.NormalRate;
                                    otherRate.NewRateBED = newOtherRate.BED;
                                    otherRate.NewRateEED = newOtherRate.EED;
                                }
                            }

                            if (originalClosedRates != null)
                            {
                                var closedOtherRate = UtilsService.getItemByVal(originalClosedRates, otherRate.Entity.RateTypeId, 'RateTypeId');

                                if (closedOtherRate != null)
                                {
                                    zoneItem.IsDirty = true;
                                    otherRate.CurrentRateNewEED = closedOtherRate.EED;
                                }
                            }

                            WhS_Sales_RatePlanUtilsService.onNewRateChanged(otherRate);
                        }
                    }

                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
        }

        function defineAPI() {

            var api = {};

            api.loadGrid = function (query) {
                zoneItem = query.zoneItem;
                settings = query.settings;

                if (zoneItem.NewRates != null) {
                    originalNewRates = [];
                    for (var i = 0; i < zoneItem.NewRates.length; i++)
                        originalNewRates.push(zoneItem.NewRates[i]);
                }
                if (zoneItem.ClosedRates != null) {
                    originalClosedRates = [];
                    for (var i = 0; i < zoneItem.ClosedRates.length; i++)
                        originalClosedRates.push(zoneItem.ClosedRates[i]);
                }
                zoneItem.NewRates = [];
                zoneItem.ClosedRates = [];

                return gridAPI.retrieveData(query);
            };
            
            api.applyChanges = function ()
            {
                if (!$scope.areOtherRatesEditable())
                    return;
                
                for (var i = 0; i < $scope.otherRates.length; i++)
                {
                    var otherRate = $scope.otherRates[i];

                    if (otherRate.NewRate != null)
                    {
                        zoneItem.NewRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.Entity.RateTypeId,
                            NormalRate: otherRate.NewRate,
                            BED: otherRate.NewRateBED,
                            EED: otherRate.NewRateEED
                        });
                    }
                    else if (!compareDates(otherRate.CurrentRateEED, otherRate.CurrentRateNewEED))
                    {
                        zoneItem.ClosedRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.Entity.RateTypeId,
                            EED: otherRate.CurrentRateNewEED
                        });
                    }
                }
            };

            function compareDates(date1, date2) {
                if (date1 && date2) {
                    if (typeof date1 == 'string')
                        date1 = new Date(date1);
                    if (typeof date2 == 'string')
                        date2 = new Date(date2);
                    return (date1.getDay() == date2.getDay() && date1.getMonth() == date2.getMonth() && date1.getYear() == date2.getYear());
                }
                else if (!date1 && !date2)
                    return true;
                else
                    return false;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
