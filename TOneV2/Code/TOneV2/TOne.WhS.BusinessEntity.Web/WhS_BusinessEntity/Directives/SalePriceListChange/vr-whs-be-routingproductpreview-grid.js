'use strict';

app.directive('vrWhsBeRoutingproductpreviewGrid', ['WhS_BE_SalePriceListChangeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_BE_SalePriceListChangeAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var saleZoneRoutingProductPreviewGrid = new SaleZoneRoutingProductPreviewGrid($scope, ctrl, $attrs);
            saleZoneRoutingProductPreviewGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/RoutingProductPreviewGrid.html'
    };

    function SaleZoneRoutingProductPreviewGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.saleZoneRoutingProductPreviews = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePriceListChangeAPIService.GetFilteredRoutingProductPreviews(dataRetrievalInput).then(function (response) {
                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            addReadyCurrentSericeApi(response.Data[i]);
                            addReadyNewSericeApi(response.Data[i]);

                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }
        var addReadyCurrentSericeApi = function (dataItem) {
            dataItem.onCurrentServiceReady = function (api) {
                dataItem.ServieApi = api;
                dataItem.ServieApi.load({ selectedIds: dataItem.RecentRouringProductServicesId });
            };
        };
        var addReadyNewSericeApi = function (dataItem) {
            dataItem.onNewServiceReady = function (api) {
                dataItem.ServieApi = api;
                dataItem.ServieApi.load({ selectedIds: dataItem.RoutingProductServicesId });
            };
        };
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                if (query != null)
                    $scope.scopeModel.showCustomerName = query.ShowCustomerName;
                return gridAPI.retrieveData(query);
            };
            api.gridHasData = function () {
                return ($scope.scopeModel.saleZoneRoutingProductPreviews.length != 0) ? true : false;
            };
            api.cleanGrid = function () {
                $scope.scopeModel.saleZoneRoutingProductPreviews.length = 0
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);