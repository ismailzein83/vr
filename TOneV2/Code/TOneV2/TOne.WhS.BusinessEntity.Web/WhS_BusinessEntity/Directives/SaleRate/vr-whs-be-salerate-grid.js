﻿"use strict";

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

        var gridAPI;
        var gridQuery;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.showGrid = false;
            var gridDrillDownTabsObj;
            $scope.salerates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Other Rates";
                drillDownDefinition.directive = "vr-whs-be-saleotherrate-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, rateItem) {
                    rateItem.otherRateGridAPI = directiveAPI;
                    rateItem.otherRateGridAPI.loadGrid(rateItem.OtherRates);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = vruiUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.isExpandable = function (dataItem) {
                return (dataItem.OtherRates != null && dataItem.OtherRates.length > 0);
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                gridQuery = dataRetrievalInput.Query;
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
        function setNormalRateIconProperties(dataItem) {
            if (gridQuery.OwnerType === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value)
                return;
            if (gridQuery.PrimarySaleEntity == undefined || gridQuery.PrimarySaleEntity == null)
                return;
            if (gridQuery.PrimarySaleEntity === whSBePrimarySaleEntityEnum.SellingProduct.value) {
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
