'use strict';

app.directive('vrWhsBeSaleentityzoneroutingproductHistoryGrid', ['WhS_BE_SaleEntityZoneRoutingProductHistoryAPIService', 'WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum', 'WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_BE_SaleEntityZoneRoutingProductHistoryAPIService, WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum, WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var saleEntityZoneRoutingProductHistoryGrid = new SaleEntityZoneRoutingProductHistoryGrid($scope, ctrl, $attrs);
            saleEntityZoneRoutingProductHistoryGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/ZoneRoutingProduct/Templates/SaleEntityZoneRoutingProductHistoryGridTemplate.html'
    };
    function SaleEntityZoneRoutingProductHistoryGrid($scope, ctrl, $attrs) {
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

                var promises = [];

                var getRecordsPromise = WhS_BE_SaleEntityZoneRoutingProductHistoryAPIService.GetFilteredSaleEntityZoneRoutingProductHistoryRecords(dataRetrievalInput);
                promises.push(getRecordsPromise);

                var serviceViewersLoadedDeferred = UtilsService.createPromiseDeferred();
                promises.push(serviceViewersLoadedDeferred.promise);

                getRecordsPromise.then(function (response) {
                    var serviceViewerLoadPromises = [];

                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var record = response.Data[i];
                            extendRecord(record);
                            serviceViewerLoadPromises.push(record.serviceViewerLoadDeferred.promise);
                            $scope.scopeModel.records.push(record);
                        }
                    }

                    UtilsService.waitMultiplePromises(serviceViewerLoadPromises).then(function () {
                        serviceViewersLoadedDeferred.resolve();
                    }).catch(function (error) {
                        serviceViewersLoadedDeferred.reject(error);
                    });

                    onResponseReady(response);
                });

                return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {

                var ownerType;

                if (query != undefined) {
                    ownerType = query.OwnerType;
                }

                if (ownerType != undefined) {
                    $scope.scopeModel.isOwnerCustomer = (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value);
                }

                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function extendRecord(record) {

            setServiceViewerMembers();
            setSourceDescription();

            function setServiceViewerMembers() {
                record.serviceViewerLoadDeferred = UtilsService.createPromiseDeferred();
                record.onServiceViewerReady = function (api) {
                    record.serviceViewerAPI = api;
                    var serviceViewerPayload = { selectedIds: record.ServiceIds };
                    VRUIUtilsService.callDirectiveLoad(api, serviceViewerPayload, record.serviceViewerLoadDeferred);
                };
            }
            function setSourceDescription() {
                switch (record.Entity.Source) {
                    case WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.CustomerZone.value:
                        record.sourceDescription = WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.CustomerZone.description;
                        break;
                    case WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.CustomerDefault.value:
                        record.sourceDescription = WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.CustomerDefault.description;
                        break;
                    case WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.ProductZone.value:
                        record.sourceDescription = WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.ProductZone.description;
                        break;
                    case WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.ProductDefault.value:
                        record.sourceDescription = WhS_BE_SaleEntityZoneRoutingProductSourceTypeEnum.ProductDefault.description;
                        break;
                }
            }
        }
    }
}]);