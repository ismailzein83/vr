'use strict';

app.directive('vrWhsSalesRatepreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_RateChangeTypeEnum', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_RateChangeTypeEnum)
{
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePreviewGrid = new RatePreviewGrid($scope, ctrl, $attrs);
            ratePreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/RatePreviewGridTemplate.html'
    };

    function RatePreviewGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        var gridAPI;
        var gridDrillDownTabs;
        var processInstanceId;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.ratePreviews = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, null);

                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanPreviewAPIService.GetFilteredRatePreviews(dataRetrievalInput).then(function (response) {
                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++)
                            extendDataItem(response.Data[i]);
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                ctrl.isNormalRateGrid = (query.ZoneName == null);
                processInstanceId = query.ProcessInstanceId;
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getDrillDownDefinitions() {
            return [{
                title: "Other Rates",
                directive: "vr-whs-sales-ratepreview-grid",
                loadDirective: function (otherRatePreviewGridAPI, dataItem)
                {
                    dataItem.otherRatePreviewGridAPI = otherRatePreviewGridAPI;
                    var otherRatePreviewGridQuery = {
                        ProcessInstanceId: processInstanceId,
                        ZoneName: dataItem.Entity.ZoneName
                    };
                    return otherRatePreviewGridAPI.load(otherRatePreviewGridQuery);
                }
            }];
        }

        function extendDataItem(dataItem)
        {
            if (ctrl.isNormalRateGrid)
                gridDrillDownTabs.setDrillDownExtensionObject(dataItem);

            var rateChangeType = UtilsService.getEnum(WhS_BE_RateChangeTypeEnum, 'value', dataItem.Entity.ChangeType);
            
            if (rateChangeType != undefined) {
                dataItem.RateChangeTypeIconType = rateChangeType.iconType;
                dataItem.RateChangeTypeIconUrl = rateChangeType.iconUrl;
                dataItem.RateChangeTypeIconTooltip = rateChangeType.description;
            }
        }
    }
}]);