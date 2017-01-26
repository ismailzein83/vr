'use strict';

app.directive('vrWhsSalesTqiGrid', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRValidationService',
    function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var tqiGrid = new TQIGrid($scope, ctrl, $attrs);
            tqiGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/TQI/Templates/TQIGridTemplate.html'
    };

    function TQIGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var drillDownManager;
        var selectedCountryIds;
        var effectiveDateDayOffset;
        var routingDatabaseId;
        var currencyId;
        var routingProductId;
        var saleZoneId;

        function initializeController() {

            $scope.analyticsData = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                defineAPI();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Sales_RatePlanAPIService.GetTQISuppliersInfo(dataRetrievalInput)
                    .then(function (response) {
                        if (response != null && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                $scope.analyticsData.push(response.Data[i]);
                                mapNeededData(response.Data[i]);
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var query = {};
                if (payload != undefined && payload.rpRouteDetail != undefined) {
                    query.RPRouteDetail = payload.rpRouteDetail;
                    query.PeriodType = payload.periodType;
                    query.PeriodValue = payload.periodValue;
                    currencyId = payload.currencyId;
                    routingProductId = payload.routingProductId;
                    routingDatabaseId = payload.routingDatabaseId;
                    saleZoneId = payload.saleZoneId;
                }
                return gridAPI.retrieveData(query);
            };

            api.getData = function () {

                if (selectedCountryIds.length == 0)
                    return null;

                return {
                    CountryIds: selectedCountryIds,
                    EED: $scope.scopeModel.endEffectiveDate
                };
            };



            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function mapNeededData(dataItem) {
            dataItem.onServiceReady = function (api) {
                dataItem.ServieApi = api;
                dataItem.ServieApi.load({ selectedIds: dataItem.ZoneServices });
            };
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var directiveTab = {
                title: "Supplier Route Options",
                directive: "vr-whs-routing-rprouteoption-grid",
                loadDirective: function (directiveAPI, tqiDataItem) {
                    tqiDataItem.supplierRouteOptionsGridAPI = directiveAPI;

                    var supplierRouteOptionsGridPayload = {
                        routingProductId: routingProductId,
                        saleZoneId: saleZoneId,
                        supplierId: tqiDataItem.SupplierId,
                        routingDatabaseId: routingDatabaseId,
                        currencyId: currencyId
                    };

                    return tqiDataItem.supplierRouteOptionsGridAPI.load(supplierRouteOptionsGridPayload);
                }
            };

            directiveTabs.push(directiveTab);

            return directiveTabs;
        }
    }
}]);