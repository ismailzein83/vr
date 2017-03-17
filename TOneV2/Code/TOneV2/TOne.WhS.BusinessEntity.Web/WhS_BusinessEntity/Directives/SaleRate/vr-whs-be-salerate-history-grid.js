'use strict';

app.directive('vrWhsBeSaleRateHistoryGrid', ['WhS_BE_SaleRateHistoryAPIService', 'WhS_BE_RateChangeTypeEnum', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_BE_PrimarySaleEntityEnum', 'VRNotificationService', function (WhS_BE_SaleRateHistoryAPIService, WhS_BE_RateChangeTypeEnum, WhS_BE_SalePriceListOwnerTypeEnum, WhS_BE_PrimarySaleEntityEnum, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var saleRateHistoryGrid = new SaleRateHistoryGrid($scope, ctrl, $attrs);
            saleRateHistoryGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SaleRate/Templates/SaleRateHistoryGridTemplate.html'
    };
    function SaleRateHistoryGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        var ownerType;
        var primarySaleEntity;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.records = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleRateHistoryAPIService.GetFilteredSaleRateHistoryRecords(dataRetrievalInput).then(function (response) {
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

            api.load = function (payload) {
                var query;

                if (payload != undefined) {
                    query = payload.query;
                    primarySaleEntity = payload.primarySaleEntity;
                }

                ownerType = (query != undefined) ? query.OwnerType : null;
                $scope.scopeModel.isOwnerCustomer = (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value);

                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendRecord(record) {

            setChangeTypeIconProperties();
            setSourceIconProperties();

            function setChangeTypeIconProperties() {
                switch (record.Entity.ChangeType) {
                    case WhS_BE_RateChangeTypeEnum.New.value:
                        record.changeTypeIconType = WhS_BE_RateChangeTypeEnum.New.iconType;
                        record.changeTypeIconUrl = WhS_BE_RateChangeTypeEnum.New.iconUrl;
                        record.changeTypeIconDescription = WhS_BE_RateChangeTypeEnum.New.description;
                        break;
                    case WhS_BE_RateChangeTypeEnum.Increase.value:
                        record.changeTypeIconType = WhS_BE_RateChangeTypeEnum.Increase.iconType;
                        record.changeTypeIconUrl = WhS_BE_RateChangeTypeEnum.Increase.iconUrl;
                        record.changeTypeIconDescription = WhS_BE_RateChangeTypeEnum.Increase.description;
                        break;

                    case WhS_BE_RateChangeTypeEnum.Decrease.value:
                        record.changeTypeIconType = WhS_BE_RateChangeTypeEnum.Decrease.iconType;
                        record.changeTypeIconUrl = WhS_BE_RateChangeTypeEnum.Decrease.iconUrl;
                        record.changeTypeIconDescription = WhS_BE_RateChangeTypeEnum.Decrease.description;
                        break;
                    case WhS_BE_RateChangeTypeEnum.NotChanged.value:
                        record.changeTypeIconType = WhS_BE_RateChangeTypeEnum.NotChanged.iconType;
                        record.changeTypeIconUrl = WhS_BE_RateChangeTypeEnum.NotChanged.iconUrl;
                        record.changeTypeIconDescription = WhS_BE_RateChangeTypeEnum.NotChanged.description;
                        break;
                }
            }
            function setSourceIconProperties() {
                if (primarySaleEntity == undefined)
                    return;
                if (primarySaleEntity == WhS_BE_PrimarySaleEntityEnum.SellingProduct.value) {
                    if (record.Entity.SellingProductId == null)
                        record.sourceIconType = 'explicit';
                }
                else if (record.Entity.SellingProductId != null) {
                    record.sourceIconType = 'inherited';
                }
            }
        }
    }
}]);