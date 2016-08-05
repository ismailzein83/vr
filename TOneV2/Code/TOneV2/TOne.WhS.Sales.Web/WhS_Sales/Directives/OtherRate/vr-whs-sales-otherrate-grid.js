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
                    var rateType = $scope.otherRates[i];

                    if (rateType.newRate != null) {
                        if (zoneChanges.NewRates == null)
                            zoneChanges.NewRates = [];

                        zoneChanges.NewRates.push({
                            ZoneId: zoneItem.ZoneId,
                            RateTypeId: rateType.Entity.RateTypeId,
                            NormalRate: rateType.newRate,
                            BED: otherRateBED,
                            EED: otherRateEED
                        });
                    }
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;

}]);
