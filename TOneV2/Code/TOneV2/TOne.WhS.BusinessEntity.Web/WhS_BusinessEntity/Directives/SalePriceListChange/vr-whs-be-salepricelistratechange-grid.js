"use strict";

app.directive("vrWhsBeSalepricelistratechangeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService", "WhS_BE_RateChangeTypeEnum", "VRUIUtilsService",
function (utilsService, vrNotificationService, whSBeSalePricelistChangeApiService, whSBeRateChangeTypeEnum, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new RateChangeGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/SalePriceListRateChangeTemplate.html"
    };

    function RateChangeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var pricelistId;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.RateChange = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, []);

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

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                if (dataRetrievalInput != undefined && dataRetrievalInput.Query != undefined)
                    pricelistId = dataRetrievalInput.Query.PriceListId;
                return whSBeSalePricelistChangeApiService.GetFilteredSalePriceListRateChanges(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                setRPService(item);
                                SetRateChangeIcon(item);
                                gridDrillDownTabsObj.setDrillDownExtensionObject(item);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
            function setRPService(item) {
                item.RPserviceViewerLoadDeferred = utilsService.createPromiseDeferred();
                item.onRPServiceViewerReady = function (api) {
                    item.serviceViewerAPI = api;
                    var routingProductserviceViewerPayload = { selectedIds: item.ServicesId };
                    VRUIUtilsService.callDirectiveLoad(item.serviceViewerAPI, routingProductserviceViewerPayload, item.RPserviceViewerLoadDeferred);
                };
            }
        }
        function SetRateChangeIcon(dataItem) {
            switch (dataItem.ChangeType) {
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
                case whSBeRateChangeTypeEnum.Deleted.value:
                    dataItem.RateChangeTypeIcon = whSBeRateChangeTypeEnum.Deleted.iconUrl;
                    dataItem.RateChangeTypeIconTooltip = whSBeRateChangeTypeEnum.Deleted.description;
                    dataItem.RateChangeTypeIconType = whSBeRateChangeTypeEnum.Deleted.iconType;
                    break;
            }
        }
        function getDrillDownDefinitions() {

            var drillDownDefinitions = [];

            drillDownDefinitions.push({
                title: 'Sale Code',
                directive: 'vr-whs-be-salepricelistcode-grid',
                loadDirective: function (directiveAPI, saleRate) {
                    var payload =
                    {
                        query: {
                            PriceListId: pricelistId,
                            ZoneId: saleRate.ZoneId
                        }
                    };
                    return directiveAPI.loadGrid(payload);
                }
            });
            drillDownDefinitions.push({
                title: "Other Rates",
                directive: "vr-whs-be-otherratespreview-grid",
                loadDirective: function (otherRatePreviewGridAPI, dataItem) {
                    dataItem.otherRatePreviewGridAPI = otherRatePreviewGridAPI;
                    var queryHandler = {
                        $type: "TOne.WhS.BusinessEntity.Business.OtherRatesPreviewHandler, TOne.WhS.BusinessEntity.Business"
                    };
                    queryHandler.Query = {
                        PriceListId: pricelistId,
                        ZoneName: dataItem.ZoneName
                    };

                    return otherRatePreviewGridAPI.load(queryHandler);
                }
            });
            return drillDownDefinitions;
        }

    }
    return directiveDefinitionObject;
}]);
