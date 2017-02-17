"use strict";

app.directive("vrWhsBeSalepricelistratechangeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService", "WhS_BE_RateChangeTypeEnum",
function (utilsService, vrNotificationService, whSBeSalePricelistChangeApiService, whSBeRateChangeTypeEnum) {

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
        this.initializeController = initializeController;

        function initializeController() {

            $scope.RateChange = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

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
                return whSBeSalePricelistChangeApiService.GetFilteredSalePriceListRateChanges(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                SetRateChangeIcon(item);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
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
            }
        }
    }
    return directiveDefinitionObject;
}]);
