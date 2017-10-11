﻿'use strict';

app.directive('vrWhsSalesSalezoneroutingproductpreviewGrid', ['WhS_Sales_RatePlanPreviewAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (WhS_Sales_RatePlanPreviewAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
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
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Preview/Templates/SaleZoneRoutingProductPreviewGridTemplate.html'
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
                return WhS_Sales_RatePlanPreviewAPIService.GetFilteredSaleZoneRoutingProductPreviews(dataRetrievalInput).then(function (response) {
                    if (response != null && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i];
                            if (dataItem.Entity.IsCurrentSaleZoneRoutingProductInherited === true)
                                dataItem.Entity.CurrentSaleZoneRoutingProductName += ' (Inherited)';
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