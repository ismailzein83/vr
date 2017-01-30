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
        var rpRouteDetail;
        var periodType;
        var periodValue;

        function initializeController() {

            $scope.analyticsData = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                defineAPI();
            };

        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];
                var query = {};
                if (payload != undefined && payload.rpRouteDetail != undefined) {
                    rpRouteDetail = payload.rpRouteDetail;
                    periodType = payload.periodType;
                    periodValue = payload.periodValue;
                    currencyId = payload.currencyId;
                    routingProductId = payload.routingProductId;
                    routingDatabaseId = payload.routingDatabaseId;
                    saleZoneId = payload.saleZoneId;
                }

                var loadTQIGridPromise = loadTQIGrid();
                promises.push(loadTQIGridPromise);

                return UtilsService.waitMultiplePromises(promises);
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

        function loadTQIGrid() {
            $scope.analyticsData.length = 0;
            return WhS_Sales_RatePlanAPIService.GetTQISuppliersInfo(buildTQIGridInputObject())
                   .then(function (response) {
                       if (response != null) {
                           for (var i = 0; i < response.SuppliersInfo.length; i++) {
                               $scope.analyticsData.push(response.SuppliersInfo[i]);
                               mapNeededData(response.SuppliersInfo[i]);
                               drillDownManager.setDrillDownExtensionObject(response.SuppliersInfo[i]);
                           }

                           if (response.TotalDurationInMinutesSummary != null)
                               gridAPI.setSummary({
                                   TotalDurationInMinutesSummary: response.TotalDurationInMinutesSummary,
                               });
                       }
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyException(error, $scope);
                   });
        } 
        

        function buildTQIGridInputObject() {
            return {
                RPRouteDetail: rpRouteDetail,
                PeriodType: periodType,
                PeriodValue: periodValue
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