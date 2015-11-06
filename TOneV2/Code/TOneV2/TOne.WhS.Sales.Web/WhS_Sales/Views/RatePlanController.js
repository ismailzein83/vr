(function (appControllers) {

    "use strict";

    RatePlanController.$inject = [
        "$scope",
        "WhS_Sales_MainService",
        "WhS_Sales_RatePlanAPIService",
        "WhS_Sales_RatePlanOwnerTypeEnum",
        "WhS_Sales_RatePlanStatusEnum",
        "UtilsService",
        "VRNotificationService"
    ];

    function RatePlanController($scope, WhS_Sales_MainService, WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, UtilsService, VRNotificationService) {
        
        var ownerSelectorAPI;
        var sellingProductSelectorAPI;
        var carrierAccountSelectorAPI;
        var gridAPI;

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
                        $scope.carrierAccountConnector.selectedCustomer = undefined;
                        carrierAccountSelectorAPI = undefined;
                    }
                    else if ($scope.selectedOwnerType.value == WhS_Sales_RatePlanOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.sellingProductConnector.selectedSellingProduct = undefined;
                        sellingProductSelectorAPI = undefined;
                    }

                    clearRatePlan();
                }
            };

            /* Selling Product */

            $scope.sellingProductConnector = {
                selectedSellingProduct: undefined,

                onSellingProductSelectorReady: function (api) {
                    sellingProductSelectorAPI = api;
                    api.load(undefined);
                },

                onSellingProductChanged: function () {
                    if ($scope.selectedSellingProduct != undefined) {
                        clearRatePlan();
                    }
                }
            };

            /* Carrier Account */

            $scope.carrierAccountConnector = {
                selectedCustomer: undefined,
                
                onCarrierAccountSelectorReady: function (api) {
                    carrierAccountSelectorAPI = api;
                    api.load(undefined);
                },
                
                onCarrierAccountChanged: function () {
                    if ($scope.selectedCustomer != undefined) {
                        clearRatePlan();
                    }
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

            $scope.zoneItems = [];

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
            $scope.zoneItems.length = 0;
            showRatePlan(false);
        }

        function showRatePlan(show) {
            $scope.showZoneLetters = show;
            $scope.showGrid = show;
        }

        /* New Functions */

        function loadRatePlan() {
            return loadZoneLetters().then(function () {
                if ($scope.zoneLetters.length > 0) {
                    showRatePlan(true);
                    return loadGrid(true);
                }
            });
        }

        function loadZoneLetters() {
            var ownerId = (sellingProductSelectorAPI != undefined) ?
                $scope.sellingProductConnector.selectedSellingProduct.SellingProductId :
                $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId;

            return WhS_Sales_RatePlanAPIService.GetZoneLetters($scope.selectedOwnerType.value, ownerId).then(function (response) {
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

            var input = buildZoneItemInput();

            return WhS_Sales_RatePlanAPIService.GetZoneItems(input).then(function (response) {
                if (response == null)
                    return;

                var zoneItems = [];

                for (var i = 0; i < response.length; i++) {
                    var item = response[i];
                    extendZoneItem(item);
                    zoneItems.push(item);
                }

                gridAPI.addItemsToSource(zoneItems);
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingGrid = false;
            });
        }

        function buildZoneItemInput() {
            var pageInfo = gridAPI.getPageInfo();

            var filter = {
                OwnerType: $scope.selectedOwnerType.value,
                OwnerId: (sellingProductSelectorAPI != undefined) ?
                    $scope.sellingProductConnector.selectedSellingProduct.SellingProductId :
                    $scope.carrierAccountConnector.selectedCustomer.CarrierAccountId,
                ZoneLetter: $scope.zoneLetters[$scope.zoneLetterConnector.selectedZoneLetterIndex]
            };

            return {
                Filter: filter,
                FromRow: pageInfo.fromRow,
                ToRow: pageInfo.toRow
            };
        }

        function extendZoneItem(zoneItem) {
            zoneItem.DisableBeginEffectiveDate = true;
            zoneItem.DisableEndEffectiveDate = true;

            zoneItem.onNewRateChanged = function (dataItem) {

            };

            zoneItem.onChildViewReady = function (api) {
                zoneItem.ChildViewAPI = api;
                delete zoneItem.onChildViewReady;
            };
        }

        function defineSaveButtonMenuActions() {
            $scope.saveButtonMenuActions = [
                { name: "Price List", clicked: savePriceList },
                { name: "Draft", clicked: saveDraft },
            ];
        }

        function savePriceList() {
            console.log("savePriceList");
        }

        function saveDraft() {
            console.log("saveDraft");
        }

        function junkCode() {
            /* ### */
            if (dataItem.DisableBeginEffectiveDate) {
                dataItem.DisableBeginEffectiveDate = false;
                dataItem.DisableEndEffectiveDate = false;

                dataItem.BeginEffectiveDate = (Number(dataItem.Extension.NewRate) > Number(dataItem.Rate)) ?
                    new Date(new Date().setDate(new Date().getDate() + 7)) : new Date();

                dataItem.EndEffectiveDate = null;
            }
            /* ### */
        }
    }

    appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);
