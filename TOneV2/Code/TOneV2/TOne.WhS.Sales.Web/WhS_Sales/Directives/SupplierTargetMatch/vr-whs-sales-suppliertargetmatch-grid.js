'use strict';

app.directive('vrWhsSalesSuppliertargetmatchGrid', ['WhS_Sales_SupplierTargetMatchAPIService', 'UtilsService', 'VRNotificationService', 'VRValidationService', 'VRUIUtilsService', 'UISettingsService','VRCommon_CurrencyAPIService',
    function (WhS_Sales_SupplierTargetMatchAPIService, UtilsService, VRNotificationService, VRValidationService, VRUIUtilsService, UISettingsService, VRCommon_CurrencyAPIService ) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new SupplierTargetMatchGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/SupplierTargetMatch/Templates/SupplierTargetMatchGrid.html'
        };

        function SupplierTargetMatchGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;

            var selectedCountryIds;
            var effectiveDate;
            var systemCurrencyId;
            var currencyPromisedDeferred;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.targetMatches = [];
                $scope.longPrecision = UISettingsService.getLongPrecision();
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Sales_SupplierTargetMatchAPIService.GetFilteredSupplierTargetMatches(dataRetrievalInput).then(function (response) {
                        if (response != null && response.Data != null) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var targetMatch = response.Data[i];
                                extendTargetMatchItem(targetMatch);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.scopeModel.search = function () {
                    return gridAPI.retrieveData(gridQuery);
                };
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    currencyPromisedDeferred = UtilsService.createPromiseDeferred();
                    setSystemCurrency();

                    promises.push(currencyPromisedDeferred.promise);

                    if (payload != undefined) {
                        gridQuery = payload;
                    }
                    var gridPromise = gridAPI.retrieveData(gridQuery);

                    promises.push(gridPromise);
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.export = function (query) {
                    return gridAPI.exportData(query);
                };
                api.getData = function () {

                    return {

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendTargetMatchItem(targetMatchItem) {
                targetMatchItem.RouteOptionsLoadDeferred = UtilsService.createPromiseDeferred();

                targetMatchItem.onRouteOptionsReady = function (api) {
                    targetMatchItem.RouteOptionsAPI = api;
                    var routeOptionsDirectivePayload = getRouteOptionsDirectivePayload(targetMatchItem);
                    VRUIUtilsService.callDirectiveLoad(targetMatchItem.RouteOptionsAPI, routeOptionsDirectivePayload, targetMatchItem.RouteOptionsLoadDeferred);
                };
            }

            function getRouteOptionsDirectivePayload(dataItem) {
                return {
                    RouteOptions: dataItem.Options,
                    SaleZoneId: dataItem.SaleZoneId,
                    RoutingDatabaseId: gridQuery.RoutingDataBaseId,
                    RoutingProductId: gridQuery.RoutingProductId,
                    CurrencyId: systemCurrencyId,
                    AnalyticDetails: {
                        ACD: 0
                    }
                };
            }
            function setSystemCurrency() {
                return VRCommon_CurrencyAPIService.GetSystemCurrency().then(function (response) {
                    if (response != undefined) {
                        currencyPromisedDeferred.resolve();
                        systemCurrencyId = response.CurrencyId;
                    }
                });
            }
        }
    }]);