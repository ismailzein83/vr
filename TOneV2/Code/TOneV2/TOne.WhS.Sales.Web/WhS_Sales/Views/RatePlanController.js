(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_MainService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_BE_CustomerZoneAPIService",
        "WhS_Sales_RatePlanOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_BE_CustomerZoneAPIService, WhS_Sales_RatePlanOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRNotificationService) {
        
        var sellingProductSelectorAPI;
        var carrierAccountSelectorAPI;
        var gridAPI;
        var changedDataItems = [];

        defineScope();
        load();

        function defineScope() {
            /* Owner Type */

            $scope.ownerTypes = UtilsService.getArrayEnum(WhS_Sales_RatePlanOwnerTypeEnum);
            $scope.selectedOwnerType = $scope.ownerTypes[0];
            $scope.showSellingProductSelector = false;
            $scope.showCarrierAccountSelector = false;

            $scope.onOwnerTypeChanged = function () {
                if ($scope.selectedOwnerType != undefined) {
                    if ($scope.selectedOwnerType.value == WhS_Sales_RatePlanOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_RatePlanOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct = undefined;
                    }

                    clearRatePlan();
                }
            };

            /* Selling Product */

            $scope.selectedSellingProduct = undefined;

            $scope.onSellingProductSelectorReady = function (api) {
                sellingProductSelectorAPI = api;
                api.load(undefined);
            };

            $scope.onSellingProductChanged = function () {
                if ($scope.selectedSellingProduct != undefined) {
                    clearRatePlan();
                }
            };

            /* Carrier Account */

            $scope.selectedCustomer = undefined;

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                api.load(undefined);
            };

            $scope.onCarrierAccountChanged = function () {
                console.log($scope.selectedCustomer);

                if ($scope.selectedCustomer != undefined) {
                    clearRatePlan();
                }
            };

            /* Zone Letters */

            $scope.zoneLetters = [];

            $scope.zoneLetterConnector = {
                selectedZoneLetterIndex: 0,
                onZoneLetterSelectionChanged: function () {
                    loadGrid(true);
                }
            };

            /* Grid */

            $scope.dataItems = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.loadMoreData = function () {
                return loadGrid(false);
            };

            /* Action Bar */

            $scope.search = function () {
                return loadRatePlan();
            };

            $scope.sellNewZones = function () {
                var onCustomerZonesSold = function (customerZones) {
                    loadRatePlan();
                };
                
                WhS_Sales_MainService.sellNewZones(carrierAccountSelectorAPI.getData().CarrierAccountId, onCustomerZonesSold);
            };

            defineSaveButtonMenuActions();
        }

        function load() {

        }

        /* New Functions */

        function clearRatePlan() {
            $scope.zoneLetters.length = 0;
            $scope.dataItems.length = 0;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showGrid = show;
        }

        /* New Functions */

        function loadRatePlan() {
            return getZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    showRatePlan(true);
                    $scope.zoneLetterConnector.selectedZoneLetterIndex = 0;
                    return;

                    return loadGrid(true).then(function () {
                        return getRatePlanDraft();
                    });
                }

                else {
                    showRatePlan(false);
                }
            });
        }

        function getZoneLetters() {
            var customerId = carrierAccountSelectorAPI.getSelectedIds();

            return WhS_BE_CustomerZoneAPIService.GetCustomerZoneLetters(customerId).then(function (response) {
                if (response == null) return;

                $scope.zoneLetters = [];

                for (var i = 0; i < response.length; i++) {
                    $scope.zoneLetters.push(response[i]);
                }
            });
        }

        function loadGrid(clearDataAndContinuePaging) {
            $scope.isLoadingGrid = true;

            if (clearDataAndContinuePaging) {
                gridAPI.clearDataAndContinuePaging();
            }

            var input = buildRatePlanItemInput();

            return WhS_Sales_RatePlanAPIService.GetRatePlanItems(input).then(function (response) {
                if (response == null)
                    return;

                var ratePlanItems = [];

                for (var i = 0; i < response.length; i++) {
                    var item = response[i];
                    defineDataItemExtension(item);
                    ratePlanItems.push(item);
                }

                gridAPI.addItemsToSource(ratePlanItems);
                setChangedDataItems();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingGrid = false;
            });
        }

        function getRatePlanDraft() {
            var ownerType = WhS_BE_RatePlanOwnerTypeEnum.Customer.value;
            var ownerId = carrierAccountSelectorAPI.getData().CarrierAccountId;
            var status = WhS_BE_RatePlanStatusEnum.Draft.value;

            return WhS_Sales_RatePlanAPIService.GetRatePlan(ownerType, ownerId, status).then(function (ratePlan) {
                if (ratePlan != null) {
                    changedDataItems = [];

                    for (var i = 0; i < ratePlan.RatePlanItems.length; i++) {
                        var item = ratePlan.RatePlanItems[i];
                        changedDataItems.push(buildDataItem(item));
                    }
                }
            });
        }

        function defineDataItemExtension(ratePlanItem) {
            var extension = {
                NewRate: null,
                DisableBeginEffectiveDate: true,
                DisableEndEffectiveDate: true
            };

            extension.onRateChanged = function (dataItem) {
                var dataItemIndex = UtilsService.getItemIndexByVal(changedDataItems, dataItem.ZoneId, "ZoneId");

                if (dataItem.Extension.NewRate == undefined || dataItem.Extension.NewRate == null || dataItem.Extension.NewRate == "") {
                    if (dataItemIndex > -1) {
                        changedDataItems.splice(dataItemIndex, 1);
                    }
                }
                else {
                    if (dataItemIndex == -1) {
                        changedDataItems.push(dataItem);
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

            extension.onChildViewReady = function (api) {
                extension.ChildViewAPI = api;
                extension.onChildViewReady = undefined;
            };

            ratePlanItem.Extension = extension;
        }

        function setChangedDataItems() {
            for (var i = 0; i < changedDataItems.length; i++) {
                var changedDataItem = changedDataItems[i];
                var dataItem = UtilsService.getItemByVal($scope.dataItems, changedDataItem.ZoneId, "ZoneId");

                if (dataItem != undefined && dataItem != null) {
                    dataItem.Rate = changedDataItem.Rate;
                    dataItem.Extension.NewRate = changedDataItem.Extension.NewRate;

                    dataItem.BeginEffectiveDate = changedDataItem.BeginEffectiveDate;
                    dataItem.Extension.DisableBeginEffectiveDate = changedDataItem.Extension.DisableBeginEffectiveDate;

                    dataItem.EndEffectiveDate = changedDataItem.EndEffectiveDate;
                    dataItem.Extension.DisableEndEffectiveDate = changedDataItem.Extension.DisableEndEffectiveDate;

                    console.log(changedDataItem);
                    console.log(dataItem);
                    return;
                }
            }
        }

        function buildRatePlanItemInput() {
            var pageInfo = gridAPI.getPageInfo();

            var filter = {
                CustomerId: carrierAccountSelectorAPI.getData().CarrierAccountId,
                ZoneLetter: $scope.zoneLetters[$scope.zoneLetterConnector.selectedZoneLetterIndex],
                CountryId: null
            };

            return {
                Filter: filter,
                FromRow: pageInfo.fromRow,
                ToRow: pageInfo.toRow
            };
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [
                { name: "Save", clicked: savePriceList },
                { name: "Save Draft", clicked: saveRatePlanDraft },
            ];
        }

        function savePriceList() {
            var input = {
                CustomerId: carrierAccountSelectorAPI.getData().CarrierAccountId,
                NewSaleRates: buildNewSaleRates()
            };

            return WhS_Sales_RatePlanAPIService.SavePriceList(input).then(function (response) {
                changedDataItems = [];
                return loadGrid(true);
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function saveRatePlanDraft() {
            var input = {
                OwnerType: WhS_BE_RatePlanOwnerTypeEnum.Customer.value,
                OwnerId: carrierAccountSelectorAPI.getData().CarrierAccountId,
                RatePlanItems: buildItems(),
                Status: WhS_BE_RatePlanStatusEnum.Draft.value
            };

            return WhS_Sales_RatePlanAPIService.SaveRatePlanDraft(input).then(function () {
                console.log("Success: Draft has been saved");
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildNewSaleRates() {
            if (changedDataItems.length == 0)
                return null;

            var newSaleRates = [];

            for (var i = 0; i < changedDataItems.length; i++) {
                var item = changedDataItems[i];

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

        function buildDataItem(item) {
            return {
                ZoneId: item.ZoneId,
                ZoneName: item.ZoneName,
                SaleRateId: item.SaleRateId,
                Rate: item.Rate,
                BeginEffectiveDate: item.BeginEffectiveDate,
                EndEffectiveDate: item.EndEffectiveDate,
                Extension: {
                    NewRate: item.Rate,
                    DisableBeginEffectiveDate: false,
                    DisableEndEffectiveDate: false
                }
            };
        }

        function buildItems() {
            if (changedDataItems.length == 0)
                return null;

            var items = [];

            for (var i = 0; i < changedDataItems.length; i++) {
                var item = buildItem(changedDataItems[i]);
                items.push(item);
            }

            return items;
        }

        function buildItem(changedDataItem) {
            return {
                ZoneId: changedDataItem.ZoneId,
                ZoneName: changedDataItem.ZoneName,
                SaleRateId: changedDataItem.SaleRateId,
                Rate: changedDataItem.Extension.NewRate,
                BeginEffectiveDate: changedDataItem.BeginEffectiveDate,
                EndEffectiveDate: changedDataItem.EndEffectiveDate
            };
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
