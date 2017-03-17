"use strict";

app.directive("vrWhsBeSalerateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleRateAPIService", "VRUIUtilsService", 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_PrimarySaleEntityEnum', 'WhS_BE_RateChangeTypeEnum',
function (utilsService, vrNotificationService, whSBeSaleRateApiService, vruiUtilsService, whSBeSalePriceListOwnerTypeEnum, whSBePrimarySaleEntityEnum, whSBeRateChangeTypeEnum) {

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

        var primarySaleEntity;

        function initializeController() {
            $scope.showGrid = false;
            $scope.salerates = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
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

                                var drillDownDefinitions = getDrillDownDefinitions(item);
                                var gridDrillDownTabsObj = vruiUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
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

        function getDrillDownDefinitions(dataItem) {

            var drillDownDefinitions = [];

            if (dataItem.OtherRates != null && dataItem.OtherRates.length > 0) {
                drillDownDefinitions.push({
                    title: 'Other Rates',
                    directive: 'vr-whs-be-saleotherrate-grid',
                    loadDirective: function (directiveAPI, rateItem) {
                        rateItem.otherRateGridAPI = directiveAPI;
                        rateItem.otherRateGridAPI.loadGrid(rateItem.OtherRates);
                    }
                });
            }

            drillDownDefinitions.push({
                title: 'History',
                directive: 'vr-whs-be-sale-rate-history-grid',
                loadDirective: function (directiveAPI, dataItem) {
                    var directivePayload = {
                        query: {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            SellingNumberPlanId: gridQuery.SellingNumberPlanId,
                            ZoneName: dataItem.ZoneName,
                            CountryId: dataItem.CountryId,
                            CurrencyId: gridQuery.CurrencyId
                        }
                    };
                    directivePayload.primarySaleEntity = primarySaleEntity;
                    return directiveAPI.load(directivePayload);
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
