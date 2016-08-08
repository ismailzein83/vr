"use strict";

app.directive("vrWhsSalesOtherrateGrid", ["UtilsService", "VRNotificationService", "VRCommon_RateTypeAPIService",
function (UtilsService, VRNotificationService, VRCommon_RateTypeAPIService) {

    var directiveDefinitionObject = {

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

        function initializeController() {

            $scope.otherRates = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.areOtherRatesEditable = function () {
                if (zoneItem == undefined)
                    return false;
                return (zoneItem.CurrentRate != null || zoneItem.NewRate != null);
            };

            $scope.onValueChanged = function () {
                if (zoneItem != undefined)
                    zoneItem.IsDirty = true;
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_RateTypeAPIService.GetFilteredRateTypes(dataRetrievalInput).then(function (response) {

                    if (response != null && response.Data != null)
                    {
                        for (var i = 0; i < response.Data.length; i++)
                        {
                            var otherRate = response.Data[i];

                            if (zoneItem.CurrentOtherRates != null)
                                otherRate.currentRate = zoneItem.CurrentOtherRates[otherRate.Entity.RateTypeId];

                            if (zoneItem.NewRates != null)
                            {
                                var newOtherRate = UtilsService.getItemByVal(zoneItem.NewRates, otherRate.Entity.RateTypeId, 'RateTypeId');

                                if (newOtherRate != null)
                                {
                                    zoneItem.IsDirty = true;
                                    otherRate.newRate = newOtherRate.NormalRate;
                                }
                            }
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
                return gridAPI.retrieveData(query);
            };

            api.onRateTypeAdded = function (rateType) {
                gridAPI.itemAdded(rateType);
            };

            api.applyChanges = function (zoneChanges) {
                if (!$scope.areOtherRatesEditable())
                    return;

                var otherRateBED;
                var otherRateEED;
                var isRateClosed = (zoneItem.IsCurrentRateEditable === true && compareDates(zoneItem.currentRateBED, zoneItem.CurrentRateBED) == 0);

                if (zoneItem.NewRate != null) {
                    otherRateBED = zoneItem.NewRateBED;
                    otherRateEED = zoneItem.NewRateEED;
                }
                else {
                    otherRateBED = zoneItem.CurrentRateBED;
                    otherRateEED = zoneItem.CurrentRateEED;
                }

                // Add new rates
                for (var i = 0; i < $scope.otherRates.length; i++) {
                    var otherRate = $scope.otherRates[i];

                    if (otherRate.newRate != null) {
                        if (zoneChanges.NewRates == null)
                            zoneChanges.NewRates = [];

                        zoneChanges.NewRates.push(
                        {
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: otherRate.Entity.RateTypeId,
                            NormalRate: otherRate.newRate,
                            BED: otherRateBED,
                            EED: otherRateEED
                        });
                    }
                    else if (isRateClosed && otherRate.currentRate != null)
                    {
                        zoneChanges.ClosedRates.push({
                            RateTypeId: otherRate.Entity.RateTypeId,
                            EED: otherRateEED
                        });
                    }
                }

                console.log(zoneChanges);

                function compareDates(date1, date2) {
                    var d1 = new Date(date1);
                    var d2 = new Date(date2);

                    var year1 = d1.getYear();
                    var year2 = d2.getYear();
                    if (year1 > year2)
                        return 1;
                    if (year2 > year1)
                        return 2;

                    var month1 = d1.getMonth();
                    var month2 = d2.getMonth();
                    if (month1 > month2)
                        return 1;
                    if (month2 > month1)
                        return 2;

                    var day1 = d1.getDay();
                    var day2 = d2.getDay();
                    if (day1 > day2)
                        return 1;
                    if (day2 > day1)
                        return 2;

                    return 0;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;

}]);
