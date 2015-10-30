(function (appControllers) {

    "use strict";

    RatePlanController.$inject = ["$scope", "WhS_Sales_MainService", "WhS_Sales_RatePlanAPIService", "WhS_BE_CustomerZoneAPIService", "UtilsService", "VRNotificationService"];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_BE_CustomerZoneAPIService, UtilsService, VRNotificationService) {
        
        var carrierAccountDirectiveAPI;
        var ratePlanGridAPI;
        var modifiedRatePlanItems = [];

        defineScope();
        load();

        function defineScope() {
            $scope.zoneLetters = [];
            $scope.ratePlanItems = [];
            $scope.disableSaveButton = true;

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
            };

            $scope.onCarrierAccountChanged = function () {
                $scope.showZoneLetters = false;
                $scope.showRatePlanGrid = false;
            };

            $scope.onRatePlanGridReady = function (api) {
                ratePlanGridAPI = api;
            };

            $scope.loadMoreData = function () {
                return loadRatePlanGrid(false);
            };

            $scope.search = function () {
                return loadRatePlan();
            };

            $scope.zoneLetterConnector = {
                selectedZoneLetterIndex: 0,
                onZoneLetterSelectionChanged: function () {
                    loadRatePlanGrid(true);
                }
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlan();
                };
                
                WhS_Sales_MainService.sellNewZones(carrierAccountDirectiveAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };

            $scope.savePriceList = function () {
                if (modifiedRatePlanItems.length > 0) {
                    var input = {
                        CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId,
                        NewSaleRates: buildNewSaleRates()
                    };

                    return WhS_Sales_RatePlanAPIService.SavePriceList(input).then(function (response) {
                        modifiedRatePlanItems = [];
                        return loadRatePlanGrid(true);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                }
            };
        }

        function load() {
            if (carrierAccountDirectiveAPI == undefined)
                return;

            $scope.loadingFilters = true;

            carrierAccountDirectiveAPI.load()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose($scope, error);
                })
                .finally(function () {
                    $scope.loadingFilters = false;
                });
        }

        function loadRatePlan() {
            return getZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    $scope.zoneLetterConnector.selectedZoneLetterIndex = 0;
                    return loadRatePlanGrid(true);
                }
            });
        }

        function getZoneLetters() {
            var customerId = carrierAccountDirectiveAPI.getData().CarrierAccountId;

            return WhS_BE_CustomerZoneAPIService.GetCustomerZoneLetters(customerId).then(function (response) {
                $scope.zoneLetters = [];

                for (var i = 0; i < response.length; i++) {
                    $scope.zoneLetters.push(response[i]);
                }

                $scope.showZoneLetters = ($scope.zoneLetters.length > 0);
            });
        }

        function loadRatePlanGrid(clearDataAndContinuePaging) {
            $scope.showRatePlanGrid = true;
            $scope.loadingRatePlanGrid = true;

            if (clearDataAndContinuePaging) {
                ratePlanGridAPI.clearDataAndContinuePaging();
            }

            var input = buildRatePlanItemInput();

            return WhS_Sales_RatePlanAPIService.GetRatePlanItems(input).then(function (response) {
                var ratePlanItems = [];

                for (var i = 0; i < response.length; i++) {
                    var item = response[i];
                    defineRatePlanItemExtension(item);
                    ratePlanItems.push(item);
                }

                ratePlanGridAPI.addItemsToSource(ratePlanItems);
                setModifiedRatePlanItems();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.loadingRatePlanGrid = false;
            });
        }

        function buildRatePlanItemInput() {
            var pageInfo = ratePlanGridAPI.getPageInfo();

            var filter = {
                CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId,
                ZoneLetter: $scope.zoneLetters[$scope.zoneLetterConnector.selectedZoneLetterIndex],
                CountryId: null
            };

            return {
                Filter: filter,
                FromRow: pageInfo.fromRow,
                ToRow: pageInfo.toRow
            };
        }

        function defineRatePlanItemExtension(ratePlanItem) {
            var extension = {
                NewRate: null,
                DisableBeginEffectiveDate: true,
                DisableEndEffectiveDate: true
            };

            extension.onRateChanged = function (dataItem) {
                var dataItemIndex = UtilsService.getItemIndexByVal(modifiedRatePlanItems, dataItem.ZoneId, "ZoneId");

                if (dataItem.Extension.NewRate == undefined || dataItem.Extension.NewRate == null || dataItem.Extension.NewRate == "") {
                    if (dataItemIndex > -1) {
                        modifiedRatePlanItems.splice(dataItemIndex, 1);
                        $scope.disableSaveButton = (modifiedRatePlanItems.length == 0);
                    }
                }
                else {
                    if (dataItemIndex == -1) {
                        modifiedRatePlanItems.push(dataItem);
                        $scope.disableSaveButton = false;
                    }
                }

                if (dataItem.Extension.DisableBeginEffectiveDate) {
                    dataItem.Extension.DisableBeginEffectiveDate = false;
                    dataItem.Extension.DisableEndEffectiveDate = false;

                    dataItem.BeginEffectiveDate = (Number(dataItem.Extension.NewRate) > Number(dataItem.Rate)) ?
                        new Date(new Date().setDate(new Date().getDate() + 7)) : new Date();

                    dataItem.EndEffectiveDate = null;
                }
            };

            ratePlanItem.Extension = extension;
        }

        function setModifiedRatePlanItems() {
            for (var i = 0; i < modifiedRatePlanItems.length; i++) {
                var item = modifiedRatePlanItems[i];
                var ratePlanItem = UtilsService.getItemByVal($scope.ratePlanItems, item.ZoneId, "ZoneId");

                if (ratePlanItem != undefined && ratePlanItem != null) {
                    ratePlanItem.Extension.NewRate = item.Extension.NewRate;

                    ratePlanItem.BeginEffectiveDate = item.BeginEffectiveDate;
                    ratePlanItem.Extension.DisableBeginEffectiveDate = item.Extension.DisableBeginEffectiveDate;

                    ratePlanItem.EndEffectiveDate = item.EndEffectiveDate;
                    ratePlanItem.Extension.DisableEndEffectiveDate = item.Extension.DisableEndEffectiveDate;
                }
            }
        }

        function buildNewSaleRates() {
            var newSaleRates = [];

            for (var i = 0; i < modifiedRatePlanItems.length; i++) {
                var item = modifiedRatePlanItems[i];

                newSaleRates.push({
                    SaleRateId: item.SaleRateId,
                    ZoneId: item.ZoneId,
                    NormalRate: item.Extension.NewRate,
                    BeginEffectiveDate: item.BeginEffectiveDate,
                    EndEffectiveDate: item.EndEffectiveDate
                });
            }

            return newSaleRates;
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
