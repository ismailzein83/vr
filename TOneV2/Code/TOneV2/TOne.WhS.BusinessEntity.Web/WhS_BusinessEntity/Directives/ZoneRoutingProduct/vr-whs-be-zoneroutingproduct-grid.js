"use strict";

app.directive("vrWhsBeZoneroutingproductGrid", ["UtilsService", "VRNotificationService", "WhS_BE_ZoneRoutingProductAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_BE_PrimarySaleEntityEnum", "VRUIUtilsService",
function (utilsService, vrNotificationService, whSBeZoneRoutingProductApiService, whSBeSalePriceListOwnerTypeEnum, whSBePrimarySaleEntityEnum, vruiUtilsService) {

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
        var gridApi;
        var gridQuery;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.zoneRoutingProduct = [];
            $scope.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridApi.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                gridQuery = dataRetrievalInput.Query;
                return whSBeZoneRoutingProductApiService.GetFilteredZoneRoutingProducts(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                setRateIconProperties(item);
                                setService(item);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
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
