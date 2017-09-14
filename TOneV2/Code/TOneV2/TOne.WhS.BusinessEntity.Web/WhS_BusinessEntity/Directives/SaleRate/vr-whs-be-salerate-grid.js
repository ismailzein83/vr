"use strict";

app.directive("vrWhsBeSalerateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleRateAPIService", "VRUIUtilsService", 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_PrimarySaleEntityEnum', 'WhS_BE_RateChangeTypeEnum', 'FileAPIService',
function (utilsService, vrNotificationService, whSBeSaleRateApiService, vruiUtilsService, whSBeSalePriceListOwnerTypeEnum, whSBePrimarySaleEntityEnum, whSBeRateChangeTypeEnum, FileAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SaleRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleRate/Templates/SaleRateGridTemplate.html"

    };

    function SaleRateGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var gridQuery;
        var gridDrillDownTabsObj;

        var primarySaleEntity;

        function initializeController() {
            $scope.showGrid = false;
            $scope.salerates = [];

            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabsObj = vruiUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return whSBeSaleRateApiService.GetFilteredSaleRate(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                setNormalRateIconProperties(item);
                                SetRateChangeIcon(item);
                                gridDrillDownTabsObj.setDrillDownExtensionObject(item);
                            }
                        }
                        $scope.showGrid = true;
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyException(error, $scope);
                    });
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    gridQuery = payload.query;
                    primarySaleEntity = payload.primarySaleEntity;
                }
                return gridAPI.retrieveData(gridQuery);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];

                if (dataItem.PriceListFileId != null) {
                    menuActions.push({
                        name: 'Download Pricelist',
                        clicked: downloadPriceList
                    });
                }

                return menuActions;
            };
        }
        function downloadPriceList(dataItem) {
            if (dataItem.PriceListFileId != null && dataItem.PriceListFileId != 0) {
                FileAPIService.DownloadFile(dataItem.PriceListFileId).then(function (response) {
                    utilsService.downloadFile(response.data, response.headers);
                });
            }
            else {
                vrNotificationService.showInformation('No file has been created for the pricelist of this rate');
            }
        }

        function getDrillDownDefinitions() {

            var drillDownDefinitions = [];

            drillDownDefinitions.push({
                title: 'History',
                directive: 'vr-whs-be-sale-rate-history-grid',
                loadDirective: function (directiveAPI, saleRate) {
                    var directivePayload = {
                        query: {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            SellingNumberPlanId: gridQuery.SellingNumberPlanId,
                            ZoneName: saleRate.ZoneName,
                            CountryId: saleRate.CountryId,
                            CurrencyId: gridQuery.CurrencyId,
                            IsSystemCurrency: gridQuery.IsSystemCurrency
                        }
                    };
                    directivePayload.primarySaleEntity = primarySaleEntity;
                    return directiveAPI.load(directivePayload);
                }
            });

            drillDownDefinitions.push({
                title: 'Other Rates',
                directive: 'vr-whs-be-othersalerate-grid',
                loadDirective: function (directiveAPI, saleRate) {
                    saleRate.otherRateGridAPI = directiveAPI;

                    var otherSaleRateGridPayload = {
                        query: {
                            ZoneName: saleRate.ZoneName,
                            ZoneId: saleRate.Entity.ZoneId,
                            CountryId: saleRate.CountryId
                        }
                    };

                    if (gridQuery != undefined) {
                        otherSaleRateGridPayload.query.SellingNumberPlanId = gridQuery.SellingNumberPlanId;
                        otherSaleRateGridPayload.query.OwnerType = gridQuery.OwnerType;
                        otherSaleRateGridPayload.query.OwnerId = gridQuery.OwnerId;
                        otherSaleRateGridPayload.query.CurrencyId = gridQuery.CurrencyId;
                        otherSaleRateGridPayload.query.EffectiveOn = gridQuery.EffectiveOn;
                        otherSaleRateGridPayload.query.IsSystemCurrency = gridQuery.IsSystemCurrency;
                    }
                    return directiveAPI.load(otherSaleRateGridPayload);
                }
            });

            return drillDownDefinitions;
        }

        function setNormalRateIconProperties(dataItem) {
            if (gridQuery.OwnerType === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value)
                return;
            if (primarySaleEntity == undefined || primarySaleEntity == null)
                return;
            if (primarySaleEntity === whSBePrimarySaleEntityEnum.SellingProduct.value) {
                if (dataItem.IsRateInherited === false) {
                    dataItem.iconType = 'explicit';
                    dataItem.iconTooltip = 'Explicit';
                }
            }
            else if (dataItem.IsRateInherited === true) {
                dataItem.iconType = 'inherited';
                dataItem.iconTooltip = 'Inherited';
            }
        }
        function SetRateChangeIcon(dataItem) {
            switch (dataItem.Entity.RateChange) {
                case whSBeRateChangeTypeEnum.New.value:
                    dataItem.RateChangeTypeIcon = whSBeRateChangeTypeEnum.New.iconUrl;
                    dataItem.RateChangeTypeIconTooltip = whSBeRateChangeTypeEnum.New.description;
                    dataItem.RateChangeTypeIconType = whSBeRateChangeTypeEnum.New.iconType;
                    break;
                case whSBeRateChangeTypeEnum.Increase.value:
                    dataItem.RateChangeTypeIcon = whSBeRateChangeTypeEnum.Increase.iconUrl;
                    dataItem.RateChangeTypeIconTooltip = whSBeRateChangeTypeEnum.Increase.description;
                    dataItem.RateChangeTypeIconType = whSBeRateChangeTypeEnum.Increase.iconType;
                    break;

                case whSBeRateChangeTypeEnum.Decrease.value:
                    dataItem.RateChangeTypeIcon = whSBeRateChangeTypeEnum.Decrease.iconUrl;
                    dataItem.RateChangeTypeIconTooltip = whSBeRateChangeTypeEnum.Decrease.description;
                    dataItem.RateChangeTypeIconType = whSBeRateChangeTypeEnum.Decrease.iconType;
                    break;
                case whSBeRateChangeTypeEnum.NotChanged.value:
                    dataItem.RateChangeTypeIcon = whSBeRateChangeTypeEnum.NotChanged.iconUrl;
                    dataItem.RateChangeTypeIconTooltip = whSBeRateChangeTypeEnum.NotChanged.description;
                    dataItem.RateChangeTypeIconType = whSBeRateChangeTypeEnum.NotChanged.iconType;
                    break;
            }
        }
    }
    return directiveDefinitionObject;

}]);
