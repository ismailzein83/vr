
'use strict';

app.directive('vrWhsBeOtherratespreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_RateChangeTypeEnum', 'WhS_BE_OtherRatesPreviewAPIService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_RateChangeTypeEnum, WhS_BE_OtherRatesPreviewAPIService) {
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
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/OtherRatesPreviewGridTemplate.html'
    };

    function RatePreviewGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;
        var gridDrillDownTabs;
        var processInstanceId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.ratePreviews = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_OtherRatesPreviewAPIService.GetFilteredRatePreviews(dataRetrievalInput).then(function (response) {
                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var otherRatepreview = response.Data[i];
                            extendDataItem(otherRatepreview);
                        }
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
            api.gridHasData = function () {
                return ($scope.scopeModel.ratePreviews.length != 0) ? true : false;
            };
            api.cleanGrid = function () {
                $scope.scopeModel.ratePreviews.length = 0
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendDataItem(dataItem) {
            var rateChangeType = UtilsService.getEnum(WhS_BE_RateChangeTypeEnum, 'value', dataItem.ChangeType);
            if (rateChangeType != undefined) {
                dataItem.RateChangeTypeIconType = rateChangeType.iconType;
                dataItem.RateChangeTypeIconUrl = rateChangeType.iconUrl;
                dataItem.RateChangeTypeIconTooltip = rateChangeType.description;
            }
        }
    }
}]);