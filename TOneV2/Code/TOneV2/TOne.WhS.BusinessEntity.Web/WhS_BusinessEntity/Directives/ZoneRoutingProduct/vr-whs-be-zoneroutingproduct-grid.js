"use strict";

app.directive("vrWhsBeZoneroutingproductGrid", ["UtilsService", "VRNotificationService", "WhS_BE_ZoneRoutingProductAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_BE_PrimarySaleEntityEnum", "VRUIUtilsService", "WhS_BE_SaleEntityZoneRoutingProductService",
function (utilsService, vrNotificationService, whSBeZoneRoutingProductApiService, whSBeSalePriceListOwnerTypeEnum, whSBePrimarySaleEntityEnum, vruiUtilsService, WhS_BE_SaleEntityZoneRoutingProductService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new ZoneRoutingProuductGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/ZoneRoutingProduct/Templates/ZoneRoutingProductGridTemplate.html"
    };

    function ZoneRoutingProuductGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridApi;
        var gridQuery;
        var gridDrillDownTabs;
        var customerId;
        var sellingNumberPlanId;

        function initializeController() {
            $scope.zoneRoutingProduct = [];

            $scope.onGridReady = function (api) {
                gridApi = api;
                var gridDrillDownDefinitions = getGridDrillDownDefinitions();
                gridDrillDownTabs = vruiUtilsService.defineGridDrillDownTabs(gridDrillDownDefinitions, gridApi);
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                customerId = dataRetrievalInput.Query.OwnerId;
                sellingNumberPlanId = dataRetrievalInput.Query.SellingNumberPlanId;
                var promises = [];

                var getFilteredZoneRoutingProductsPromise = whSBeZoneRoutingProductApiService.GetFilteredZoneRoutingProducts(dataRetrievalInput);
                promises.push(getFilteredZoneRoutingProductsPromise);

                var serviceViewersLoadedDeferred = utilsService.createPromiseDeferred();
                promises.push(serviceViewersLoadedDeferred.promise);

                getFilteredZoneRoutingProductsPromise.then(function (response) {
                    var serviceViewerLoadPromises = [];

                    if (response != undefined && response.Data != null) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var item = response.Data[i];

                            gridDrillDownTabs.setDrillDownExtensionObject(item);
                            setRateIconProperties(item);

                            setService(item);
                            serviceViewerLoadPromises.push(item.serviceViewerLoadDeferred.promise);
                        }
                    }

                    utilsService.waitMultiplePromises(serviceViewerLoadPromises).then(function () {
                        serviceViewersLoadedDeferred.resolve();
                    }).catch(function (error) {
                        serviceViewersLoadedDeferred.reject(error);
                    });

                    onResponseReady(response);
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                });

                return utilsService.waitMultiplePromises(promises);
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRoutingProduct
            }];
        }

        function editRoutingProduct(routingProductObj) {
            var onZoneRoutingProductUpdated = function (updatedObj) {
                gridApi.itemUpdated(updatedObj);
                gridDrillDownTabs.setDrillDownExtensionObject(updatedObj);
                setRateIconProperties(updatedObj);
                setService(updatedObj);
            };
            WhS_BE_SaleEntityZoneRoutingProductService.editSaleEntityZoneRouting(customerId,sellingNumberPlanId, routingProductObj.Entity.ZoneId,
             routingProductObj.ZoneName, routingProductObj.Entity.RoutingProductId, onZoneRoutingProductUpdated);
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                gridQuery = query;
                return gridApi.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getGridDrillDownDefinitions() {
            return [{
                title: "History",
                directive: "vr-whs-be-saleentityzoneroutingproduct-history-grid",
                loadDirective: function (directiveAPI, dataItem) {
                    var directivePayload = {
                        query: {
                            OwnerType: gridQuery.OwnerType,
                            OwnerId: gridQuery.OwnerId,
                            SellingNumberPlanId: gridQuery.SellingNumberPlanId,
                            ZoneName: dataItem.ZoneName,
                            CountryId: dataItem.Entity.CountryId
                        },
                        primarySaleEntity: gridQuery.PrimarySaleEntity
                    };
                    return directiveAPI.load(directivePayload);
                }
            }];
        }
        function setService(item) {
            item.serviceViewerLoadDeferred = utilsService.createPromiseDeferred();
            item.onServiceViewerReady = function (api) {
                item.serviceViewerAPI = api;
                var serviceViewerPayload = { selectedIds: item.Entity.ServiceIds };
                vruiUtilsService.callDirectiveLoad(item.serviceViewerAPI, serviceViewerPayload, item.serviceViewerLoadDeferred);
            };
        }
        function setRateIconProperties(dataItem) {
            if (gridQuery.OwnerType === whSBeSalePriceListOwnerTypeEnum.SellingProduct.value)
                return;
            if (gridQuery.PrimarySaleEntity == null)
                return;
            if (gridQuery.PrimarySaleEntity === whSBePrimarySaleEntityEnum.SellingProduct.value) {
                if (dataItem.Entity.IsInherited === false) {
                    dataItem.iconType = 'explicit';
                    dataItem.iconTooltip = 'Explicit';
                }
            }
            else if (dataItem.Entity.IsInherited === true) {
                dataItem.iconType = 'inherited';
                dataItem.iconTooltip = 'Inherited';
            }
        }
    }
    return directiveDefinitionObject;

}]);