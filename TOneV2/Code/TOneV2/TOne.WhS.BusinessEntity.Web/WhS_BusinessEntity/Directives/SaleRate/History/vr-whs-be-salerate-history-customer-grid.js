'use strict';

app.directive('vrWhsBeSalerateHistoryCustomerGrid', ['WhS_BE_SaleRateHistoryAPIService', 'WhS_BE_RateChangeTypeEnum', 'VRNotificationService', function (WhS_BE_SaleRateHistoryAPIService, WhS_BE_RateChangeTypeEnum, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var customerZoneRateHistoryGrid = new CustomerZoneRateHistoryGrid($scope, ctrl, $attrs);
            customerZoneRateHistoryGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SaleRate/History/Template/CustomerZoneRateHistoryGridTemplate.html'
    };
    function CustomerZoneRateHistoryGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.records = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleRateHistoryAPIService.GetFilteredCustomerZoneRateHistoryRecords(dataRetrievalInput).then(function (response) {
                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var record = response.Data[i];
                            extendRecord(record);
                            $scope.scopeModel.records.push(record);
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendRecord(record) {
            switch (record.Entity.ChangeType) {
                case WhS_BE_RateChangeTypeEnum.New.value:
                    record.iconType = WhS_BE_RateChangeTypeEnum.New.iconType;
                    record.iconUrl = WhS_BE_RateChangeTypeEnum.New.iconUrl;
                    break;
                case WhS_BE_RateChangeTypeEnum.Increase.value:
                    record.iconType = WhS_BE_RateChangeTypeEnum.Increase.iconType;
                    record.iconUrl = WhS_BE_RateChangeTypeEnum.Increase.iconUrl;
                    break;
                case WhS_BE_RateChangeTypeEnum.Decrease.value:
                    record.iconType = WhS_BE_RateChangeTypeEnum.Decrease.iconType;
                    record.iconUrl = WhS_BE_RateChangeTypeEnum.Decrease.iconUrl;
                    break;
            }
        }
    }
}]);