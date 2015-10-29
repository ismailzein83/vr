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

            $scope.search = function () {
                return loadZoneLetters().then(function () {
                    if ($scope.zoneLetters.length > 0) {
                        return loadRatePlanGrid($scope.zoneLetters[0]);
                    }
                });
            };

            $scope.loadRatePlanGrid = function (zoneLetter) {
                loadRatePlanGrid(zoneLetter);
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    return loadZoneLetters().then(function () {
                        if ($scope.zoneLetters.length > 0) {
                            return loadRatePlanGrid($scope.zoneLetters[0]);
                        }
                    });
                };
                
                WhS_Sales_MainService.sellNewZones(carrierAccountDirectiveAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };

            $scope.savePriceList = function () {
                if (modifiedRatePlanItems.length > 0) {
                    var input = {
                        CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId,
                        NewSaleRates: GetNewSaleRates()
                    };

                    return WhS_Sales_RatePlanAPIService.SavePriceList(input).then(function (response) {
                        modifiedRatePlanItems = [];
                        return loadRatePlanGrid($scope.zoneLetters[0]); // $scope.zoneLetters[0] is defined in this case
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

        function loadZoneLetters() {
            if (carrierAccountDirectiveAPI != undefined) {
                var customerId = carrierAccountDirectiveAPI.getData().CarrierAccountId;

                return WhS_BE_CustomerZoneAPIService.GetCustomerZoneLetters(customerId).then(function (response) {
                    $scope.zoneLetters = [];

                    for (var i = 0; i < response.length; i++) {
                        $scope.zoneLetters.push(response[i]);
                    }

                    $scope.showZoneLetters = ($scope.zoneLetters.length > 0);
                });
            }
        }

        function loadRatePlanGrid(zoneLetter) {
            if (ratePlanGridAPI != undefined) {
                $scope.ratePlanItems = [];
                $scope.showRatePlanGrid = true;

                var query = {
                    CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId,
                    ZoneLetter: zoneLetter,
                    CountryId: null
                };

                return WhS_Sales_RatePlanAPIService.GetRatePlanItems(query).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        var ratePlanItem = response[i];

                        ratePlanItem.onRateChanged = function (dataItem) {
                            var dataItemIndex = UtilsService.getItemIndexByVal(modifiedRatePlanItems, dataItem.ZoneId, "ZoneId");
                            
                            if (dataItem.NewRate == undefined || dataItem.NewRate == null || dataItem.NewRate == "") {
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
                        };
                        ratePlanItem.BeginEffectiveDate = new Date();

                        $scope.ratePlanItems.push(ratePlanItem);
                    }

                    setModifiedRatePlanItems();
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }
        }

        function setModifiedRatePlanItems() {
            for (var i = 0; i < modifiedRatePlanItems.length; i++) {
                var item = modifiedRatePlanItems[i];
                var ratePlanItem = UtilsService.getItemByVal($scope.ratePlanItems, item.ZoneId, "ZoneId");

                if (ratePlanItem != undefined && ratePlanItem != null) {
                    ratePlanItem.NewRate = item.NewRate;
                }
            }
        }

        function GetNewSaleRates() {
            var newSaleRates = [];

            for (var i = 0; i < modifiedRatePlanItems.length; i++) {
                var item = modifiedRatePlanItems[i];

                newSaleRates.push({
                    SaleRateId: item.SaleRateId,
                    ZoneId: item.ZoneId,
                    NormalRate: item.NewRate,
                    BeginEffectiveDate: item.BeginEffectiveDate,
                    EndEffectiveDate: item.EndEffectiveDate
                });
            }

            return newSaleRates;
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
