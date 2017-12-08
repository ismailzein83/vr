"use strict";

app.directive("vrWhsBeSupplierrateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierRateAPIService", "VRUIUtilsService", "FileAPIService", "WhS_BE_RateChangeTypeEnum",
function (UtilsService, VRNotificationService, WhS_BE_SupplierRateAPIService, VRUIUtilsService, fileApiService, whSBeRateChangeTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SupplierRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierRate/Templates/SupplierRateGridTemplate.html"

    };

    function SupplierRateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var effectiveOn;
        var supplierId;
        var isSystemCurrency;
        var isExpandable;
        this.initializeController = initializeController;
        var hideHistory;

        function initializeController() {

            $scope.supplierrates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (input) {
                        hideHistory = input.Query.HideHistory;
                        isExpandable = input.Query.IsChild;
                        effectiveOn = input.Query.EffectiveOn;
                        supplierId = input.Query.SupplierId;
                        isSystemCurrency = input.IsSystemCurrency;
                        drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);
                        return gridAPI.retrieveData(input);
                    };

                    return directiveAPI;
                }
            };
            $scope.isExpandable = function () {
                return !isExpandable;
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierRateAPIService.GetFilteredSupplierRates(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                SetRateChangeIcon(item);
                                drillDownManager.setDrillDownExtensionObject(item);
                            }

                        }

                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
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

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Download PriceList",
                clicked: downloadPriceList
            }];
        }
        function downloadPriceList(priceListObj) {
            fileApiService.DownloadFile(priceListObj.Entity.PriceListFileId)
                    .then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
        }
        function getDirectiveTabs() {
            var directiveTabs = [];

            var otherRatesTab = {
                title: "Other Rates",
                directive: "vr-whs-be-supplierotherrate-grid",
                loadDirective: function (directiveAPI, rateDataItem) {
                    rateDataItem.otherRateGridAPI = directiveAPI;

                    var otherRateGridPayload = {
                        ZoneId: rateDataItem.Entity.ZoneId,
                        EffectiveOn: effectiveOn,
                        IsSystemCurrency: isSystemCurrency
                    };
                    return rateDataItem.otherRateGridAPI.loadGrid(otherRateGridPayload);
                }
            };

            directiveTabs.push(otherRatesTab);
            if (!hideHistory) {
                var historyRatesTab = {
                    title: "History",
                    directive: "vr-whs-be-supplierrate-grid",
                    loadDirective: function (directiveApi, rateDataItem) {
                        rateDataItem.historygRateGridAPI = directiveApi;
                        var historyRateGridPayload = {
                            $type: "TOne.WhS.BusinessEntity.Business.SupplierRateHistoryQueryHandler,TOne.WhS.BusinessEntity.Business",
                            IsSystemCurrency: isSystemCurrency,
                            EffectiveOn: effectiveOn,
                            Query: {
                                SupplierZoneName: rateDataItem.SupplierZoneName,
                                SupplierId: supplierId,
                                IsChild: true
                            }
                        };
                        return rateDataItem.historygRateGridAPI.loadGrid(historyRateGridPayload);
                    }
                };
                directiveTabs.push(historyRatesTab);
            }
            return directiveTabs;
        }
    }

    return directiveDefinitionObject;

}]);
